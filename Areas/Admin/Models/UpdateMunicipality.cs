using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace municipal_election_web.Areas.Admin.Models
{
    public class UpdateMunicipality
    {
        public int MunicipalityId { get; set; }

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

        // Tracks the existing image path to display on the HTML form
        public string? ExistingLogoUrl { get; set; }

        // Nullable because the admin might just want to change text, not the logo
        public IFormFile? NewLogoUpload { get; set; }
    }
}