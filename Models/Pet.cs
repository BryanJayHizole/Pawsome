using System.ComponentModel.DataAnnotations;

namespace Pawsome.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PetType { get; set; }
        public string Breed { get; set; }
        public int? Age { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerContact { get; set; }
        public string PetProvince { get; set; }
        public string PetCity { get; set; }
        public string PetBarangay { get; set; }
        public string Habitat { get; set; } // Dropdown options: Caged, Free Roaming, Leased, House Only
        public DateTime? DateOfBirth { get; set; } // Nullable DateTime
        public string Color { get; set; }
        public string Gender { get; set; }
        public string TagType { get; set; } // Dropdown options: Collar, Microchip, Tattoo Code
        public string? TagNumber { get; set; }
        public double? Weight { get; set; } // Nullable double
        public string VaccinationStatus { get; set; }
        public string? VaccineType { get; set; } // Dropdown options: Anti Rabies, Others
        public DateTime? VaccinationDate { get; set; } // Nullable DateTime
        public DateTime? NextDueDate { get; set; } // Nullable DateTime for next due date
        public string? VaccinatedBy { get; set; }
        public int? VaccineBlockNo { get; set; } // Nullable int
        public string? VaccineSource { get; set; } // Dropdown options: BAI, DARFO, PLGU, MLGU, DOH, NGO, OIE
        public byte[]? Photo { get; set; }  // New property to store the photo as a byte array
        public bool IsVerified { get; set; } // New property to track verification status
        public bool IsArchived { get; set; } // New property to track verification status
        public List<VaccinationHistory> VaccinationHistories { get; set; } = new List<VaccinationHistory>();
    }

    public class PetType
    {
        [Key]
        public int TypeId { get; set; }
        public string PType { get; set; }
    }

    public class Habitat
    {
        [Key]
        public int HabitatId { get; set; }
        public string HabitatType { get; set; }
    }

    public class TagType
    {
        [Key]
        public int TagId { get; set; }
        public string TagTypeName { get; set; }
    }

    public class VaccinationStatus
    {
        [Key]
        public int VStatusId { get; set; }
        public string VStatusType { get; set; }
    }

    public class VaccineType
    {
        [Key]
        public int VTypeId { get; set; }
        public string VType { get; set; }
    }

    public class VaccineSource
    {
        [Key]
        public int VSourceId { get; set; }
        public string VSource { get; set; }
    }
}
