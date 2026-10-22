using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace municipal_election_web.Areas.Admin.Models
{
    public class CreateMunicipality
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Province { get; set; }

        [Required]
        public DateTime ElectionDate { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a logo to upload.")]
        public IFormFile LogoUpload { get; set; }
    }
}