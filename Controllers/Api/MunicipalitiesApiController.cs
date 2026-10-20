using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using municipal_election_web.Data;
using municipal_election_web.DTOs;

[Route("api/municipalities")]
[ApiController]
public class MunicipalitiesApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MunicipalitiesApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMunicipalities()
    {
        var municipalities = await _context.Municipalities
            .Select(m => new MunicipalityDto
            {
                MunicipalityId = m.MunicipalityId,
                Name = m.Name,
                Province = m.Province,
                ElectionDate = m.ElectionDate,
                Description = m.Description,
                LogoUrl = m.LogoUrl
            })
            .ToListAsync();

        return Ok(municipalities);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMunicipality(int id)
    {
        var m = await _context.Municipalities.FindAsync(id);
        if (m == null) return NotFound();

        return Ok(new MunicipalityDto
        {
            MunicipalityId = m.MunicipalityId,
            Name = m.Name,
            Province = m.Province,
            ElectionDate = m.ElectionDate,
            Description = m.Description,
            LogoUrl = m.LogoUrl
        });
    }
}