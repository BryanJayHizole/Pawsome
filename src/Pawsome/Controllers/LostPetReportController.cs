using Microsoft.AspNetCore.Mvc;
using Pawsome.Data;
using Pawsome.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Pawsome.Controllers
{
    public class LostPetReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LostPetReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LostPetReport/Create
        [Authorize]
        public IActionResult Create()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }
            var firstname = HttpContext.Session.GetString("Firstname");
            ViewBag.Pets = _context.Pets.Where(p => p.OwnerName == firstname).ToList();
            return View();
        }

        // POST: LostPetReport/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LostPetReport lostPetReport)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var firstname = HttpContext.Session.GetString("Firstname");
            var pet = _context.Pets.FirstOrDefault(p => p.Id == lostPetReport.PetId && p.OwnerName == firstname);

            if (pet == null)
            {
                ModelState.AddModelError("PetId", "The selected pet does not belong to you.");
                ViewBag.Pets = _context.Pets.Where(p => p.OwnerName == firstname).ToList();
                return View(lostPetReport);
            }

            // Populate lost pet report details
            lostPetReport.OwnerName = pet.OwnerName;
            lostPetReport.OwnerContact = pet.OwnerContact;
            lostPetReport.ReporterBarangay = pet.PetBarangay;
            lostPetReport.PetName = pet.Name;
            lostPetReport.PetGender = pet.Gender;
            lostPetReport.PetColor = pet.Color;
            lostPetReport.PetType = pet.PetType;
            lostPetReport.PetBreed = pet.Breed;
            lostPetReport.PetTag = pet.TagType;
            lostPetReport.PetTagNo = pet.TagNumber;
            lostPetReport.Photo = pet.Photo;
            lostPetReport.DateReported = DateTime.Now;

            _context.LostPetReports.Add(lostPetReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: LostPetReport/Index
        [Authorize]
        public IActionResult Index()
        {
            var firstname = HttpContext.Session.GetString("Firstname");
            var email = HttpContext.Session.GetString("Email");
            var barangay = HttpContext.Session.GetString("Barangay"); // Get the user's Barangay from the session

            var userLostPetReports = _context.LostPetReports
                .Include(r => r.Pet)
                .Where(r => r.OwnerName == firstname && !r.IsFound)
                .OrderByDescending(r => r.DateReported)
                .ToList();

            // Determine if the user is a Barangay admin
            var isBarangayAdmin = HttpContext.Session.GetString("IsBarangayAdmin") == "True";

            var userBarangay = _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.Barangay)
                .FirstOrDefault();

            // Filter reports based on the user's role
            var allLostPetReports = isBarangayAdmin
                ? _context.LostPetReports
                    .Include(r => r.Pet)
                    .Where(r => !r.IsFound && r.ReporterBarangay == userBarangay)
                    .OrderByDescending(r => r.DateReported)
                    .ToList()
                : _context.LostPetReports
                    .Include(r => r.Pet)
                    .Where(r => r.OwnerName != firstname && !r.IsFound)
                    .OrderByDescending(r => r.DateReported)
                    .ToList();

            var foundPetReports = _context.LostPetReports
                .Include(r => r.Pet) // Ensure Pet is included
                .Where(r => r.IsFound)
                .OrderByDescending(r => r.DateFound)
                .ToList();

            var viewModel = new LostPetReportsViewModel
            {
                UserLostPetReports = userLostPetReports,
                AllLostPetReports = allLostPetReports, 
                FoundPets = foundPetReports
            };

            return View(viewModel);
        }

        // POST: LostPetReport/MarkAsFound
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MarkAsFound(int reportId, string finderName, string foundLocation, string additionalInfo)
        {
            var report = await _context.LostPetReports.FindAsync(reportId);

            if (report == null || report.OwnerName != HttpContext.Session.GetString("Firstname"))
            {
                return NotFound();
            }

            report.IsFound = true;
            report.DateFound = DateTime.Now;
            report.AdditionalInfo = additionalInfo;  // Optionally, you can store additional information
            report.LastSeenLocation = foundLocation; // If applicable, save the found location
            report.FinderName = finderName; // You might want to create a new property in your model for the finder name

            _context.LostPetReports.Update(report);
            await _context.SaveChangesAsync();

            return Json(new { success = true }); // Return JSON response for AJAX
        }

        // GET: LostPetReport/BarangayFoundPetHistory
        [HttpGet]
        public async Task<IActionResult> BarangayFoundPetHistory()
        {
            var email = HttpContext.Session.GetString("Email");
            var barangay = HttpContext.Session.GetString("Barangay");

            var foundPetReports = await _context.LostPetReports
                .Include(r => r.Pet) // Ensure Pet is included
                .Where(r => r.ReporterBarangay == barangay && r.IsFound)
                .Select(r => new
                {
                    PetName = r.Pet != null ? r.Pet.Name : "Unknown", // Handle null Pet cases
                    DateFound = r.DateFound.HasValue ? r.DateFound.Value.ToString("yyyy-MM-dd") : "Not Available", // Handle null DateFound cases
                    OwnerName = r.OwnerName
                })
                .ToListAsync();

            return View(foundPetReports);
        }

        [HttpGet]
        public IActionResult GetReportDetails(int id)
        {
            // Fetch the report from the database using the provided ID
            var report = _context.LostPetReports
                .Include(r => r.Pet) // Include related Pet details
                .FirstOrDefault(r => r.Id == id); // Assuming 'Id' is the key in LostPetReports

            if (report == null)
            {
                return NotFound(); // Return 404 if not found
            }

            // Return the details to be rendered in the modal
            return PartialView("_ReportDetails", report); // Assuming you create a partial view for the details
        }

    }
}
