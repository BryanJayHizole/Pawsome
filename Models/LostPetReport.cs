namespace Pawsome.Models
{
    public class LostPetReport
    {
        public int Id { get; set; }
        public int? PetId { get; set; }
        public Pet Pet { get; set; }
        public string PetName { get; set; }
        public string PetGender { get; set; }
        public string PetColor { get; set; }
        public string PetType { get; set; }
        public string PetBreed { get; set; }
        public string PetTag { get; set; }
        public string? PetTagNo { get; set; }
        public byte[]? Photo { get; set; }  // New property to store the photo as a byte array
        public string LastSeenLocation { get; set; }
        public int? ReporterId { get; set; }
        public string ReporterBarangay { get; set; }
        public string City { get; set; }
        public string Barangay { get; set; }

        public string? Purok { get; set; }

        public DateTime DateLost { get; set; }
        public int? OwnerId { get; set; }
        public string OwnerName { get; set; } // New property to store the owner's username
        public string OwnerContact { get; set; } // New property to store the owner's contact information
        public string? FinderName { get; set; } // New property to store the finder’s name
        public string? FoundLocation { get; set; } // New property to store where the pet was found
        public string AdditionalInfo { get; set; }
        public bool IsFound { get; set; }
        public bool IsOwnPet { get; set; } // New property

        public DateTime? DateReported { get; set; } // Optional, to track when the report was created
        public DateTime? DateFound { get; set; } // Optional, to track when the report was found
    }

}