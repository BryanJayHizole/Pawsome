using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Pawsome.Data;
using Pawsome.Models;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics.Metrics;
using System.Drawing;

namespace Pawsome.Controllers
{
    public class StrayReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StrayReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ReportStray()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Get the list of countries, provinces, cities, and barangays from the database
            ViewBag.Countries = _context.Countries.Select(c => new SelectListItem { Value = c.CountryId.ToString(), Text = c.CountryName }).ToList();
            ViewBag.Provinces = _context.Provinces.Select(c => new SelectListItem { Value = c.ProvinceId.ToString(), Text = c.ProvinceName }).ToList();
            ViewBag.Cities = _context.Cities.Select(c => new SelectListItem { Value = c.CityId.ToString(), Text = c.CityName }).ToList();
            ViewBag.Barangays = _context.Barangays.Select(c => new SelectListItem { Value = c.BarangayId.ToString(), Text = c.BarangayName }).ToList();

            var pettypes = _context.PetTypes.Select(c => new SelectListItem { Value = c.TypeId.ToString(), Text = c.PType }).ToList();
            ViewBag.PetTypes = pettypes;
            var tagtypes = _context.TagTypes.Select(c => new SelectListItem { Value = c.TagId.ToString(), Text = c.TagTypeName }).ToList();
            ViewBag.TagTypes = tagtypes;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReportStray(StrayReport report, IFormFile photoFile, Barangay barangy, City municipality, PetType pettyp, TagType tagtyp)
        {
            var pettype = _context.PetTypes.SingleOrDefault(c => c.TypeId == pettyp.TypeId);
            var tagtype = _context.TagTypes.SingleOrDefault(c => c.TagId == tagtyp.TagId);

            report.StrayType = pettype?.PType;
            report.StrayTag = tagtype?.TagTypeName;

            var email = HttpContext.Session.GetString("Email");
            var firstname = HttpContext.Session.GetString("Firstname");
            var user = _context.Users.SingleOrDefault(u => u.Email == email);
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (photoFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await photoFile.CopyToAsync(memoryStream);
                    report.Photo = memoryStream.ToArray();
                }
            }

            // Fetch the names based on the selected IDs
            var city = _context.Cities.SingleOrDefault(c => c.CityId == municipality.CityId);
            var barangay = _context.Barangays.SingleOrDefault(b => b.BarangayId == barangy.BarangayId);

            // Set the names in the user object
            report.City = city?.CityName;
            report.Barangay = barangay?.BarangayName;

            // Set the status to "Pending" for regular users
            report.Status = "Pending";
            report.ReporterName = firstname;
            report.ReporterId = user.Id;
            report.DateReported = DateTime.Now;

            // Save the report to the database
            await _context.StrayReports.AddAsync(report);
            await _context.SaveChangesAsync();

            return RedirectToAction("StrayReports");
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

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var report = await _context.StrayReports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            // Check if the current status is "Pending" before updating
            if (report.Status == "Pending" && (newStatus == "Captured" || newStatus == "Euthanized"))
            {
                report.Status = newStatus; // Update to Captured or Euthanized

                // Create a notification for the user
                var notification = new NotificationModel
                {
                    UserId = report.ReporterId, // Assuming you store the reporter's email in the report
                    Message = $"Your stray report for '{report.StrayType}' has been updated to '{newStatus}'.",
                    CreatedAt = DateTime.Now,
                    IsRead = false // Set to false initially
                };
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            if (report.Status == "Captured" && (newStatus == "Euthanized"))
            {
                report.Status = newStatus; // Update to Euthanized

                // Create a notification for the user
                var notification = new NotificationModel
                {
                    UserId = report.ReporterId, // Assuming you store the reporter's email in the report
                    Message = $"Your stray report for '{report.StrayType}' has been updated to '{newStatus}'.",
                    CreatedAt = DateTime.Now,
                    IsRead = false // Set to false initially
                };
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("StrayReports");
        }



        public IActionResult StrayReports()
        {
            var firstname = HttpContext.Session.GetString("Firstname");
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var isBarangayAdmin = HttpContext.Session.GetString("IsBarangayAdmin") == "True";
            var userBarangay = _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.Barangay) // Assuming BarangayId is stored in the User model
                .FirstOrDefault();

            // Get all stray reports
            var allStrayReports = _context.StrayReports
                .OrderByDescending(r => r.DateReported)
                .ToList();

            // Filter reports based on the user's role
            var filteredStrayReports = isBarangayAdmin
                ? allStrayReports.Where(r => r.Barangay == userBarangay).ToList()
                : allStrayReports;

            var viewModel = new StrayReportsViewModel
            {
                UserStrayReports = _context.StrayReports
                    .Where(r => r.ReporterName == firstname)
                    .OrderByDescending(r => r.DateReported)
                    .ToList(),
                AllStrayReports = filteredStrayReports.Where(r => r.Status == "Pending").ToList(), // Only Pending reports
                CapturedReports = filteredStrayReports.Where(r => r.Status == "Captured").ToList(), // Captured reports
                EuthanizedReports = filteredStrayReports.Where(r => r.Status == "Euthanized").ToList() // Euthanized reports
            };

            return View(viewModel);
        }

    }
}
