using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Make sure to include this for DbContext
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Needed for async methods
using Pawsome.Models;
using Pawsome.Data;

namespace Pawsome.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context; // Assuming you have a DbContext called ApplicationDbContext

        // Constructor to inject the DbContext
        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action method to get notifications
        public async Task<IActionResult> GetNotifications()
        {
            var userId = HttpContext.Session.GetString("UserId"); // or however you store user info
            var notifications = await GetNotificationsForUserAsync(userId); // Fetch notifications asynchronously

            // Mark notifications as read
            if (notifications.Any())
            {
                foreach (var notification in notifications)
                {
                    notification.IsRead = true; // Set IsRead to true
                }

                await _context.SaveChangesAsync(); // Save changes to the database
            }

            return Json(notifications);
        }

        // Asynchronous method to fetch notifications from the database
        private async Task<List<NotificationModel>> GetNotificationsForUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<NotificationModel>(); // Return empty if userId is null
            }

            // Convert userId to int
            if (int.TryParse(userId, out int userIdInt))
            {
                // Fetch notifications for the specified user from the database
                return await _context.Notifications
                    .Where(n => n.UserId == userIdInt) // Now both are of type int
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
            }

            return new List<NotificationModel>(); 
        }
    }
}
