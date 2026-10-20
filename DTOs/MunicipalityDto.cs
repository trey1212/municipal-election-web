using System;

namespace municipal_election_web.DTOs
{
    public class MunicipalityDto
    {
        public int MunicipalityId { get; set; }
        public string Name { get; set; }
        public string Province { get; set; }
        public DateTime ElectionDate { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
    }
}