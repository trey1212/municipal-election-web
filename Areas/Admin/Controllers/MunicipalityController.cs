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

    // GET: /Admin/Municipalities
    public async Task<IActionResult> Index()
    {
        var municipalities = await _context.Municipalities.ToListAsync();
        return View(municipalities);
    }

    // GET: /Admin/Municipalities/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Admin/Municipalities/Create
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

            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: /Admin/Municipalities/Edit/5
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

        return View(model);
    }

    // POST: /Admin/Municipalities/Edit/5
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

            // 1. Update text properties
            municipalityToUpdate.Name = model.Name;
            municipalityToUpdate.Province = model.Province;
            municipalityToUpdate.ElectionDate = model.ElectionDate;
            municipalityToUpdate.Description = model.Description;

            // 2. Check if a NEW logo was uploaded
            if (model.NewLogoUpload != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.NewLogoUpload.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewLogoUpload.CopyToAsync(stream);
                }

                // Overwrite the old logo path
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
                    // Update the existing user's username instead of creating a new one
                    var newUsername = model.Name.ToLower().Replace(" ", "");
                    adminUser.UserName = newUsername;
                    await _userManager.UpdateAsync(adminUser);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }
}