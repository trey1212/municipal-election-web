namespace municipal_election_web.DTOs;

public class PositionDto
{
    public int PositionId { get; set; }

    public string Name { get; set; } = "";

    public int SeatsAvailable { get; set; }

    public int MunicipalityId { get; set; }
}