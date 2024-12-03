using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pawsome.Data;
using Pawsome.Migrations;
using Pawsome.Models;
using System.Drawing;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pawsome.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display calendar with appointments and announcements
        [HttpGet]
        public async Task<IActionResult> Calendar()
        {
            var services = _context.Services.Select(c => new SelectListItem { Value = c.ServiceId.ToString(), Text = c.ServiceName }).ToList();
            ViewBag.Services = services;

            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin") == "True";
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Fetch all appointments for display
            var appointments = await _context.Appointments.Include(a => a.User).ToListAsync();

            return View(appointments);
        }

        // Action for PvetAdmin to edit available dates
        [HttpPost]
        public IActionResult EditCalendar(string SelectedDates)
        {
            // Remove all previous available dates
            _context.AvailableDates.RemoveRange(_context.AvailableDates);

            // Add the new available dates
            if (!string.IsNullOrEmpty(SelectedDates))
            {
                var datesArray = SelectedDates.Split(',');
                foreach (var date in datesArray)
                {
                    if (DateTime.TryParse(date, out DateTime parsedDate))
                    {
                        _context.AvailableDates.Add(new AvailableDate { Date = parsedDate });
                    }
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Calendar");
        }




        public IActionResult GetAvailableDates()
        {
            var availableDates = _context.AvailableDates.Select(d => d.Date.ToString("yyyy-MM-dd")).ToList();
            return Json(availableDates);
        }


        // Regular User: Book an appointment
        [HttpGet]
        public async Task<IActionResult> BookAppointment(int id)
        {

            var services = _context.Services.Select(c => new SelectListItem { Value = c.ServiceId.ToString(), Text = c.ServiceName }).ToList();
            ViewBag.Services = services;
            var appointmenttypes = _context.AppointmentTypes.Select(c => new SelectListItem { Value = c.AppointTypeId.ToString(), Text = c.AppointType }).ToList();
            ViewBag.AppointmentTypes = appointmenttypes;



            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }



            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }



            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment(Appointment appointment, Service servic, AppointmentType atyp)
        {

            var email = HttpContext.Session.GetString("Email");
            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin") == "True";
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (isPvetAdmin)
            {
                appointment.Status = "Approved"; // Automatically approve the appointment
            }
            else
            {
                appointment.Status = "Pending"; // Default to Pending for other users
            }

            var service = _context.Services.SingleOrDefault(c => c.ServiceId == servic.ServiceId);
            appointment.Service = service?.ServiceName;
            var appointmenttype = _context.AppointmentTypes.SingleOrDefault(c => c.AppointTypeId == atyp.AppointTypeId);
            appointment.AppointmentType = appointmenttype?.AppointType;

            // Check the number of existing appointments for the selected date
            var appointmentCount = await _context.Appointments
                .Where(a => a.Date.Date == appointment.Date.Date)
                .CountAsync();

            if (appointmentCount >= 10)
            {
                // If 10 or more appointments exist, return an error
                ModelState.AddModelError("", "The maximum number of appointments for this day has been reached.");
                return View(appointment);
            }

            if (isPvetAdmin)
            {
                // Logic to set ClientName if it's a PvetAdmin (for example, from the input)
                appointment.ClientName = appointment.ClientName; // Update this according to your input field for PvetAdmin
                // Logic to set ClientName if it's a PvetAdmin (for example, from the input)
                appointment.AppointmentType = appointment.AppointmentType; // Update this according to your input field for PvetAdmin
            }
            else
            {
                // Logic for regular user - the ClientName is set in the hidden field
                appointment.ClientName = HttpContext.Session.GetString("Firstname");
                // Ensure that the AppointmentType is set to "Online"
                appointment.AppointmentType = "Online";
            }

            appointment.UserId = user.Id;
            appointment.Status = "Pending"; // Initial status

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments");
        }



        [HttpGet]
        public async Task<IActionResult> GetAppointmentCount(DateTime date)
        {
            var count = await _context.Appointments
                .Where(a => a.Date.Date == date.Date)
                .CountAsync();

            return Json(count);
        }


        // PvetAdmin: View all pending appointments
        [HttpGet]
        public async Task<IActionResult> PendingAppointments()
        {
            var services = _context.Services.Select(c => new SelectListItem { Value = c.ServiceId.ToString(), Text = c.ServiceName }).ToList();
            ViewBag.Services = services;
            var appointmenttypes = _context.AppointmentTypes.Select(c => new SelectListItem { Value = c.AppointTypeId.ToString(), Text = c.AppointType }).ToList();
            ViewBag.AppointmentTypes = appointmenttypes;

            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");
            if (isPvetAdmin != "True")
            {
                return RedirectToAction("Index", "Home");
            }

            var pendingAppointments = await _context.Appointments
                .Where(a => a.Status == "Pending")
                .Include(a => a.User)
                .ToListAsync();

            return View(pendingAppointments);
        }

        // PvetAdmin: Approve an appointment
        [HttpPost]
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            var services = _context.Services.Select(c => new SelectListItem { Value = c.ServiceId.ToString(), Text = c.ServiceName }).ToList();
            ViewBag.Services = services;
            var appointmenttypes = _context.AppointmentTypes.Select(c => new SelectListItem { Value = c.AppointTypeId.ToString(), Text = c.AppointType }).ToList();
            ViewBag.AppointmentTypes = appointmenttypes;

            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");
            if (isPvetAdmin != "True")
            {
                return RedirectToAction("Index", "Home");
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            appointment.Status = "Approved";
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            // Create a notification for the user
            var notification = new NotificationModel
            {
                UserId = appointment.UserId, // Assuming Pet has an OwnerId property linking to the User
                Message = $"Your appoitment for '{appointment.Service}' has been Approved!",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification); // Add the notification to the context
            _context.SaveChanges(); // Save changes to the database

            return RedirectToAction("PendingAppointments");
        }

        [HttpPost]
        public IActionResult ApproveAllAppointments()
        {
            var pendingAppointments = _context.Appointments.Where(a => a.Status == "Pending").ToList();
            foreach (var appointment in pendingAppointments)
            {
                appointment.Status = "Approved";
            }

            _context.SaveChanges();
            return RedirectToAction("PendingAppointments");
        }

        [HttpPost]
        public IActionResult DeclineAllAppointments()
        {
            var pendingAppointments = _context.Appointments.Where(a => a.Status == "Pending").ToList();
            foreach (var appointment in pendingAppointments)
            {
                appointment.Status = "Declined";
            }

            _context.SaveChanges();
            return RedirectToAction("PendingAppointments");
        }

        [HttpPost]
        public IActionResult DeclineAppointment(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null)
            {
                // Decline the appointment
                appointment.Status = "Declined";  // Assuming "Declined" is a valid status
                _context.SaveChanges();
            }

            return RedirectToAction("PendingAppointments");
        }


        // PvetAdmin: View all approved appointments
        [HttpGet]
        public async Task<IActionResult> ApprovedAppointments()
        {
            var services = _context.Services.Select(c => new SelectListItem { Value = c.ServiceId.ToString(), Text = c.ServiceName }).ToList();
            ViewBag.Services = services;
            var appointmenttypes = _context.AppointmentTypes.Select(c => new SelectListItem { Value = c.AppointTypeId.ToString(), Text = c.AppointType }).ToList();
            ViewBag.AppointmentTypes = appointmenttypes;

            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");
            if (isPvetAdmin != "True")
            {
                return RedirectToAction("Index", "Home");
            }

            var approvedAppointments = await _context.Appointments
                .Where(a => a.Status == "Approved")
                .Include(a => a.User)
                .ToListAsync();

            return View(approvedAppointments);
        }


        // Regular User: View my appointments
        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var appointments = await _context.Appointments
            
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

            return View(appointments);
        }

        // Regular User: Delete a pending appointment
        [HttpGet]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null || user.IsPvetAdmin)
            {
                return Unauthorized();
            }

            var appointment = await _context.Appointments
                .SingleOrDefaultAsync(a => a.Id == id && a.UserId == user.Id && a.Status == "Pending");

            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments");
        }

        // Regular User or PvetAdmin: Mark an approved appointment as done
        [HttpGet]
        public async Task<IActionResult> MarkAsDone(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null || (!user.IsPvetAdmin && !await _context.Appointments.AnyAsync(a => a.Id == id && a.UserId == user.Id)))
            {
                return Unauthorized();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null || appointment.Status != "Approved")
            {
                return NotFound();
            }

            appointment.Status = "Done";
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments");
        }



    }
}
