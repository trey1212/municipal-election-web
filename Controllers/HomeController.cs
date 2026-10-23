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

        return View("MunicipalCandidates", municipality);
    }

    public async Task<IActionResult> CandidateDetails(int id)
    {
        var candidate = await _context.Candidates
            .Include(c => c.Position)
            .Include(c => c.Municipality)
            .FirstOrDefaultAsync(c => c.CandidateId == id);

        if (candidate == null) return NotFound();

        // Check if this candidate is already in the session list
        var savedList = HttpContext.Session.GetString("SavedCandidates");
        var candidateIds = string.IsNullOrEmpty(savedList) 
            ? new List<int>() 
            : savedList.Split(',').Select(int.Parse).ToList();
        
        ViewBag.IsSaved = candidateIds.Contains(id);

        return View(candidate);
    }

    [HttpPost]
    public IActionResult ToggleSavedCandidate(int candidateId)
    {
        var savedList = HttpContext.Session.GetString("SavedCandidates");
        var candidateIds = string.IsNullOrEmpty(savedList) 
            ? new List<int>() 
            : savedList.Split(',').Select(int.Parse).ToList();

        if (candidateIds.Contains(candidateId))
            candidateIds.Remove(candidateId); // Unsave
        else
            candidateIds.Add(candidateId); // Save

        HttpContext.Session.SetString("SavedCandidates", string.Join(",", candidateIds));
        return Json(new { success = true, isSaved = candidateIds.Contains(candidateId) });
    }

    public async Task<IActionResult> BallotBuilder()
    {
        var savedList = HttpContext.Session.GetString("SavedCandidates");
        if (string.IsNullOrEmpty(savedList)) return View(new List<Candidate>());

        var candidateIds = savedList.Split(',').Select(int.Parse).ToList();
        
        var candidates = await _context.Candidates
            .Include(c => c.Position)
            .Include(c => c.Municipality)
            .Where(c => candidateIds.Contains(c.CandidateId))
            .ToListAsync();

        return View(candidates);
    }

    [HttpGet]
    public async Task<IActionResult> GetCartPartial()
    {
        var savedList = HttpContext.Session.GetString("SavedCandidates");
        if (string.IsNullOrEmpty(savedList)) return PartialView("_BallotBuilderCart", new List<Candidate>());

        var candidateIds = savedList.Split(',').Select(int.Parse).ToList();
        
        var candidates = await _context.Candidates
            .Include(c => c.Position)
            .Where(c => candidateIds.Contains(c.CandidateId))
            .ToListAsync();

        return PartialView("_BallotBuilderCart", candidates);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
