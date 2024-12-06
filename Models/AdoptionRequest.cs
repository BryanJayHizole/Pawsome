namespace Pawsome.Models
{
    public class AdoptionRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StrayReportId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Declined
        public string? AdminRemarks { get; set; }

        public User User { get; set; }
        public StrayReport StrayReport { get; set; }
    }
}
