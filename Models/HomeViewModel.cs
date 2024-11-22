namespace Pawsome.Models
{
    public class HomeViewModel
    {
        public User User { get; set; }
        public IEnumerable<Announcement> Announcements { get; set; }
       
    }
}
