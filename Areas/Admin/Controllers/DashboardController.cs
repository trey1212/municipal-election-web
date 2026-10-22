using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Data;
using System.Security.Claims;

namespace municipal_election_web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,MunicipalityAdmin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var isSuperAdmin = User.IsInRole("SuperAdmin");
        var isMunicipalityAdmin = User.IsInRole("MunicipalityAdmin");

        ViewBag.IsSuperAdmin = isSuperAdmin;
        ViewBag.IsMunicipalityAdmin = isMunicipalityAdmin;

        if (isMunicipalityAdmin)
        {
            var adminMunicipalityId = int.Parse(User.FindFirstValue("MunicipalityId")!);
            var municipality = await _context.Municipalities.FindAsync(adminMunicipalityId);

            ViewBag.MunicipalityName = municipality?.Name ?? "Assigned Municipality";
            ViewBag.CandidateCount = await _context.Candidates.CountAsync(c => c.MunicipalityId == adminMunicipalityId);
            ViewBag.PositionCount = await _context.Positions.CountAsync(p => p.MunicipalityId == adminMunicipalityId);
        }
        else if (isSuperAdmin)
        {
            ViewBag.MunicipalityCount = await _context.Municipalities.CountAsync();
            ViewBag.CandidateCount = await _context.Candidates.CountAsync();
            ViewBag.PositionCount = await _context.Positions.CountAsync();
        }

        return View();
    }
}