using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Areas.Admin.Models;
using municipal_election_web.Data;
using municipal_election_web.Models;

namespace municipal_election_web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public class MunicipalitiesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public MunicipalitiesController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
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

            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }
}