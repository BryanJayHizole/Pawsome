namespace Pawsome.Models
{
    public class SystemSetting
    {
        public int Id { get; set; }
        public int CapturedLimit { get; set; }  // This will store the captured limit
        public string BarangayName { get; set; }  // New property to link the setting to a Barangay
        public Barangay Barangay { get; set; }
        public int EuthanasiaDays { get; set; }

    }
}
