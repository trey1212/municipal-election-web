using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Data;
using municipal_election_web.DTOs;

[Route("api/positions")]
[ApiController]
public class PositionsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PositionsApiController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPositions()
    {
        var positions = await _context.Positions
            .Select(position => new PositionDto
            {
                PositionId = position.PositionId,
                Name = position.Name ?? "",
                SeatsAvailable = position.SeatsAvailable,
                MunicipalityId = position.MunicipalityId
            })
            .ToListAsync();

        return Ok(positions);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPosition(int id)
    {
        var position = await _context.Positions
            .Where(position => position.PositionId == id)
            .Select(position => new PositionDto
            {
                PositionId = position.PositionId,
                Name = position.Name ?? "",
                SeatsAvailable = position.SeatsAvailable,
                MunicipalityId = position.MunicipalityId
            })
            .FirstOrDefaultAsync();

        if (position == null)
        {
            return NotFound();
        }

        return Ok(position);
    }
}