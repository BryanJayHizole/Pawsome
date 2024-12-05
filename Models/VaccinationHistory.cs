namespace Pawsome.Models
{
    public class VaccinationHistory
    {
        public int Id { get; set; }
        public int? PetId { get; set; } // Added PetName property
        public string? VaccinationStatus { get; set; }
        public string? VaccineType { get; set; }
        public string? VaccineSource { get; set; }
        public int? VaccineBlockNo { get; set; } 
        public DateTime? VaccinationDate { get; set; }
        public string? AdministeredBy { get; set; }
        public DateTime? NextDueDate { get; set; }
    }

   
}
