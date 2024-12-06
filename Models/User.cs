using System.ComponentModel.DataAnnotations;

namespace Pawsome.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string LastName { get; set; }
       
        public string Password { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public DateTime? Birthday { get; set; }  
        public int Age { get; set; }              
        public string Country { get; set; }       
        public string Province { get; set; }    
        public string City { get; set; }        
        public string Barangay { get; set; }     
        public byte[]? ProfilePhoto { get; set; }
        public bool IsBarangayAdmin { get; set; } 
        public bool IsPvetAdmin { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public string? VerificationToken { get; set; }

        // Navigation property for AdoptionRequests
        public ICollection<AdoptionRequest> AdoptionRequests { get; set; }
        public ICollection<PenaltyAssignment> PenaltyAssignments { get; set; } = new List<PenaltyAssignment>();
    }
}
