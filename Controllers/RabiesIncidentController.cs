using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pawsome.Data;
using Pawsome.Models;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Pawsome.Controllers
{
    public class RabiesIncidentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RabiesIncidentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Regular User: Report a rabies incident
        [HttpGet]
        public IActionResult ReportIncident()
        {
            ViewBag.Cities = _context.Cities.Select(c => new SelectListItem { Value = c.CityId.ToString(), Text = c.CityName }).ToList();
            ViewBag.Barangays = _context.Barangays.Select(c => new SelectListItem { Value = c.BarangayId.ToString(), Text = c.BarangayName }).ToList();

            return View(new RabiesIncident());
        }

        [HttpPost]
        public async Task<IActionResult> ReportIncident(RabiesIncident incident, IFormFile photoFile, Barangay barangy, City municipality)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null || user.IsBarangayAdmin)
            {
                return Unauthorized();
            }

            if(photoFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await photoFile.CopyToAsync(memoryStream);
                    incident.Photo = memoryStream.ToArray();
                }
            }

            // Fetch the names based on the selected IDs
            var city = _context.Cities.SingleOrDefault(c => c.CityId == municipality.CityId);
            var barangay = _context.Barangays.SingleOrDefault(b => b.BarangayId == barangy.BarangayId);

            // Set the names in the user object
            incident.City = city?.CityName;
            incident.Barangay = barangay?.BarangayName;

            incident.ReportedByUser = user;
            incident.ReportedByUserId = user.Id;
            incident.DateReported = DateTime.Now;
            incident.IsVerified = false;


            _context.RabiesIncidents.Add(incident);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyReportedIncidents");
        }

        [HttpGet]
        public JsonResult GetBarangaysByCity(int cityId)
        {
            var barangays = _context.Barangays
                .Where(b => b.CityId == cityId)
                .Select(b => new { b.BarangayId, b.BarangayName })
                .ToList();

            return Json(barangays);
        }


        [HttpGet]
        public async Task<IActionResult> VerifyIncidents(string selectedCity, string selectedBarangay)
        {
            var email = HttpContext.Session.GetString("Email");
            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin") == "True";

            if (!isPvetAdmin)
            {
                return Unauthorized();
            }

            var incidents = _context.RabiesIncidents.Include(r => r.ReportedByUser).AsQueryable();

            // Apply filters based on selected city and barangay
            if (!string.IsNullOrEmpty(selectedCity))
            {
                incidents = incidents.Where(r => r.City == selectedCity);
            }

            if (!string.IsNullOrEmpty(selectedBarangay))
            {
                incidents = incidents.Where(r => r.Barangay == selectedBarangay);
            }

            ViewBag.Cities = await _context.Cities.ToListAsync();
            ViewBag.Barangays = await _context.Barangays.ToListAsync();

            var viewModel = new RabiesIncidentsViewModel
            {
                RabiesIncidents = await incidents.ToListAsync(),
                Cities = ViewBag.Cities,
                Barangays = ViewBag.Barangays
            };



            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> VerifyIncident(int id)
        {
            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");
            if (isPvetAdmin != "True")
            {
                return Unauthorized();
            }

            var incident = await _context.RabiesIncidents.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }

            incident.IsVerified = true;
            incident.VerifiedByAdminId = (await _context.Users.SingleOrDefaultAsync(u => u.Email == HttpContext.Session.GetString("Email"))).Id;
            incident.DateVerified = DateTime.Now;

            _context.RabiesIncidents.Update(incident);
            await _context.SaveChangesAsync();

            // Create a notification for the user
            var notification = new NotificationModel
            {
                UserId = incident.ReportedByUserId, // Assuming Pet has an OwnerId property linking to the User
                Message = $"Your Rabies incident report at '{incident.Barangay}' has been verified!",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification); // Add the notification to the context
            _context.SaveChanges(); // Save changes to the database


            return RedirectToAction("VerifyIncidents");
        }

        // Regular User: View my reported incidents
        [HttpGet]
        public async Task<IActionResult> MyReportedIncidents()
        {
            var email = HttpContext.Session.GetString("Email");
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Unauthorized();
            }

            var incidents = await _context.RabiesIncidents
                .Where(i => i.ReportedByUserId == user.Id)
                .ToListAsync();

            return View(incidents);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteIncident(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Unauthorized();
            }

            var incident = await _context.RabiesIncidents
                .SingleOrDefaultAsync(i => i.RabiesIncidentId == id && i.ReportedByUserId == user.Id);

            if (incident == null)
            {
                return NotFound();
            }

            _context.RabiesIncidents.Remove(incident);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyReportedIncidents");
        }

    }
}
