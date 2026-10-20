namespace municipal_election_web.DTOs
{
    public class CandidateDto
    {
        public int CandidateId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public string PhotoUrl { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string PositionName { get; set; }
    }
}