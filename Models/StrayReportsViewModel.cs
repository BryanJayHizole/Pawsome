namespace Pawsome.Models
{
    public class StrayReportsViewModel
    {
        public IEnumerable<StrayReport> UserStrayReports { get; set; }
        public IEnumerable<StrayReport> AllStrayReports { get; set; }
        public IEnumerable<StrayReport> CapturedReports { get; set; } 
        public IEnumerable<StrayReport> EuthanizedReports { get; set; }
        public int EuthanasiaDays { get; set; }  // Add this property if it's missing.

    }
}
