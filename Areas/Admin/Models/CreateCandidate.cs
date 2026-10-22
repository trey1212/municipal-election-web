using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace municipal_election_web.Areas.Admin.Models
{
    public class CreateCandidate
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Biography { get; set; }

        [Required]
        public int PositionId { get; set; }

        [Required]
        public int MunicipalityId { get; set; }

        [Required(ErrorMessage = "Please select a photo.")]
        public IFormFile PhotoUpload { get; set; }
    }
}