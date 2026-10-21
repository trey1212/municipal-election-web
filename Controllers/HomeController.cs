using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Models;
using municipal_election_web.Data;


namespace municipal_election_web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var municipalities = await _context.Municipalities.ToListAsync();
        return View(municipalities);
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

        return View(municipality);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
