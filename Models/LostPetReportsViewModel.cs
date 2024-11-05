using System.Collections.Generic;

namespace Pawsome.Models
{
    public class LostPetReportsViewModel
    {
        public IEnumerable<LostPetReport> UserLostPetReports { get; set; }
        public IEnumerable<LostPetReport> AllLostPetReports { get; set; }
        public IEnumerable<LostPetReport> FoundPets { get; set; }

    }
    
}
