using System.ComponentModel.DataAnnotations;

namespace Pawsome.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int UserId { get; set; }
        public string ClientName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Service { get; set; }

        public string PetName { get; set; }
        public int PetId { get; set; }
        public string Description { get; set; }

        public string Status { get; set; }
        public string? AppointmentType { get; set; }
        public User User { get; set; }

        public string PetType { get; set; }
        public string Breed { get; set; }
        public int? Age { get; set; }      
        public DateTime? DateOfBirth { get; set; } // Nullable DateTime
        public string Gender { get; set; }
        public double? Weight { get; set; } // Nullable double
        public string VaccinationStatus { get; set; }
        public string? VaccineType { get; set; } // Dropdown options: Anti Rabies, Others
        public DateTime? VaccinationDate { get; set; } // Nullable DateTime
        public DateTime? NextDueDate { get; set; } // Nullable DateTime for next due date
        public string? VaccinatedBy { get; set; }
        public int? VaccineBlockNo { get; set; } // Nullable int
        public string? VaccineSource { get; set; } // Dropdown options: BAI, DARFO, PLGU, MLGU, DOH, NGO, OIE
        public byte[]? PetPhoto { get; set; }  // New property to store the photo as a byte array




    }

    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string GenderAvailability { get; set; } // "Male", "Female", "Both"
        public ICollection<ServiceInventoryItem> ServiceInventoryItems { get; set; }
        public ICollection<ServicePetType> ServicePetTypes { get; set; }
        public ICollection<AvailableDateService> AvailableDateServices { get; set; }
    }

    public class ServicePetType
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int PetTypeId { get; set; }
        public PetType PetType { get; set; }
    }


    // Linking Model between Service and InventoryItem
    public class ServiceInventoryItem
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }

        public int QuantityUsed { get; set; }
    }

    public class AvailableDate
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ICollection<AvailableDateService> AvailableDateServices { get; set; }
    }

    public class AvailableDateService
    {
        public int Id { get; set; }
        public int AvailableDateId { get; set; }
        public AvailableDate AvailableDate { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }


    public class AppointmentType
    {
        [Key]
        public int AppointTypeId { get; set; }
        public string AppointType { get; set; }
    }


}
