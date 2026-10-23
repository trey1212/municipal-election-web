using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Data;

namespace municipal_election_web.Controllers;

public class CandidatesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CandidatesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> CandidatesBrowser(int? municipalityId, int? positionId)
    {
        // Start with the base query including related data
        var query = _context.Candidates
            .Include(c => c.Position)
            .Include(c => c.Municipality)
            .AsQueryable();

        // Apply filters only if the user selected them
        if (municipalityId.HasValue)
        {
            query = query.Where(c => c.MunicipalityId == municipalityId);
        }

        if (positionId.HasValue)
        {
            query = query.Where(c => c.PositionId == positionId);
        }

        // Pass data for the dropdown menus, keeping the user's current selection active
        ViewBag.Municipalities = new SelectList(await _context.Municipalities.ToListAsync(), "MunicipalityId", "Name", municipalityId);
        ViewBag.Positions = new SelectList(await _context.Positions.ToListAsync(), "PositionId", "Name", positionId);

        return View("CandidatesBrowser", await query.ToListAsync());
    }
}