namespace Pawsome.Models
{
    public class RabiesIncident
    {
        public int RabiesIncidentId { get; set; }
        public string Description { get; set; }
        public int ReportedByUserId { get; set; }
        public User ReportedByUser { get; set; }  // Correctly defined navigation property
        public string City { get; set; }
        public string Barangay { get; set; }
        public string Type { get; set; }
        public DateTime DateReported { get; set; }
        public bool IsVerified { get; set; } 
        public int? VerifiedByAdminId { get; set; }
        public User VerifiedByAdmin { get; set; }  
        public DateTime? DateVerified { get; set; }
        public byte[]? Photo { get; set; }
    }

}
