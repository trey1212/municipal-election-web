using System;

public class SavedCandidate
{
    public int SavedCandidateId { get; set; }
    
    // Needs to remain non-nullable for IOS
    public string UserId { get; set; } // Links to the ASP.NET Identity User
    public int CandidateId { get; set; }
    public Candidate? Candidate { get; set; }
    public DateTime DateSaved { get; set; }
}