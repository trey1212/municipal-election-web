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
}