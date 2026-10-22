using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace municipal_election_web.Areas.Admin.Models
{
    public class UpdateCandidate
    {
        public int CandidateId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Biography { get; set; }

        [Required]
        public int PositionId { get; set; }

        [Required]
        public int MunicipalityId { get; set; }

        // Tracks the existing image path to display on the HTML form
        public string? ExistingPhotoUrl { get; set; }

        // Nullable so the existing photo is kept if the admin leaves this input blank
        public IFormFile? NewPhotoUpload { get; set; }
    }
}