using System.Collections.Generic;

namespace Pawsome.Models
{
    public class LostPetReportsViewModel
    {
        public IEnumerable<LostPetReport> UserLostPetReports { get; set; }
        public IEnumerable<LostPetReport> AllLostPetReports { get; set; }
        public IEnumerable<LostPetReport> FoundPets { get; set; }
        public List<string> Municipalities { get; set; }  
        public List<string> Barangays { get; set; }

        public string CurrentUserBarangay { get; set; }
        public int CurrentUserPage { get; set; }
        public int TotalUserPages { get; set; }
        public int CurrentAllPage { get; set; }
        public int TotalAllPages { get; set; }
        public int CurrentFoundPage { get; set; }
        public int TotalFoundPages { get; set; }




    }
    
}
