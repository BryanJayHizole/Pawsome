namespace Pawsome.Models
{
    public class VaccinationStatusPvet
    {
        public int Id { get; set; }
        public string Municipality { get; set; }
        public int DogsCount { get; set; }
        public int CatsCount { get; set; }
        public int VaccinatedDogsCount { get; set; } // New property for vaccinated dogs
        public int VaccinatedCatsCount { get; set; } // New property for vaccinated cats
        public double VaccinatedDogsPercentage { get; set; } // New property for percentage of vaccinated dogs
        public double VaccinatedCatsPercentage { get; set; }
        public int CapturedDogsCount { get; set; } // New property for captured dogs count
        public int CapturedCatsCount { get; set; } // New property for captured cats count
        public int EuthanizedDogsCount { get; set; } // New property for euthanized dogs count
        public int EuthanizedCatsCount { get; set; } // New property for euthanized cats count
        public int VerifiedRabiesIncidentsCount { get; set; } // New property for euthanized cats count

    }
}
