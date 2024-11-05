using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pawsome.Data;
using Pawsome.Models;
using Pawsome.Services;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;

namespace Pawsome.Controllers
{
    public class PetController : Controller
    {
        private readonly ApplicationDbContext _context;
  

        public PetController(ApplicationDbContext context)
        {
            _context = context;
           
        }

        public IActionResult Index(string selectedCity, string selectedBarangay)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var firstname = HttpContext.Session.GetString("Firstname");
            var isBarangayAdmin = HttpContext.Session.GetString("IsBarangayAdmin");
            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");
            var userBarangay = _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.Barangay)
                .FirstOrDefault();

            // Fetch all cities and barangays for filtering
            ViewBag.Cities = _context.Cities.ToList(); // Ensure this is populated
            ViewBag.Barangays = _context.Barangays.ToList(); // Ensure this is populated

            // Start with all pets
            var pets = _context.Pets
                .Where(p => !p.IsArchived) // Filter out archived pets
                .ToList(); // Ensure pets is a List

            if (isBarangayAdmin == "True")
            {
                // If the user is a Barangay Admin, show all pets in their barangay
                pets = pets.Where(p => p.PetBarangay == userBarangay).ToList(); // Convert to List
            }
            else if (isPvetAdmin == "True")
            {
                // Apply filters based on selected city and barangay
                if (!string.IsNullOrEmpty(selectedCity))
                {
                    pets = pets.Where(p => p.PetCity == selectedCity).ToList(); // Convert to List
                }

                if (!string.IsNullOrEmpty(selectedBarangay))
                {
                    pets = pets.Where(p => p.PetBarangay == selectedBarangay).ToList(); // Convert to List
                }
            }
            else
            {
                // If the user is not an admin, show only their pets
                pets = pets.Where(p => p.OwnerName == firstname).ToList(); // Convert to List
            }

            // Create the view model with filtered pets
            var viewModel = new PetViewModel
            {
                Pets = pets, // No need to call ToList() again as it's already a List
                Cities = ViewBag.Cities, // Pass cities to the view model if needed
                Barangays = ViewBag.Barangays // Pass barangays to the view model if needed
            };

