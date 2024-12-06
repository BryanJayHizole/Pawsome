using Microsoft.AspNetCore.Mvc;
using Pawsome.Data;
using Pawsome.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pawsome.Migrations;
using System.Drawing;
using Pawsome.Services;

namespace Pawsome.Controllers
{
    public class LostPetReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public LostPetReportController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
            // Get the list of countries, provinces, cities, and barangays from the database
            ViewBag.Countries = _context.Countries.Select(c => new SelectListItem { Value = c.CountryId.ToString(), Text = c.CountryName }).ToList();
            ViewBag.Provinces = _context.Provinces.Select(c => new SelectListItem { Value = c.ProvinceId.ToString(), Text = c.ProvinceName }).ToList();
            ViewBag.Cities = _context.Cities.Select(c => new SelectListItem { Value = c.CityId.ToString(), Text = c.CityName }).ToList();
            ViewBag.Barangays = _context.Barangays.Select(c => new SelectListItem { Value = c.BarangayId.ToString(), Text = c.BarangayName }).ToList();

            var pettypes = _context.PetTypes.Select(c => new SelectListItem { Value = c.TypeId.ToString(), Text = c.PType }).ToList();
            ViewBag.PetTypes = pettypes;
            var breeds = _context.Breeds.Select(c => new SelectListItem { Value = c.BreedId.ToString(), Text = c.BreedType }).ToList();
            ViewBag.Breeds = breeds;
            var tagtypes = _context.TagTypes.Select(c => new SelectListItem { Value = c.TagId.ToString(), Text = c.TagTypeName }).ToList();
            ViewBag.TagTypes = tagtypes;

            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.Pets = _context.Pets.Where(p => p.OwnerId.ToString() == userId && p.IsArchived == false && p.PetStatus != "Lost").ToList();

