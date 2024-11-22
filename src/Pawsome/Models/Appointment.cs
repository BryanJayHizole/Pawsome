using System.ComponentModel.DataAnnotations;

namespace Pawsome.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ClientName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Service { get; set; }
        public string Description { get; set; }

        public string Status { get; set; }
        public string? AppointmentType { get; set; }
        public User User { get; set; }
    }

    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
    }

    public class AvailableDate
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }

    public class AppointmentType
    {
        [Key]
        public int AppointTypeId { get; set; }
        public string AppointType { get; set; }
    }

}