            return View(viewModel);
        }



        public IActionResult Create()
        {
            var pettypes = _context.PetTypes.Select(c => new SelectListItem { Value = c.TypeId.ToString(), Text = c.PType }).ToList();
            ViewBag.PetTypes = pettypes;
            var habitats = _context.Habitats.Select(c => new SelectListItem { Value = c.HabitatId.ToString(), Text = c.HabitatType }).ToList();
            ViewBag.Habitats = habitats;
            var tagtypes = _context.TagTypes.Select(c => new SelectListItem { Value = c.TagId.ToString(), Text = c.TagTypeName }).ToList();
            ViewBag.TagTypes = tagtypes;
            var vaccinationstatuses = _context.VaccinationStatuses.Select(c => new SelectListItem { Value = c.VStatusId.ToString(), Text = c.VStatusType }).ToList();
            ViewBag.VaccinationStatuses = vaccinationstatuses;
            var vaccinetypes = _context.VaccineTypes.Select(c => new SelectListItem { Value = c.VTypeId.ToString(), Text = c.VType }).ToList();
            ViewBag.VaccineTypes = vaccinetypes;
            var vaccinesources = _context.VaccineSources.Select(c => new SelectListItem { Value = c.VSourceId.ToString(), Text = c.VSource }).ToList();
            ViewBag.VaccineSources = vaccinesources;

            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pet pet, IFormFile photoFile, PetType pettyp, Habitat habitaat, TagType tagtyp, VaccinationStatus vstats, VaccineType vtyp, VaccineSource vsourc)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }
            var pettype = _context.PetTypes.SingleOrDefault(c => c.TypeId == pettyp.TypeId);
            var habitat = _context.Habitats.SingleOrDefault(c => c.HabitatId == habitaat.HabitatId);
            var tagtype = _context.TagTypes.SingleOrDefault(c => c.TagId == tagtyp.TagId);
            var vstatus = _context.VaccinationStatuses.SingleOrDefault(c => c.VStatusId == vstats.VStatusId);
            var vtype = _context.VaccineTypes.SingleOrDefault(c => c.VTypeId == vtyp.VTypeId);
            var vsource = _context.VaccineSources.SingleOrDefault(c => c.VSourceId == vsourc.VSourceId);

            pet.PetType = pettype?.PType;
            pet.Habitat = habitat?.HabitatType;
            pet.TagType = tagtype?.TagTypeName;
            pet.VaccinationStatus = vstatus?.VStatusType;
            pet.VaccineType = vtype?.VType;
            pet.VaccineSource = vsource?.VSource;

            var user = _context.Users.SingleOrDefault(u => u.Email == email);
            pet.OwnerId = user.Id;
            pet.OwnerName = user.Firstname;
            pet.OwnerContact = user.ContactNumber;
            pet.PetProvince = user.Province;
            pet.PetCity = user.City;
            pet.PetBarangay = user.Barangay;


            if (photoFile != null && photoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await photoFile.CopyToAsync(memoryStream);
                    pet.Photo = memoryStream.ToArray();  // Save the photo as a byte array
                }
            }

            // Clear vaccination details if pet is not vaccinated
            if (pet.VaccinationStatus == "Not Vaccinated")
            {
                pet.VaccineType = null;
                pet.VaccinationDate = null;
                pet.VaccinatedBy = null;
                pet.VaccineBlockNo = null;
                pet.VaccineSource = null;
                pet.NextDueDate = null; // Set to null since it won't be vaccinated
            }
            else if (pet.VaccinationDate.HasValue)
            {
                // Set NextDueDate to one year after VaccinationDate
                pet.NextDueDate = pet.VaccinationDate.Value.AddYears(1);
            }

            _context.Pets.Add(pet);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult VaccinationHistory(int id)
        {
            // Fetch the pet details including vaccination information
            var pet = _context.Pets
                .Include(p => p.VaccinationHistories) // Ensure this is included
                .FirstOrDefault(p => p.Id == id);

            if (pet == null)
            {
                return NotFound("Pet not found.");
            }

            return View("VaccinationHistory", pet); // Pass the single pet to a new view
        }


        public IActionResult Edit(int id)
        {
            var pettypes = _context.PetTypes.Select(c => new SelectListItem { Value = c.TypeId.ToString(), Text = c.PType }).ToList();
            ViewBag.PetTypes = pettypes;
            var habitats = _context.Habitats.Select(c => new SelectListItem { Value = c.HabitatId.ToString(), Text = c.HabitatType }).ToList();
            ViewBag.Habitats = habitats;
            var tagtypes = _context.TagTypes.Select(c => new SelectListItem { Value = c.TagId.ToString(), Text = c.TagTypeName }).ToList();
            ViewBag.TagTypes = tagtypes;
            var vaccinationstatuses = _context.VaccinationStatuses.Select(c => new SelectListItem { Value = c.VStatusId.ToString(), Text = c.VStatusType }).ToList();
            ViewBag.VaccinationStatuses = vaccinationstatuses;
            var vaccinetypes = _context.VaccineTypes.Select(c => new SelectListItem { Value = c.VTypeId.ToString(), Text = c.VType }).ToList();
            ViewBag.VaccineTypes = vaccinetypes;
            var vaccinesources = _context.VaccineSources.Select(c => new SelectListItem { Value = c.VSourceId.ToString(), Text = c.VSource }).ToList();
            ViewBag.VaccineSources = vaccinesources;

           
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }
            var firstname = HttpContext.Session.GetString("Firstname");
            var pet = _context.Pets.FirstOrDefault(p => p.Id == id && p.OwnerName == firstname);
            if (pet == null)
            {
                return NotFound(); // Or handle the case where the pet is not found
            }

            return View(pet);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Pet updatedPet, IFormFile petProfile, PetType pettyp, Habitat habitaat, TagType tagtyp, VaccinationStatus vstats, VaccineType vtyp, VaccineSource vsourc)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var existingPet = await _context.Pets.Include(p => p.VaccinationHistories).FirstOrDefaultAsync(p => p.Id == updatedPet.Id);

            if (existingPet == null)
            {
                return NotFound(); // Handle the case where the pet is not found
            }

            // Save the previous vaccination details to history
            existingPet.VaccinationHistories.Add(new VaccinationHistory
            {
                VaccinationStatus = existingPet.VaccinationStatus,
                VaccineType = existingPet.VaccineType,
                VaccinationDate = existingPet.VaccinationDate ?? DateTime.MinValue,
                AdministeredBy = existingPet.VaccinatedBy,
                NextDueDate = existingPet.NextDueDate ?? DateTime.MinValue
            });

     
            existingPet.Name = updatedPet.Name;
            existingPet.Breed = updatedPet.Breed;
            existingPet.Age = updatedPet.Age;

     
            var habitat = _context.Habitats.SingleOrDefault(c => c.HabitatId == habitaat.HabitatId);
            existingPet.Habitat = habitat?.HabitatType;
            var tagtype = _context.TagTypes.SingleOrDefault(c => c.TagId == tagtyp.TagId);
            existingPet.TagType = tagtype?.TagTypeName;
            var vaccinationstatus = _context.VaccinationStatuses.SingleOrDefault(c => c.VStatusId == vstats.VStatusId);
            existingPet.VaccinationStatus = vaccinationstatus?.VStatusType;
            var vaccinetype = _context.VaccineTypes.SingleOrDefault(c => c.VTypeId == vtyp.VTypeId);
            existingPet.VaccineType = vaccinetype?.VType;
            var vaccinesource = _context.VaccineSources.SingleOrDefault(c => c.VSourceId == vsourc.VSourceId);
            existingPet.VaccineSource = vaccinesource?.VSource;
     




            existingPet.TagNumber = updatedPet.TagNumber;
            existingPet.Weight = updatedPet.Weight;
            existingPet.VaccinationDate = updatedPet.VaccinationDate;
            existingPet.VaccinatedBy = updatedPet.VaccinatedBy;
            existingPet.VaccineBlockNo = updatedPet.VaccineBlockNo;
            existingPet.VaccineSource = updatedPet.VaccineSource;

            // Calculate and update the NextDueDate based on the vaccine type
            existingPet.NextDueDate = CalculateNextDueDate(existingPet.VaccinationDate, existingPet.VaccineType);

            // Check if a new profile photo is uploaded
            if (petProfile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await petProfile.CopyToAsync(memoryStream);
                    existingPet.Photo = memoryStream.ToArray(); // Save the uploaded photo to the pet's profile
                }
            }

            await _context.SaveChangesAsync(); // Save the changes to the database

            return RedirectToAction("Index"); // Redirect to the list of registered pets
        }

        // Method to calculate the next due date based on vaccination date and vaccine type
        private DateTime? CalculateNextDueDate(DateTime? vaccinationDate, string vaccineType)
        {
            if (!vaccinationDate.HasValue)
                return null;

            // Example logic for calculating next due date
            int dueIntervalInMonths = vaccineType switch
            {
                "Anti Rabies" => 12, // Rabies vaccine is due every 12 months
                "Others" => 6,       // Other vaccines are due every 6 months
                _ => 12               // Default to 12 months if type is unknown
            };

            return vaccinationDate.Value.AddMonths(dueIntervalInMonths);
        }

        public IActionResult View(int id)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.Id == id);
            if (pet == null)
            {
                return NotFound();
            }
            return View(pet);
        }


        public IActionResult Delete(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var firstname = HttpContext.Session.GetString("Firstname");
            var pet = _context.Pets.FirstOrDefault(p => p.Id == id && p.OwnerName == firstname);
            if (pet == null)
            {
                return NotFound(); // Or handle the case where the pet is not found
            }

            _context.Pets.Remove(pet); // Remove the pet from the database
            _context.SaveChanges(); // Save the changes to the database

            return RedirectToAction("Index"); // Redirect to the list of registered pets
        }

        [HttpPost]
        public IActionResult Archive(int id)
        {
            var pet = _context.Pets.Find(id);
            if (pet != null)
            {
                // Assuming there's a property called IsArchived
                pet.IsArchived = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Index"); // Redirect back to the index or relevant page
        }


        [HttpPost]
        public IActionResult Verify(int id)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.Id == id);
            if (pet == null)
            {
                return NotFound();
            }

            pet.IsVerified = true; // Set the pet as verified
            _context.SaveChanges(); // Save changes to the database

            // Create a notification for the user
            var notification = new NotificationModel
            {
                UserId = pet.OwnerId, // Assuming Pet has an OwnerId property linking to the User
                Message = $"Your pet '{pet.Name}' has been verified!",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification); // Add the notification to the context
            _context.SaveChanges(); // Save changes to the database

            return RedirectToAction("Index"); // Redirect back to the list of pets
        }

        


    }
}
