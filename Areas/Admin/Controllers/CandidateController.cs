using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Areas.Admin.Models;
using municipal_election_web.Data;
using municipal_election_web.Models;
using System.Security.Claims;

namespace municipal_election_web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin, MunicipalityAdmin")]
public class CandidatesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public CandidatesController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // GET: /Admin/Candidates
    public async Task<IActionResult> Index()
    {
        var query = _context.Candidates
            .Include(c => c.Position)
            .Include(c => c.Municipality)
            .AsQueryable();

        // Scope data for Municipality Admins
        if (User.IsInRole("MunicipalityAdmin"))
        {
            var adminMunicipalityId = int.Parse(User.FindFirstValue("MunicipalityId")!);
            query = query.Where(c => c.MunicipalityId == adminMunicipalityId);
        }

        return View(await query.ToListAsync());
    }

    // GET: /Admin/Candidates/Create
    public IActionResult Create()
    {
        PopulateDropdowns();
        return View();
    }

    // POST: /Admin/Candidates/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCandidate model)
    {
        // Make sure the municipality admin can only CRUD their municipality
        if (User.IsInRole("MunicipalityAdmin"))
        {
            model.MunicipalityId = int.Parse(User.FindFirstValue("MunicipalityId")!);
        }

        if (ModelState.IsValid)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "candidates");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.PhotoUpload.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.PhotoUpload.CopyToAsync(stream);
            }

            var candidate = new Candidate
            {
                Name = model.Name,
                Biography = model.Biography,
                PositionId = model.PositionId,
                MunicipalityId = model.MunicipalityId,
                PhotoUrl = "/images/candidates/" + uniqueFileName
            };

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        PopulateDropdowns(model.MunicipalityId, model.PositionId);
        return View(model);
    }

    // Helper method to generate the <select> dropdown options for the Razor Views
    private void PopulateDropdowns(int? selectedMunicipality = null, int? selectedPosition = null)
    {
        var municipalitiesQuery = _context.Municipalities.AsQueryable();

        if (User.IsInRole("MunicipalityAdmin"))
        {
            var adminMunicipalityId = int.Parse(User.FindFirstValue("MunicipalityId")!);
            municipalitiesQuery = municipalitiesQuery.Where(m => m.MunicipalityId == adminMunicipalityId);
        }

        ViewBag.Municipalities = new SelectList(municipalitiesQuery, "MunicipalityId", "Name", selectedMunicipality);
        ViewBag.Positions = new SelectList(_context.Positions, "PositionId", "Name", selectedPosition);
    }

    // GET: /Admin/Candidates/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null) return NotFound();

        // Ensures the municipal admin can only edit their respective municipality
        if (User.IsInRole("MunicipalityAdmin"))
        {
            var adminMunicipalityId = int.Parse(User.FindFirstValue("MunicipalityId")!);
            if (candidate.MunicipalityId != adminMunicipalityId) return Forbid();
        }

        var model = new UpdateCandidate
        {
            CandidateId = candidate.CandidateId,
            Name = candidate.Name,
            Biography = candidate.Biography,
            PositionId = candidate.PositionId,
            MunicipalityId = candidate.MunicipalityId,
            ExistingPhotoUrl = candidate.PhotoUrl
        };

        PopulateDropdowns(model.MunicipalityId, model.PositionId);
        return View(model);
    }

    // POST: /Admin/Candidates/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateCandidate model)
    {
        if (id != model.CandidateId) return NotFound();

        // Security check: Lock the Municipality ID for Municipality Admins
        if (User.IsInRole("MunicipalityAdmin"))
        {
            var adminMunicipalityId = int.Parse(User.FindFirstValue("MunicipalityId")!);
            if (model.MunicipalityId != adminMunicipalityId) return Forbid();
        }

        if (ModelState.IsValid)
        {
            var candidateToUpdate = await _context.Candidates.FindAsync(model.CandidateId);
            if (candidateToUpdate == null) return NotFound();

            // One final verification that the admin owns this original record
            if (User.IsInRole("MunicipalityAdmin") && candidateToUpdate.MunicipalityId != int.Parse(User.FindFirstValue("MunicipalityId")!))
            {
                return Forbid();
            }

            candidateToUpdate.Name = model.Name;
            candidateToUpdate.Biography = model.Biography;
            candidateToUpdate.PositionId = model.PositionId;
            candidateToUpdate.MunicipalityId = model.MunicipalityId;

            // Check if a NEW photo was uploaded
            if (model.NewPhotoUpload != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "candidates");
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.NewPhotoUpload.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewPhotoUpload.CopyToAsync(stream);
                }

                candidateToUpdate.PhotoUrl = "/images/candidates/" + uniqueFileName;
            }

            _context.Update(candidateToUpdate);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        PopulateDropdowns(model.MunicipalityId, model.PositionId);
        return View(model);
    }
}