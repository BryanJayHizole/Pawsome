namespace Pawsome.Models
{
    public class RabiesIncident
    {
        public int RabiesIncidentId { get; set; }
        public string Description { get; set; }
        public int ReportedByUserId { get; set; }
        public User ReportedByUser { get; set; }  
        public string Purok { get; set; }
        public string City { get; set; }
        public string Barangay { get; set; }
        public string? TypeofIncident { get; set; }
        public string? Symptoms { get; set; }
        public DateTime DateReported { get; set; }
        public bool IsVerified { get; set; }
        public bool IsVerifiedbyBarangayAdmin { get; set; }
        public bool IsStray { get; set; } // New property
        public int? VerifiedByAdminId { get; set; }
        public User VerifiedByAdmin { get; set; }  
        public DateTime? DateVerified { get; set; }
        public byte[]? Photo { get; set; }
        public string IncidentCase { get; set; }
        public string RabiesCase { get; set; }
        public int? PetId { get; set; }
        public Pet Pet { get; set; }
        public int? OwnerId { get; set; }
        public string? OwnerName { get; set; } // New property to store the owner's username
        public string? OwnerContact { get; set; } // New property to store the owner's contact information
        public string? PetName { get; set; }
        public string AnimalType { get; set; } // Add this property if it doesn't exist
        public string? Breed { get; set; }
        public string Gender { get; set; }
        public string Color { get; set; }
        public string? Tag { get; set; }
        public string? TagNo { get; set; }
        public string? VaccinationStatus { get; set; }
        public string? VaccineType { get; set; } // Dropdown options: Anti Rabies, Others
        public DateTime? VaccinationDate { get; set; } // Nullable DateTime
        public DateTime? NextDueDate { get; set; } // Nullable DateTime for next due date
        public string? VaccinatedBy { get; set; }
        public int? VaccineBlockNo { get; set; } // Nullable int
        public byte[]? PetPhoto { get; set; }  // Optional photo of the stray animal
    }

}
