using System;

namespace municipal_election_web.DTOs
{
    public class SavedCandidateDto
    {
        public int SavedCandidateId { get; set; }
        public DateTime DateSaved { get; set; }
        public SavedCandidateDetailDto Candidate { get; set; }
    }
}