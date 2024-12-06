using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pawsome.Data;
using Pawsome.Models;
using Pawsome.Services;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Security.Claims;

namespace Pawsome.Controllers
{
    public class PetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public PetController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public IActionResult Index(string searchTerm, string selectedCity, string selectedBarangay, int page = 1)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var currentUserId = HttpContext.Session.GetString("UserId");
            var isBarangayAdmin = HttpContext.Session.GetString("IsBarangayAdmin");
            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");
            var userBarangay = _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.Barangay)
                .FirstOrDefault();

            // Fetch all cities and barangays for filtering
            ViewBag.Cities = _context.Cities.ToList();
            ViewBag.Barangays = _context.Barangays.ToList();

            // Start with all pets and include related incidents for checks
            var pets = _context.Pets
                .Where(p => !p.IsArchived) // Filter out archived pets
                .AsQueryable(); // Use IQueryable for better query composition

            if (isBarangayAdmin == "True")
            {
                // If the user is a Barangay Admin, show all pets in their barangay
                pets = pets.Where(p => p.PetBarangay == userBarangay);
            }
            else if (isPvetAdmin == "True")
            {
                // Apply filters based on selected city and barangay
                if (!string.IsNullOrEmpty(selectedCity))
                {
                    pets = pets.Where(p => p.PetCity == selectedCity);
                }

                if (!string.IsNullOrEmpty(selectedBarangay))
                {
                    pets = pets.Where(p => p.PetBarangay == selectedBarangay);
                }
            }
            else
            {
                // If the user is not an admin, show only their pets
                pets = pets.Where(p => p.OwnerId.ToString() == currentUserId);
            }

            // Apply search term if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                pets = pets.Where(p => p.Name.Contains(searchTerm) ||
                                       p.Breed.Contains(searchTerm) ||
                                       p.OwnerName.Contains(searchTerm));
            }

            // Pagination logic
            int pageSize = 12; // Number of pets per page
            int totalPets = pets.Count(); // Get total pet count
            int totalPages = (int)Math.Ceiling(totalPets / (double)pageSize); // Calculate total pages

            // Get pets for the current page
            var petsOnPage = pets.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Fetch transfer requests where the logged-in user is either the current owner or the new owner
            var transferRequests = _context.Pets
                .Where(p => p.IsTransferPending &&
                            (p.OwnerId.ToString() == currentUserId || p.NewOwnerId.ToString() == currentUserId))
                .ToList();

            var barangay = HttpContext.Session.GetString("Barangay");
            // Create the view model with filtered pets and pagination info
            var viewModel = new PetViewModel
            {
                Pets = petsOnPage, // Pets for the current page
                Cities = ViewBag.Cities,
                Barangays = ViewBag.Barangays,
                TransferRequests = transferRequests,
                CurrentPage = page,
                TotalPages = totalPages,
                ArchivedPetsByUser = _context.Pets.Where(p => p.OwnerId .ToString()== currentUserId && p.IsArchived).ToList(),
                ArchivedPetsByBarangay = _context.Pets.Where(p => p.PetBarangay == barangay && p.IsArchived).ToList(),
                AllArchivedPets = _context.Pets.Where(p => p.IsArchived).ToList(),
            };

            // Set flags for each pet individually
            foreach (var pet in petsOnPage)
            {
                var matchingIncidents = _context.RabiesIncidents
                    .Where(ri => ri.PetId == pet.Id)
                    .ToList();

                pet.HasReportedIncidents = matchingIncidents.Any(); // Check for any matching incidents
                pet.HasRabies = matchingIncidents.Any(ri => ri.IsVerified); // Check if any incident is verified as rabies
            }

            // Assign flags for the view if needed
            ViewBag.HasReportedIncidents = petsOnPage.Any(p => p.HasReportedIncidents);
            ViewBag.HasRabies = petsOnPage.Any(p => p.HasRabies);


            return View(viewModel);
        }


        public IActionResult Create(Pet pet)
        {
            var pettypes = _context.PetTypes.Select(c => new SelectListItem { Value = c.TypeId.ToString(), Text = c.PType }).ToList();
            ViewBag.PetTypes = pettypes;
            var breeds = _context.Breeds.Select(c => new SelectListItem { Value = c.BreedId.ToString(), Text = c.BreedType }).ToList();
            ViewBag.Breeds = breeds;
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

            ViewBag.AllPenalties = _context.PenaltyFines.ToList(); // Pass available penalties
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pet pet, IFormFile photoFile, PetType pettyp, Breed brid, Habitat habitaat, TagType tagtyp, VaccinationStatus vstats, VaccineType vtyp, VaccineSource vsourc)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Check if the pet with the same name and owner already exists
            var existingPet = _context.Pets
                .FirstOrDefault(p => p.Name == pet.Name && p.OwnerId.ToString() == userId);

            if (existingPet != null)
            {
                // Set TempData for duplicate pet modal
                TempData["PetAlreadyExists"] = "true";
                TempData["ExistingPetId"] = existingPet.Id.ToString();
                TempData["ExistingPetName"] = existingPet.Name;
                return RedirectToAction("Create"); // Redirect to prevent submission before modal
            }

            var pettype = _context.PetTypes.SingleOrDefault(c => c.TypeId == pettyp.TypeId);
            var breed = _context.Breeds.SingleOrDefault(c => c.BreedId == brid.BreedId);
            var habitat = _context.Habitats.SingleOrDefault(c => c.HabitatId == habitaat.HabitatId);
            var tagtype = _context.TagTypes.SingleOrDefault(c => c.TagId == tagtyp.TagId);
            var vstatus = _context.VaccinationStatuses.SingleOrDefault(c => c.VStatusId == vstats.VStatusId);
            var vtype = _context.VaccineTypes.SingleOrDefault(c => c.VTypeId == vtyp.VTypeId);
            var vsource = _context.VaccineSources.SingleOrDefault(c => c.VSourceId == vsourc.VSourceId);

            pet.PetType = pettype?.PType;
            pet.Breed = breed?.BreedType;
            pet.Habitat = habitat?.HabitatType;
            pet.TagType = tagtype?.TagTypeName;
            pet.VaccinationStatus = vstatus?.VStatusType;
            pet.VaccineType = vtype?.VType;
            pet.VaccineSource = vsource?.VSource;

            var user = _context.Users.SingleOrDefault(u => u.Email == email);
            pet.OwnerId = user.Id;
            pet.OwnerName = $"{user.Firstname} {user.LastName}".Trim();
            pet.OwnerEmail = user.Email;
            pet.OwnerContact = user.ContactNumber;
            pet.PetStatus = "Alive";
            pet.PetProvince = user.Province;
            pet.PetCity = user.City;
            pet.PetBarangay = user.Barangay;


            if (photoFile != null && photoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await photoFile.CopyToAsync(memoryStream);
                    pet.Photo = memoryStream.ToArray();
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

            if (pet.DateOfBirth.HasValue)
            {
                var today = DateTime.Today;
                var age = today.Year - pet.DateOfBirth.Value.Year;
                if (pet.DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                pet.Age = age;
            }



            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            // Send vaccination reminder if needed
            await _emailService.SendVaccinationReminderAsync(pet);

            return RedirectToAction("Index", new { success = true });
        }

        [HttpGet]
        public JsonResult GetBreedsByType(int typeId)
        {
            // Fetch the breeds from the database where TypeId matches the selected type
            var breeds = _context.Breeds
                                 .Where(b => b.TypeId == typeId)
                                 .Select(b => new { b.BreedId, b.BreedType })
                                 .ToList();

            // Return breeds as JSON
            return Json(breeds);
        }



        public IActionResult VaccinationHistory(int id)
        {
            var vaccinationstatuses = _context.VaccinationStatuses
                .Select(c => new SelectListItem { Value = c.VStatusId.ToString(), Text = c.VStatusType })
                .ToList();
            ViewBag.VaccinationStatuses = vaccinationstatuses;

            var vaccinetypes = _context.VaccineTypes
                .Select(c => new SelectListItem { Value = c.VTypeId.ToString(), Text = c.VType })
                .ToList();
            ViewBag.VaccineTypes = vaccinetypes;

            var vaccinesources = _context.VaccineSources
                .Select(c => new SelectListItem { Value = c.VSourceId.ToString(), Text = c.VSource })
                .ToList();
            ViewBag.VaccineSources = vaccinesources;

            var pet = _context.Pets
                .Include(p => p.VaccinationHistories)
                .FirstOrDefault(p => p.Id == id);

            if (pet == null)
            {
                return NotFound("Pet not found.");
            }

            // Populate the properties of the pet with the names associated with the IDs
            foreach (var history in pet.VaccinationHistories)
            {
                var vaccinationStatus = _context.VaccinationStatuses
                    .FirstOrDefault(v => v.VStatusId.ToString() == history.VaccinationStatus);
                history.VaccinationStatus = vaccinationStatus?.VStatusType;

                var vaccineType = _context.VaccineTypes
                    .FirstOrDefault(v => v.VTypeId.ToString() == history.VaccineType);
                history.VaccineType = vaccineType?.VType;

                var vaccineSource = _context.VaccineSources
                    .FirstOrDefault(v => v.VSourceId.ToString() == history.VaccineSource);
                history.VaccineSource = vaccineSource?.VSource;


                // Calculate NextDueDate as 1 year after VaccinationDate
                if (history.VaccinationDate.HasValue)
                {
                    history.NextDueDate = history.VaccinationDate.Value.AddYears(1);
                }
            }

            return View("VaccinationHistory", pet);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOldVaccination(VaccinationHistory vaccinationHistory, int petId)
        {
            if (ModelState.IsValid)
            {
                var pet = await _context.Pets.Include(p => p.VaccinationHistories)
                                              .FirstOrDefaultAsync(p => p.Id == petId);

                if (pet != null)
                {

                    // Automatically calculate NextDueDate when adding a new vaccination record
                    if (vaccinationHistory.VaccinationDate.HasValue)
                    {
                        vaccinationHistory.NextDueDate = vaccinationHistory.VaccinationDate.Value.AddYears(1);
                    }


                    // Add the new vaccination record to the pet's list of vaccination histories
                    pet.VaccinationHistories.Add(vaccinationHistory);
                    _context.Update(pet);
                    await _context.SaveChangesAsync();

                    // Redirect back to the Vaccination History view, passing the updated pet model
                    return RedirectToAction("VaccinationHistory", new { id = pet.Id });
                }

                // Return a JSON response indicating failure if the pet is not found
                return Json(new { success = false, message = "Pet not found." });
            }

            // Return a JSON response with validation errors if the model state is not valid
            return Json(new { success = false, message = "Validation failed." });
        }




        [HttpPost]
        public IActionResult AddRecord(VaccinationHistory record)
        {
            if (record == null)
            {
                return BadRequest("Invalid vaccination record data.");
            }

            // Validate the record (e.g., ensure all required fields are present)
            if (!ModelState.IsValid)
            {
                return View(record); // Return to the form with validation errors
            }

            // Add the vaccination record to the database
            _context.VaccinationHistories.Add(record);

            // Update the pet's vaccination status (assumes a relationship between record and pet)
            var pet = _context.Pets.FirstOrDefault(p => p.Id == record.PetId);
            if (pet != null)
            {
                pet.VaccinationStatus = record.VaccinationStatus;
                pet.VaccineType = record.VaccineType;
                pet.VaccinationDate = record.VaccinationDate;
                pet.VaccinatedBy = record.AdministeredBy;
                pet.VaccineBlockNo = record.VaccineBlockNo;
                pet.VaccineSource = record.VaccineSource;
                pet.NextDueDate = record.NextDueDate;

                _context.Pets.Update(pet);
            }

            // Save changes to the database
            _context.SaveChanges();

            // Update the appointment status to 'Done'
            var appointment = _context.Appointments.FirstOrDefault(a => a.PetId == record.PetId && a.Status == "Approved");
            if (appointment != null)
            {
                appointment.Status = "Done";

                // Retrieve the service associated with the appointment
                var service = _context.Services
                                      .Include(s => s.ServiceInventoryItems)
                                      .ThenInclude(s => s.InventoryItem)
                                      .FirstOrDefault(s => s.ServiceName == appointment.Service);

                if (service != null)
                {
                    // Loop through the related inventory items and update quantities if they are consumable
                    foreach (var serviceInventoryItem in service.ServiceInventoryItems)
                    {
                        var inventoryItem = serviceInventoryItem.InventoryItem;

                        if (inventoryItem.Consumable && serviceInventoryItem.QuantityUsed > 0)
                        {
                            // Deduct the quantity from the inventory item
                            inventoryItem.Quantity -= serviceInventoryItem.QuantityUsed;

                            // Ensure the quantity does not go negative
                            inventoryItem.Quantity = Math.Max(0, inventoryItem.Quantity);

                            // Update the inventory item in the database
                            _context.InventoryItems.Update(inventoryItem);
                        }
                    }
                }

                // Save changes to the database
                _context.SaveChanges();
            }

            // Redirect to the "ApprovedAppointments" view or return a success response
            return RedirectToAction("ApprovedAppointments", "Appointment");
        }




        public IActionResult Edit(int id)
        {
            var pettypes = _context.PetTypes.Select(c => new SelectListItem { Value = c.TypeId.ToString(), Text = c.PType }).ToList();
            ViewBag.PetTypes = pettypes;
            var breeds = _context.Breeds.Select(c => new SelectListItem { Value = c.BreedId.ToString(), Text = c.BreedType }).ToList();
            ViewBag.Breeds = breeds;
            var habitats = _context.Habitats.Select(c => new SelectListItem { Value = c.HabitatId.ToString(), Text = c.HabitatType }).ToList();
            ViewBag.Habitats = habitats;
            var tagtypes = _context.TagTypes.Select(c => new SelectListItem { Value = c.TagId.ToString(), Text = c.TagTypeName }).ToList();
            ViewBag.TagTypes = tagtypes;
          

            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }
            var userId = HttpContext.Session.GetString("UserId");
            var pet = _context.Pets.FirstOrDefault(p => p.Id == id && p.OwnerId.ToString() == userId);
            if (pet == null)
            {
                return NotFound(); // Or handle the case where the pet is not found
            }

            return View(pet);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Pet updatedPet, IFormFile petProfile, PetType pettyp, Breed brid, Habitat habitaat, TagType tagtyp)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var existingPet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == updatedPet.Id);

            if (existingPet == null)
            {
                return NotFound(); // Handle the case where the pet is not found
            }

            
            existingPet.Name = updatedPet.Name;
            existingPet.PetStatus = updatedPet.PetStatus;


            var habitat = _context.Habitats.SingleOrDefault(c => c.HabitatId == habitaat.HabitatId);
            existingPet.Habitat = habitat?.HabitatType;
            var tagtype = _context.TagTypes.SingleOrDefault(c => c.TagId == tagtyp.TagId);
            existingPet.TagType = tagtype?.TagTypeName;
          

            existingPet.TagNumber = updatedPet.TagNumber;
            existingPet.Weight = updatedPet.Weight;
        


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


        public IActionResult DeletePet(int petId)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var userId = HttpContext.Session.GetString("UserId");
            // Retrieve the pet based on the petId
            var pet = _context.Pets.FirstOrDefault(p => p.Id == petId);
            if (pet == null)
            {
                return NotFound();
            }

            if (pet.OwnerId.ToString() != userId)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this pet.";
                return RedirectToAction("Index");
            }

            _context.Pets.Remove(pet);
            _context.SaveChanges();

            return RedirectToAction("Index");
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

        public IActionResult ShowArchivedPets()
        {
            var currentUserId = HttpContext.Session.GetString("UserId");
            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin") == "True";
            var isBarangayAdmin = HttpContext.Session.GetString("IsBarangayAdmin") == "True";

            List<Pet> archivedPets = new List<Pet>();

            if (isPvetAdmin)
            {
                // Fetch all archived pets for PvetAdmin
                archivedPets = _context.Pets.Where(p => p.IsArchived).ToList();
            }
            else if (isBarangayAdmin)
            {
                // Fetch archived pets for the Barangay of the logged-in user
                var barangay = HttpContext.Session.GetString("Barangay");
                archivedPets = _context.Pets.Where(p => p.IsArchived && p.PetBarangay == barangay).ToList();
            }
            else
            {
                // Fetch only the archived pets for the current user
                archivedPets = _context.Pets.Where(p => p.IsArchived && p.OwnerId.ToString() == currentUserId).ToList();
            }

            var model = new PetViewModel
            {
                ArchivedPets = archivedPets
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> TransferOwnership(TransferOwnershipViewModel model)
        {
            var pet = await _context.Pets.FindAsync(model.PetId);
            if (pet == null) return NotFound();

            pet.NewOwnerId = model.NewOwnerId;
            pet.IsTransferPending = true;
            await _context.SaveChangesAsync();

            TempData["Message"] = "Ownership transfer request sent.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> TransferOwnershipForm(int id)
        {

            var users = await _context.Users
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = $"{u.Firstname} {u.LastName}" })
                .ToListAsync();

            var viewModel = new TransferOwnershipViewModel
            {
                PetId = id,
                Users = users
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AcceptTransfer(int petId)
        {

            var pet = await _context.Pets.FindAsync(petId);
            if (pet == null) return NotFound();

            if (pet.NewOwnerId == null || !pet.IsTransferPending)
                return BadRequest("No pending transfer request.");

            // Get the new owner's details from the database (assuming the new owner is the User)
            var newOwner = await _context.Users.FindAsync(pet.NewOwnerId);
            if (newOwner == null)
            {
                return NotFound("New owner not found");
            }

            // Set the PreviousOwnerName if the pet already has an owner
            if (!string.IsNullOrEmpty(pet.OwnerName))
            {
                pet.PreviousOwnerName = pet.OwnerName; // Save the current owner's name as the previous owner
            }

            pet.OwnerId = pet.NewOwnerId.Value;
            pet.NewOwnerId = null;
            pet.IsTransferPending = false;
            // Update owner details for the pet
            pet.OwnerName = newOwner.Firstname; // Assuming `Name` is the field storing the owner's name
            pet.OwnerContact = newOwner.ContactNumber; // Assuming `ContactNumber` is the field storing the owner's contact number
            pet.IsTransferred = true; // Mark the pet as transferred



            _context.Update(pet);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Ownership transfer accepted.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> DeclineTransfer(int petId)
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet == null) return NotFound();

            if (pet.NewOwnerId == null || !pet.IsTransferPending)
                return BadRequest("No pending transfer request.");

            pet.NewOwnerId = null;
            pet.IsTransferPending = false;

            await _context.SaveChangesAsync();

            TempData["Message"] = "Ownership transfer declined.";
            return RedirectToAction("Index");
        }

        public IActionResult VaccinationStats()
        {
            var barangay = HttpContext.Session.GetString("Barangay");

            if (string.IsNullOrEmpty(barangay))
            {
                return RedirectToAction("Index", "Home");
            }

            // Get vaccinated and unvaccinated dogs and cats in the logged-in user's barangay
            var vaccinatedDogs = _context.Pets
                .Where(p => p.PetBarangay == barangay && p.PetType == "Dog" && p.VaccinationStatus == "Vaccinated")
               .Select(p => p.Name)  // Assuming the dog has a 'Name' property
                .ToList();

            var unvaccinatedDogs = _context.Pets
                .Where(p => p.PetBarangay == barangay && p.PetType == "Dog" && p.VaccinationStatus == "Not Vaccinated")
                .Select(p => p.Name)  // Assuming the dog has a 'Name' property
                .ToList();

            var vaccinatedCats = _context.Pets
                .Where(p => p.PetBarangay == barangay && p.PetType == "Cat" && p.VaccinationStatus == "Vaccinated")
                .Select(p => p.Name)  // Assuming the dog has a 'Name' property
                .ToList();

            var unvaccinatedCats = _context.Pets
                .Where(p => p.PetBarangay == barangay && p.PetType == "Cat" && p.VaccinationStatus == "Not Vaccinated")
                .Select(p => p.Name)  // Assuming the dog has a 'Name' property
                .ToList();

            // Pass Barangay and stats to the ViewData and Model
            ViewData["Barangay"] = barangay;

            // Prepare the data for the chart
            var vaccinationStats = new
            {
                Barangay = barangay,
                VaccinatedDogs = vaccinatedDogs.Count(),
                VaccinatedCats = vaccinatedCats.Count(),
                VaccinatedDogsList = vaccinatedDogs,
                VaccinatedCatsList = vaccinatedCats,
                UnvaccinatedDogs = unvaccinatedDogs.Count(),
                UnvaccinatedCats = unvaccinatedCats.Count(),
                UnvaccinatedDogsList = unvaccinatedDogs,
                UnvaccinatedCatsList = unvaccinatedCats
            };

            return View(vaccinationStats);
        }




    }
}
