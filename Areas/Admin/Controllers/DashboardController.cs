using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Areas.Admin.Models;
using municipal_election_web.Data;
using System.Security.Claims;

namespace municipal_election_web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,MunicipalityAdmin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<IdentityUser> _signInManager;

    public DashboardController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager)
    {
        _context = context;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            IsSuperAdmin = User.IsInRole("SuperAdmin"),
            IsMunicipalityAdmin = User.IsInRole("MunicipalityAdmin")
        };

        if (model.IsMunicipalityAdmin)
        {
            var adminMunicipalityId = int.Parse(User.FindFirstValue("MunicipalityId")!);
            var municipality = await _context.Municipalities.FindAsync(adminMunicipalityId);

            model.MunicipalityName = municipality?.Name ?? "Assigned Municipality";

            // Fetch only candidates for this specific city
            model.Candidates = await _context.Candidates
                .Include(c => c.Position)
                .Where(c => c.MunicipalityId == adminMunicipalityId)
                .ToListAsync();
        }
        else if (model.IsSuperAdmin)
        {
            // Super admins get everything
            model.Municipalities = await _context.Municipalities.ToListAsync();
            model.Candidates = await _context.Candidates
                .Include(c => c.Position)
                .Include(c => c.Municipality)
                .ToListAsync();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home", new { area = "" }); // Sends them back to the public homepage
    }
}