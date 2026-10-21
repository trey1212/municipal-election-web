using System.ComponentModel.DataAnnotations;

namespace municipal_election_web.Models;

public class Candidate
{
    public int CandidateId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string? Name { get; set; }

    [MaxLength(300)]
    public string? Biography { get; set; }

    public string? PhotoUrl { get; set; }

    [Url]
    public string? Website { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    // Forgein keys
    public int PositionId { get; set; }
    public Position? Position { get; set; }
    public int MunicipalityId { get; set; }
    public Municipality? Municipality { get; set; }
}