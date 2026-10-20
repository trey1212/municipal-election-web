namespace municipal_election_web.DTOs
{
    public class SavedCandidateDetailDto
    {
        public int CandidateId { get; set; }
        public string Name { get; set; }
        public string PositionName { get; set; }
        public string MunicipalityName { get; set; }
        public string PhotoUrl { get; set; }
    }
}