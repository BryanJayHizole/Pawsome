namespace Pawsome.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public int UserId { get; set; } // To link the notification to the user
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; } // To indicate if the notification has been read
    }


}
