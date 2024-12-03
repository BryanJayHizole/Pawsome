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

            var pettypes = _context.PetTypes.Select(c => new SelectListItem { Value = c.TypeId.ToString(), Text = c.PType }).ToList();
            ViewBag.PetTypes = pettypes;
            var breeds = _context.Breeds.Select(c => new SelectListItem { Value = c.BreedId.ToString(), Text = c.BreedType }).ToList();
            ViewBag.Breeds = breeds;
            var tagtypes = _context.TagTypes.Select(c => new SelectListItem { Value = c.TagId.ToString(), Text = c.TagTypeName }).ToList();
            ViewBag.TagTypes = tagtypes;
            var vaccinationstatuses = _context.VaccinationStatuses.Select(c => new SelectListItem { Value = c.VStatusId.ToString(), Text = c.VStatusType }).ToList();
            ViewBag.VaccinationStatuses = vaccinationstatuses;
            var vaccinetypes = _context.VaccineTypes.Select(c => new SelectListItem { Value = c.VTypeId.ToString(), Text = c.VType }).ToList();
            ViewBag.VaccineTypes = vaccinetypes;
          

            ViewBag.Pets = _context.Pets.Where(p => p.IsArchived == false).ToList();

            return View(new RabiesIncident());
        }

        [HttpPost]
        public async Task<IActionResult> ReportIncident(RabiesIncident incident, IFormFile photoFile, Barangay barangy, City municipality, PetType pettyp, Breed brid, TagType tagtyp, VaccinationStatus vstats, VaccineType vtyp)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
           

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
            var pettype = _context.PetTypes.SingleOrDefault(c => c.TypeId == pettyp.TypeId);
            var breed = _context.Breeds.SingleOrDefault(c => c.BreedId == brid.BreedId);
            var tagtype = _context.TagTypes.SingleOrDefault(c => c.TagId == tagtyp.TagId);
            var vstatus = _context.VaccinationStatuses.SingleOrDefault(c => c.VStatusId == vstats.VStatusId);
            var vtype = _context.VaccineTypes.SingleOrDefault(c => c.VTypeId == vtyp.VTypeId);
        

            incident.AnimalType = pettype?.PType;
            incident.Breed = breed?.BreedType; 
            incident.Tag = tagtype?.TagTypeName;
            incident.VaccinationStatus = vstatus?.VStatusType;
            incident.VaccineType = vtype?.VType;

            
            if (incident.PetId.HasValue)
            {
                // If the user selected an existing pet
                var pet = await _context.Pets.FindAsync(incident.PetId);
                if (pet != null)
                {
                    incident.IsStray = false;
                    incident.OwnerId = pet.OwnerId;
                    incident.OwnerName = pet.OwnerName;
                    incident.OwnerContact = pet.OwnerContact;
                    incident.PetName = pet.Name;
                    incident.Gender = pet.Gender;
                    incident.Color = pet.Color;
                    incident.AnimalType = pet.PetType; // Use existing type from the pet
                    incident.Breed = pet.Breed; // Use existing breed from the pet
                    incident.Tag = pet.TagType; // Use existing tag from the pet
                    incident.VaccinationStatus = pet.VaccinationStatus;
                    incident.VaccineType = pet.VaccineType;
                    incident.VaccinationDate = pet.VaccinationDate;
                    incident.NextDueDate = pet.NextDueDate;
                    incident.VaccineBlockNo = pet.VaccineBlockNo;
                    incident.VaccinatedBy = pet.VaccinatedBy;
                    incident.PetPhoto = pet.Photo;       
                }
            }


            // If the IncidentCase is "No", set IncidentType to null
            if (incident.IncidentCase == "No")
            {
                incident.TypeofIncident = null;
            }

            // If the RabiesCase is "No", set Symptoms to null
            if (incident.RabiesCase == "No")
            {
                incident.Symptoms = null;
            }

            // Set the names in the user object
            incident.City = city?.CityName;
            incident.Barangay = barangay?.BarangayName;

            incident.ReportedByUser = user;
            incident.ReportedByUserId = user.Id;
            incident.DateReported = DateTime.Now;
            incident.IsVerified = false;


            await _context.RabiesIncidents.AddAsync(incident);
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

        // Barangay Admin Verifies Rabies Incident
        [HttpPost]
        public IActionResult VerifyIncidentByBarangay(int id)
        {
            var incident = _context.RabiesIncidents.FirstOrDefault(i => i.RabiesIncidentId == id);
            if (incident != null && !incident.IsVerifiedbyBarangayAdmin)
            {
                incident.IsVerifiedbyBarangayAdmin = true;
              
                _context.SaveChanges();
            }
            return RedirectToAction("VerifyIncidents");
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
            if (incident != null && incident.IsVerifiedbyBarangayAdmin && !incident.IsVerified)
            {
                incident.IsVerified = true;
                incident.DateVerified = DateTime.Now;
                incident.VerifiedByAdminId = (await _context.Users.SingleOrDefaultAsync(u => u.Email == HttpContext.Session.GetString("Email"))).Id;
                _context.SaveChanges();
            }

          
            
        

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
