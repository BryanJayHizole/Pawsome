namespace Pawsome.Models
{
    public class PenaltyAssignment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int PenaltyFineId { get; set; }
        public PenaltyFine PenaltyFine { get; set; }
        public DateTime AssignedDate { get; set; }
        public PenaltyStatus Status { get; set; }
        public string? Description { get; set; } // Add this property
    }

    public enum PenaltyStatus
    {
        Unpaid,
        Paid,
        Resolved
    }
}
