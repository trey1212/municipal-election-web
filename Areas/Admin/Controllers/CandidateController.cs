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

    public async Task<IActionResult> Index()
    {
        var query = _context.Candidates
            .Include(c => c.Position)
            .Include(c => c.Municipality)
            .AsQueryable();

        if (User.IsInRole("MunicipalityAdmin"))
        {
            var adminMunicipalityId = int.Parse(User.FindFirstValue("MunicipalityId")!);
            query = query.Where(c => c.MunicipalityId == adminMunicipalityId);
        }

        return View(await query.ToListAsync());
    }

    // CREATE Functions
    public IActionResult Create()
    {
        PopulateDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCandidate model)
    {
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

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        PopulateDropdowns(model.MunicipalityId, model.PositionId);
        return View(model);
    }

    // Helper method to generate the dropdown options
    private void PopulateDropdowns(int? selectedMunicipality = null, int? selectedPosition = null)
    {
        var municipalitiesQuery = _context.Municipalities.AsQueryable();

        if (User.IsInRole("MunicipalityAdmin"))
        {
            var adminMunicipalityId = int.Parse(User.FindFirstValue("MunicipalityId")!);
            municipalitiesQuery = municipalitiesQuery.Where(m => m.MunicipalityId == adminMunicipalityId);
        }

        ViewBag.Municipalities = new SelectList(municipalitiesQuery, "MunicipalityId", "Name", selectedMunicipality);
        var uniquePositions = _context.Positions
            .AsEnumerable()
            .DistinctBy(p => p.Name)
            .ToList();

        ViewBag.Positions = new SelectList(uniquePositions, "PositionId", "Name", selectedPosition);
    }

    // UPDATE Functions
    public async Task<IActionResult> Edit(int id)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null) return NotFound();

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
        return View("Update", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateCandidate model)
    {
        if (id != model.CandidateId) return NotFound();

        if (User.IsInRole("MunicipalityAdmin"))
        {
            var adminMunicipalityId = int.Parse(User.FindFirstValue("MunicipalityId")!);
            if (model.MunicipalityId != adminMunicipalityId) return Forbid();
        }

        if (ModelState.IsValid)
        {
            var candidateToUpdate = await _context.Candidates.FindAsync(model.CandidateId);
            if (candidateToUpdate == null) return NotFound();

            if (User.IsInRole("MunicipalityAdmin") && candidateToUpdate.MunicipalityId != int.Parse(User.FindFirstValue("MunicipalityId")!))
            {
                return Forbid();
            }

            candidateToUpdate.Name = model.Name;
            candidateToUpdate.Biography = model.Biography;
            candidateToUpdate.PositionId = model.PositionId;
            candidateToUpdate.MunicipalityId = model.MunicipalityId;

            if (model.NewPhotoUpload != null)
            {
                if (!string.IsNullOrEmpty(candidateToUpdate.PhotoUrl))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, candidateToUpdate.PhotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

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

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        PopulateDropdowns(model.MunicipalityId, model.PositionId);
        return View(model);
    }

    // DELETE Functions
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null) return NotFound();

        if (User.IsInRole("MunicipalityAdmin") && candidate.MunicipalityId != int.Parse(User.FindFirstValue("MunicipalityId")!))
        {
            return Forbid();
        }

        if (!string.IsNullOrEmpty(candidate.PhotoUrl))
        {
            var imagePath = Path.Combine(_environment.WebRootPath, candidate.PhotoUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
        }

        _context.Candidates.Remove(candidate);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }
}