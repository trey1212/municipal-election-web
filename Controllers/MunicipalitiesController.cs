using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Data;
using System.Threading.Tasks;

namespace municipal_election_web.Controllers;

public class MunicipalitiesController : Controller
{
    private readonly ApplicationDbContext _context;

    public MunicipalitiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> MunicipalityBrowser()
    {
        var municipalities = await _context.Municipalities.ToListAsync();
        return View("MunicipalityBrowser", municipalities);
    }

    public async Task<IActionResult> Candidates(int id)
    {
        var municipality = await _context.Municipalities
            .Include(m => m.Candidates)
                .ThenInclude(c => c.Position)
            .FirstOrDefaultAsync(m => m.MunicipalityId == id);

        if (municipality == null)
        {
            return NotFound();
        }

        municipality.Candidates = municipality.Candidates.OrderBy(c => c.PositionId).ToList();

        return View("MunicipalityDetails", municipality);
    }
}