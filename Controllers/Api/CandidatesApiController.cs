using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Data;
using municipal_election_web.DTOs;

[Route("api")]
[ApiController]
public class CandidatesApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CandidatesApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Gets all candidates for a specific municipality
    [HttpGet("municipalities/{municipalityId}/candidates")]
    public async Task<IActionResult> GetCandidates(int municipalityId, [FromQuery] string? search)
    {
        var query = _context.Candidates
            .Include(c => c.Position)
            .Where(c => c.MunicipalityId == municipalityId);

        // If the iOS app sends a search query, filter the database
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search) || c.Position.Name.Contains(search));
        }

        var candidates = await query
            .Select(c => new CandidateDto
            {
                CandidateId = c.CandidateId,
                Name = c.Name,
                Biography = c.Biography,
                PhotoUrl = c.PhotoUrl,
                Website = c.Website,
                Email = c.Email,
                PositionName = c.Position.Name
            })
            .ToListAsync();

        return Ok(candidates);
    }

    // Gets a single candidate by their ID
    [HttpGet("candidates/{id}")]
    public async Task<IActionResult> GetCandidate(int id)
    {
        var c = await _context.Candidates
            .Include(c => c.Position)
            .FirstOrDefaultAsync(c => c.CandidateId == id);

        if (c == null) return NotFound();

        return Ok(new CandidateDto
        {
            CandidateId = c.CandidateId,
            Name = c.Name,
            Biography = c.Biography,
            PhotoUrl = c.PhotoUrl,
            Website = c.Website,
            Email = c.Email,
            PositionName = c.Position.Name
        });
    }
}