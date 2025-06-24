namespace Pawsome.Models
{
    public class VaccinationStatusPvet
    {
        public int Id { get; set; }
        public string Municipality { get; set; }
        public string Barangay { get; set; }
        public int DogsCount { get; set; } // Municipality dogs count    
        public int CatsCount { get; set; }
        public int VaccinatedDogsCount { get; set; } 
        public int VaccinatedCatsCount { get; set; } 
        public double VaccinatedDogsPercentage { get; set; }  
        public double VaccinatedCatsPercentage { get; set; }
        public int CapturedDogsCount { get; set; } 
        public int CapturedCatsCount { get; set; } 
        public int EuthanizedDogsCount { get; set; } 
        public int EuthanizedCatsCount { get; set; } 
        public int VerifiedRabiesIncidentsCount { get; set; } 
    }

    public class RabiesRiskViewModel
    {
        public string Barangay { get; set; }
        public string Municipality { get; set; }
        public int BarangayDogsCount { get; set; }
        public int BarangayCatsCount { get; set; }
        public int BarangayVaccinatedDogsCount { get; set; }
        public int BarangayVaccinatedCatsCount { get; set; }
        public double BarangayVaccinatedDogsPercentage { get; set; }
        public double BarangayVaccinatedCatsPercentage { get; set; }
        public int BarangayVerifiedRabiesIncidentsCount { get; set; }
        public string RiskLevel { get; set; }
        public DateTime DateReported { get; set; }

    }

    public class ReportsViewModel
    {
        public List<VaccinationStatusPvet> PetCounts { get; set; }
        public List<RabiesRiskViewModel> RabiesRisk { get; set; }
    }

}
