using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace municipal_election_web.Models;

public class Municipality
{
    public int MunicipalityId { get; set; }


    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string? Name { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string? Province { get; set; }
    public DateTime ElectionDate { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }

    public string? LogoUrl { get; set; }

    // Navigation properties
    public List<Position> Positions { get; set; } = new List<Position>();
    public List<Candidate> Candidates { get; set; } = new List<Candidate>();
}