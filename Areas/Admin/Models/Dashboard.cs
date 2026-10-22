using municipal_election_web.Models;
using System.Collections.Generic;

namespace municipal_election_web.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public bool IsSuperAdmin { get; set; }
        public bool IsMunicipalityAdmin { get; set; }
        public string? MunicipalityName { get; set; }

        public List<Municipality> Municipalities { get; set; } = new List<Municipality>();
        public List<Candidate> Candidates { get; set; } = new List<Candidate>();
    }
}