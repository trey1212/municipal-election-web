using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Areas.Admin.Models;
using municipal_election_web.Data;
using municipal_election_web.Models;
using System.Security.Claims;

namespace municipal_election_web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public class MunicipalitiesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly UserManager<IdentityUser> _userManager;

    public MunicipalitiesController(ApplicationDbContext context,
                                    IWebHostEnvironment environment,
                                    UserManager<IdentityUser> userManager)
    {
        _context = context;
        _environment = environment;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var municipalities = await _context.Municipalities.ToListAsync();
        return View(municipalities);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMunicipality model)
    {
        if (ModelState.IsValid)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.LogoUpload.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.LogoUpload.CopyToAsync(stream);
            }

            var municipality = new Municipality
            {
                Name = model.Name,
                Province = model.Province,
                ElectionDate = model.ElectionDate,
                Description = model.Description,
                LogoUrl = "/images/" + uniqueFileName
            };

            _context.Municipalities.Add(municipality);
            await _context.SaveChangesAsync();

            var adminUsername = model.Name.ToLower().Replace(" ", "");
            var user = new IdentityUser { UserName = adminUsername };
            var result = await _userManager.CreateAsync(user, "P@$$w0rd");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "MunicipalityAdmin");
                await _userManager.AddClaimAsync(user, new Claim("MunicipalityId", municipality.MunicipalityId.ToString()));
            }

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var municipality = await _context.Municipalities.FindAsync(id);
        if (municipality == null) return NotFound();

        // Map the database record to the Edit ViewModel
        var model = new UpdateMunicipality
        {
            MunicipalityId = municipality.MunicipalityId,
            Name = municipality.Name,
            Province = municipality.Province,
            ElectionDate = municipality.ElectionDate,
            Description = municipality.Description,
            ExistingLogoUrl = municipality.LogoUrl
        };

        return View("Update", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateMunicipality model)
    {
        if (id != model.MunicipalityId) return NotFound();

        if (ModelState.IsValid)
        {
            var municipalityToUpdate = await _context.Municipalities.FindAsync(model.MunicipalityId);
            if (municipalityToUpdate == null) return NotFound();

            bool nameChanged = municipalityToUpdate.Name != model.Name;

            municipalityToUpdate.Name = model.Name;
            municipalityToUpdate.Province = model.Province;
            municipalityToUpdate.ElectionDate = model.ElectionDate;
            municipalityToUpdate.Description = model.Description;

            if (model.NewLogoUpload != null)
            {
                if (!string.IsNullOrEmpty(municipalityToUpdate.LogoUrl))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, municipalityToUpdate.LogoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.NewLogoUpload.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewLogoUpload.CopyToAsync(stream);
                }

                municipalityToUpdate.LogoUrl = "/images/" + uniqueFileName;
            }

            _context.Update(municipalityToUpdate);
            await _context.SaveChangesAsync();

            if (nameChanged)
            {
                var users = await _userManager.GetUsersForClaimAsync(new Claim("MunicipalityId", municipalityToUpdate.MunicipalityId.ToString()));
                var adminUser = users.FirstOrDefault();

                if (adminUser != null)
                {
                    var newUsername = model.Name.ToLower().Replace(" ", "");
                    adminUser.UserName = newUsername;
                    await _userManager.UpdateAsync(adminUser);
                }
            }

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var municipality = await _context.Municipalities.FindAsync(id);
        if (municipality == null) return NotFound();

        if (!string.IsNullOrEmpty(municipality.LogoUrl))
        {
            var imagePath = Path.Combine(_environment.WebRootPath, municipality.LogoUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
        }

        var users = await _userManager.GetUsersForClaimAsync(new Claim("MunicipalityId", municipality.MunicipalityId.ToString()));
        var adminUser = users.FirstOrDefault();
        if (adminUser != null) await _userManager.DeleteAsync(adminUser);

        _context.Municipalities.Remove(municipality);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }
}