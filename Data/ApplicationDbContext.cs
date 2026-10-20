using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace municipal_election_web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<SavedCandidate> SavedCandidates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Municipality>().HasData(
            new Municipality
            {
                MunicipalityId = 1, 
                Name = "Surrey", 
                Province = "BC", 
                ElectionDate = new DateTime(2026, 10, 17), 
                Description = "The future lives here", 
                LogoUrl = "/images/surrey-logo.png"
            },
            new Municipality 
            { 
                MunicipalityId = 2, 
                Name = "Langley", 
                Province = "BC", 
                ElectionDate = new DateTime(2026, 10, 17), 
                Description = "City of Langley", 
                LogoUrl = "/images/langley-logo.png" 
            }
        );

        modelBuilder.Entity<Position>().HasData(
            // Surrey positions
            new Position { PositionId = 1, Name = "Mayor", SeatsAvailable = 1, MunicipalityId = 1 },
            new Position { PositionId = 2, Name = "Councilor", SeatsAvailable = 8, MunicipalityId = 1 },
            new Position { PositionId = 3, Name = "Trustee", SeatsAvailable = 6, MunicipalityId = 1 },
            //Langley positions
            new Position { PositionId = 4, Name = "Mayor", SeatsAvailable = 1, MunicipalityId = 2 },
            new Position { PositionId = 5, Name = "Councilor", SeatsAvailable = 8, MunicipalityId = 2 },
            new Position { PositionId = 6, Name = "Trustee", SeatsAvailable = 5, MunicipalityId = 2 }
        );

        modelBuilder.Entity<Candidate>().HasData(
            // SURREY CANDIDATES
            // Surrey Mayors
            new Candidate
            {
                CandidateId = 1,
                Name = "Marcus Vance",
                Biography = "Former city planner focused on sustainable transit expansion, community safety, and local business growth.",
                PhotoUrl = "/images/candidates/marcus-vance.jpg",
                Website = "https://marcusvanceforsurrey.ca",
                Email = "campaign@marcusvance.ca",
                PositionId = 1,
                MunicipalityId = 1
            },
            new Candidate
            {
                CandidateId = 2,
                Name = "Amrit Gill",
                Biography = "Small business owner and civic activist championing affordable housing initiatives and municipal tax relief.",
                PhotoUrl = "/images/candidates/amrit-gill.jpg",
                Website = "https://gillforsurrey.com",
                Email = "info@gillforsurrey.com",
                PositionId = 1,
                MunicipalityId = 1
            },

            // Surrey Councilors
            new Candidate
            {
                CandidateId = 3,
                Name = "Linda Nguyen",
                Biography = "Advocating for green urban spaces, improved cycling networks, and revitalization of community recreational centres.",
                PhotoUrl = "/images/candidates/linda-nguyen.jpg",
                Website = "https://lindanguyen.ca",
                Email = "linda@nguyenforward.ca",
                PositionId = 2,
                MunicipalityId = 1
            },
            new Candidate
            {
                CandidateId = 4,
                Name = "David Chen",
                Biography = "Focusing on transparent city budgeting, neighborhood watch initiatives, and infrastructure upgrades.",
                PhotoUrl = "/images/candidates/david-chen.jpg",
                Website = "https://davidchensurrey.org",
                Email = "david@chensurrey.org",
                PositionId = 2,
                MunicipalityId = 1
            },

            // Surrey School Trustees
            new Candidate
            {
                CandidateId = 5,
                Name = "Priya Sharma",
                Biography = "Parent and STEM educator passionate about reducing classroom sizes and increasing funding for special education.",
                PhotoUrl = "/images/candidates/priya-sharma.jpg",
                Website = "https://sharmafortrustee.ca",
                Email = "contact@priyasharma.ca",
                PositionId = 3,
                MunicipalityId = 1
            },
            new Candidate
            {
                CandidateId = 6,
                Name = "Robert O'Connor",
                Biography = "Retired high school principal dedicated to expanding modern library resources and mental health supports for students.",
                PhotoUrl = "/images/candidates/robert-oconnor.jpg",
                Website = "https://oconnorforstudents.com",
                Email = "robert@oconnortrustee.ca",
                PositionId = 3,
                MunicipalityId = 1
            },

            // LANGLEY CANDIDATES
            // Langley Mayors
            new Candidate
            {
                CandidateId = 7,
                Name = "Thomas Wright",
                Biography = "Dedicated to sensible downtown redevelopment, walkable corridors, and strengthening emergency response services.",
                PhotoUrl = "/images/candidates/thomas-wright.jpg",
                Website = "https://wrightforlangley.ca",
                Email = "thomas@wrightforlangley.ca",
                PositionId = 4,
                MunicipalityId = 2
            },
            new Candidate
            {
                CandidateId = 8,
                Name = "Sarah Jenkins",
                Biography = "Long-time community volunteer prioritizing local heritage preservation and sensible fiscal management.",
                PhotoUrl = "/images/candidates/sarah-jenkins.jpg",
                Website = "https://sarahjenkins.ca",
                Email = "hello@sarahjenkins.ca",
                PositionId = 4,
                MunicipalityId = 2
            },

            // Langley Councilors
            new Candidate
            {
                CandidateId = 9,
                Name = "Michael Chang",
                Biography = "Civil engineer working toward safer roadways, traffic calming in residential areas, and better flood management.",
                PhotoUrl = "/images/candidates/michael-chang.jpg",
                Website = "https://changforlangley.org",
                Email = "michael@changforlangley.org",
                PositionId = 5,
                MunicipalityId = 2
            },
            new Candidate
            {
                CandidateId = 10,
                Name = "Claire Tremblay",
                Biography = "Advocate for park improvements, community gardens, and fostering supportive environments for local small businesses.",
                PhotoUrl = "/images/candidates/claire-tremblay.jpg",
                Website = "https://clairetremblay.com",
                Email = "claire@tremblaycouncil.com",
                PositionId = 5,
                MunicipalityId = 2
            },

            // Langley School Trustees
            new Candidate
            {
                CandidateId = 11,
                Name = "Harpreet Dhillon",
                Biography = "Youth mentor and volunteer focusing on upgrading school technology, digital literacy, and classroom safety.",
                PhotoUrl = "/images/candidates/harpreet-dhillon.jpg",
                Website = "https://dhillonfortrustee.ca",
                Email = "harpreet@dhillontrustee.ca",
                PositionId = 6,
                MunicipalityId = 2
            },
            new Candidate
            {
                CandidateId = 12,
                Name = "Emily Watson",
                Biography = "Former PAC chair working to secure provincial grants for arts programs and after-school childcare options.",
                PhotoUrl = "/images/candidates/emily-watson.jpg",
                Website = "https://watsonforlangleykids.com",
                Email = "emily@watsonfortrustee.ca",
                PositionId = 6,
                MunicipalityId = 2
            }
        );
    }
}