            return View();
        }

        // POST: LostPetReport/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile photoFile, LostPetReport lostPetReport, Barangay barangy, City municipality, PetType pettyp, Breed brid, TagType tagtyp)
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

            // Fetch the required details using IDs
            var pettype = _context.PetTypes.SingleOrDefault(c => c.TypeId == pettyp.TypeId);
            var breed = _context.Breeds.SingleOrDefault(c => c.BreedId == brid.BreedId);
            var tagtype = _context.TagTypes.SingleOrDefault(c => c.TagId == tagtyp.TagId);

            if (lostPetReport.PetId.HasValue)
            {
                // If the user selected an existing pet
                var pet = await _context.Pets.FindAsync(lostPetReport.PetId);
                if (pet != null)
                {
                    lostPetReport.IsOwnPet = true;
                    lostPetReport.OwnerId = pet.OwnerId;
                    lostPetReport.OwnerName = pet.OwnerName;
                    lostPetReport.OwnerContact = pet.OwnerContact;
                    lostPetReport.ReporterBarangay = pet.PetBarangay;
                    lostPetReport.PetName = pet.Name;
                    lostPetReport.PetGender = pet.Gender;
                    lostPetReport.PetColor = pet.Color;
                    lostPetReport.PetType = pet.PetType; // Use existing type from the pet
                    lostPetReport.PetBreed = pet.Breed; // Use existing breed from the pet
                    lostPetReport.PetTag = pet.TagType; // Use existing tag from the pet
                    lostPetReport.PetTagNo = pet.TagNumber;
                    lostPetReport.Photo = pet.Photo;
                    pet.PetStatus = "Lost"; // Mark the pet as lost
                }
            }
            else
            {
                // If manually entering pet details
                if (string.IsNullOrEmpty(lostPetReport.PetName) || string.IsNullOrEmpty(lostPetReport.PetType))
                {
                    ModelState.AddModelError("", "Pet name and type are required for reporting a pet.");
                    return View(lostPetReport);
                }

                // Save the uploaded photo
                if (photoFile != null && photoFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await photoFile.CopyToAsync(memoryStream);
                        lostPetReport.Photo = memoryStream.ToArray();
                    }
                }

                // Assign `PetType`, `BreedType`, and `TagType` as strings
                if (pettype != null)
                {
                    lostPetReport.PetType = pettype?.PType; // Save PetType (string)
                }

                if (breed != null)  
                {
                    lostPetReport.PetBreed = breed?.BreedType; // Save BreedType (string)
                }

                if (tagtype != null)
                {
                    lostPetReport.PetTag = tagtype?.TagTypeName;// Save TagType (string)
                }

                lostPetReport.ReporterBarangay = user.Barangay;
            }

            // Populate lost pet report details
            lostPetReport.DateReported = DateTime.Now;
            lostPetReport.ReporterId = user.Id;

            // Fetch the city and barangay names based on the IDs
            var city = _context.Cities.SingleOrDefault(c => c.CityId == municipality.CityId);
            var barangay = _context.Barangays.SingleOrDefault(b => b.BarangayId == barangy.BarangayId);

            lostPetReport.City = city?.CityName;
            lostPetReport.Barangay = barangay?.BarangayName;

            _context.LostPetReports.Add(lostPetReport);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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

        // GET: LostPetReport/Index
        [Authorize]
        public IActionResult Index(int userPage = 1, int allPage = 1, int foundPage = 1)
        {
            const int pageSize = 12; // Number of reports per page

            // Calculate skip values for each section
            int userSkip = (userPage - 1) * pageSize;
            int allSkip = (allPage - 1) * pageSize;
            int foundSkip = (foundPage - 1) * pageSize;

            // Get user information from the session
            var userId = HttpContext.Session.GetString("UserId");
            var email = HttpContext.Session.GetString("Email");
            var barangay = HttpContext.Session.GetString("Barangay"); // Get the user's Barangay from the session

            // Determine if the user is a Barangay admin
            var isBarangayAdmin = HttpContext.Session.GetString("IsBarangayAdmin") == "True";

            // Fetch UserLostPetReports (reports by the current user)
            var userLostPetReports = _context.LostPetReports
                .Include(r => r.Pet)
                .Where(r => r.ReporterId.ToString() == userId && !r.IsFound)
                .OrderByDescending(r => r.DateReported)
                .Skip(userSkip)
                .Take(pageSize)
                .ToList();

            // Fetch the user's Barangay
            var userBarangay = _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.Barangay)
                .FirstOrDefault();

            // Fetch AllLostPetReports (reports from the user's Barangay if they are a Barangay admin)
            var allLostPetReports = isBarangayAdmin
                ? _context.LostPetReports
                    .Include(r => r.Pet)
                    .Where(r => !r.IsFound && r.Barangay == userBarangay)
                    .OrderByDescending(r => r.DateReported)
                    .Skip(allSkip)
                    .Take(pageSize)
                    .ToList()
                : _context.LostPetReports
                    .Include(r => r.Pet)
                    .Where(r => r.ReporterId.ToString() != userId && !r.IsFound)
                    .OrderByDescending(r => r.DateReported)
                    .Skip(allSkip)
                    .Take(pageSize)
                    .ToList();

            // Fetch FoundPetReports (found pets)
            var foundPetReports = isBarangayAdmin
                ? _context.LostPetReports
                    .Include(r => r.Pet)
                    .Where(r => r.IsFound && r.Barangay == userBarangay) // Filter by Barangay
                    .OrderByDescending(r => r.DateFound)
                    .Skip(foundSkip)
                    .Take(pageSize)
                    .ToList()
                : _context.LostPetReports
                    .Include(r => r.Pet)
                    .Where(r => r.IsFound) // Show all found pets for non-Barangay admins
                    .OrderByDescending(r => r.DateFound)
                    .Skip(foundSkip)
                    .Take(pageSize)
                    .ToList();


            // Total count for each section's pagination calculation
            var totalUserReports = _context.LostPetReports.Count(r => r.ReporterId.ToString() == userId && !r.IsFound);
            var totalAllReports = isBarangayAdmin
                ? _context.LostPetReports.Count(r => r.Barangay == userBarangay && !r.IsFound)
                : _context.LostPetReports.Count(r => r.ReporterId.ToString() != userId && !r.IsFound);
            var totalFoundReports = isBarangayAdmin
                ? _context.LostPetReports.Count(r => r.IsFound && r.Barangay == userBarangay) // Count only found pets in the same Barangay
                : _context.LostPetReports.Count(r => r.IsFound);

            // Calculate total pages for each section
            var totalUserPages = (int)Math.Ceiling(totalUserReports / (double)pageSize);
            var totalAllPages = (int)Math.Ceiling(totalAllReports / (double)pageSize);
            var totalFoundPages = (int)Math.Ceiling(totalFoundReports / (double)pageSize);

            // Prepare the view model
            var viewModel = new LostPetReportsViewModel
            {
                UserLostPetReports = userLostPetReports,
                AllLostPetReports = allLostPetReports,
                FoundPets = foundPetReports,
                CurrentUserPage = userPage,
                TotalUserPages = totalUserPages,
                CurrentAllPage = allPage,
                TotalAllPages = totalAllPages,
                CurrentFoundPage = foundPage,
                TotalFoundPages = totalFoundPages
            };

            return View(viewModel);
        }



        // POST: LostPetReport/MarkAsFound
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MarkAsFound( int id, string finderName, string foundLocation, string additionalInfo)
        {
            

            var report = _context.LostPetReports.FirstOrDefault(r => r.Id == id);

            if (report == null )
            {
                return NotFound();
            }

            report.IsFound = true;
            report.DateFound = DateTime.Now;
            report.AdditionalInfo = additionalInfo;  // Optionally, you can store additional information
            report.FoundLocation = foundLocation; // If applicable, save the found location
            report.FinderName = finderName; // You might want to create a new property in your model for the finder name

            // Update the pet's status (if applicable)
            var pet = _context.Pets.FirstOrDefault(p => p.Id == report.PetId);
            if (pet != null)
            {
                // Remove or adjust pet's lost status
                pet.PetStatus = "Alive"; // Example: update status to "Found"
                _context.Update(pet);
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == report.ReporterId);
            if (user != null)
            {
                // Prepare the email content
                var subject = "Good News! Your Lost Pet Has Been Found!";
                var body = $"Dear {user.Firstname},\n\n" +
                           $"We are excited to inform you that your lost pet has been successfully found!\n\n" +
                           $"Here are the details:\n" +
                           $"- **Finder's Name**: {finderName}\n" +
                           $"- **Found Location**: {foundLocation}\n" +
                           $"- **Additional Information**: {additionalInfo}\n\n" +
                           $"We truly appreciate your use of Pawsome. We’re happy to have been part of your pet's journey back home.\n\n" +
                           $"Should you have any questions or need further assistance, feel free to reach out to us.\n\n" +
                           $"Thank you for trusting us with your pet's safety!\n\n" +
                           $"Best regards,\n" +
                           $"The Pawsome Team\n";
                 


                // Send the email (you would need to inject an email sender service)
                await _emailService.SendEmailAsync(user.Email, subject, body);
            }


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
                .Where(r => r.Barangay == barangay && r.IsFound)
                .Select(r => new
                {
                    PetName = r.Pet != null ? r.PetName : "Unknown", // Handle null Pet cases
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