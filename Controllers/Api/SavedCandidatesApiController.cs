using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Data;
using municipal_election_web.DTOs;
using System.Security.Claims;
using municipal_election_web.Models;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/savedcandidates")]
[ApiController]
public class SavedCandidatesApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SavedCandidatesApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetSavedCandidates()
    {
        // Extracts the UserId string from the incoming JWT token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var saved = await _context.SavedCandidates
            .Include(sc => sc.Candidate).ThenInclude(c => c.Position)
            .Include(sc => sc.Candidate).ThenInclude(c => c.Municipality)
            .Where(sc => sc.UserId == userId)
            .Select(sc => new SavedCandidateDto
            {
                SavedCandidateId = sc.SavedCandidateId,
                DateSaved = sc.DateSaved,
                Candidate = new SavedCandidateDetailDto
                {
                    CandidateId = sc.Candidate.CandidateId,
                    Name = sc.Candidate.Name,
                    PositionName = sc.Candidate.Position.Name,
                    MunicipalityName = sc.Candidate.Municipality.Name,
                    PhotoUrl = sc.Candidate.PhotoUrl
                }
            })
            .ToListAsync();

        return Ok(saved);
    }

    [HttpPost]
    public async Task<IActionResult> SaveCandidate([FromBody] SaveCandidateRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Prevent users from saving the exact same candidate twice
        var exists = await _context.SavedCandidates
            .AnyAsync(sc => sc.UserId == userId && sc.CandidateId == request.CandidateId);
        
        if (exists) return BadRequest("Candidate is already saved.");

        var savedCandidate = new SavedCandidate
        {
            UserId = userId,
            CandidateId = request.CandidateId,
            DateSaved = DateTime.UtcNow
        };

        _context.SavedCandidates.Add(savedCandidate);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{candidateId}")]
    public async Task<IActionResult> RemoveSavedCandidate(int candidateId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var savedCandidate = await _context.SavedCandidates
            .FirstOrDefaultAsync(sc => sc.UserId == userId && sc.CandidateId == candidateId);

        if (savedCandidate == null) return NotFound();

        _context.SavedCandidates.Remove(savedCandidate);
        await _context.SaveChangesAsync();

        return Ok();
    }
}