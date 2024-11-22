namespace Pawsome.Models
{
    public class StrayReport
    {
        public int Id { get; set; }
        public int ReporterId { get; set; }
        public string ReporterName { get; set; }
        public string City { get; set; }
        public string Barangay { get; set; }
        public DateTime DateReported { get; set; }
        public string StrayType { get; set; }
        public string StrayBreed { get; set; }
        public string StrayGender { get; set; }
        public string StrayColor { get; set; }
        public string? StrayTag { get; set; }
        public string? StrayTagNo { get; set; }
        public string Description { get; set; }
        public byte[]? Photo { get; set; }  // Optional photo of the stray animal
        public string Status { get; set; } = "Pending";  // Status of the report
    }
}
