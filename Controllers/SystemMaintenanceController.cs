using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Pawsome.Data;
using Pawsome.Models;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Pawsome.Controllers
{
    public class SystemMaintenanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public SystemMaintenanceController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // ViewModel to pass data to the view
        public IActionResult Index()
        {
            var viewModel = new SystemMaintenanceViewModel
            {
                PetTypes = _context.PetTypes.Select(c => new SelectListItem
                {
                    Value = c.TypeId.ToString(),
                    Text = c.PType
                }).ToList(),

                Breeds = _context.Breeds.Select(c => new SelectListItem
                {
                    Value = c.BreedId.ToString(),
                    Text = c.BreedType
                }).ToList(),

                Habitats = _context.Habitats.Select(c => new SelectListItem
                {
                    Value = c.HabitatId.ToString(),
                    Text = c.HabitatType
                }).ToList(),

                TagTypes = _context.TagTypes.Select(c => new SelectListItem
                {
                    Value = c.TagId.ToString(),
                    Text = c.TagTypeName
                }).ToList(),

                VaccineTypes = _context.VaccineTypes.Select(c => new SelectListItem
                {
                    Value = c.VTypeId.ToString(),
                    Text = c.VType
                }).ToList(),

                VaccineSources = _context.VaccineSources.Select(c => new SelectListItem
                {
                    Value = c.VSourceId.ToString(),
                    Text = c.VSource
                }).ToList(),

                Services = _context.Services.Select(c => new SelectListItem
                {
                    Value = c.ServiceId.ToString(),
                    Text = c.ServiceName
                }).ToList(),

                // Add Inventory Items to the ViewModel
                InventoryItems = _context.InventoryItems.Select(c => new InventoryItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    Quantity = c.Quantity
                }).ToList(),

                 // Add Inventory Items to the ViewModel
                PenaltyFines = _context.PenaltyFines.Select(c => new PenaltyFine
                {
                    Id = c.Id,
                    Name = c.Name,
                    FineAmount = c.FineAmount
                }).ToList()


            };
         

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(SystemMaintenanceViewModel viewModel)
        {
            if (viewModel.Document != null && viewModel.Document.Length > 0)
            {
                // Set the path where the file will be stored
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", viewModel.Document.FileName);

                // Create a directory if it doesn't exist
                if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "uploads")))
                {
                    Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "uploads"));
                }

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.Document.CopyToAsync(stream);
                }

                TempData["SuccessMessage"] = "Document uploaded successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "No document selected.";
            }



            return RedirectToAction("Index");
        }

        // This action will render the view to display uploaded documents
        public IActionResult ViewDocuments(string filename)
        {
            var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var files = Directory.GetFiles(directoryPath);

            var fileNames = new List<string>();
            foreach (var file in files)
            {
                fileNames.Add(Path.GetFileName(file));
            }

            return View(fileNames); // Pass file names to the view
        }

        // Action to add a new Pet Type
        [HttpPost]
        public IActionResult AddPetType(string petType)
        {
            if (string.IsNullOrEmpty(petType))
            {
                ModelState.AddModelError("petType", "Pet type cannot be empty.");
                return RedirectToAction("Index");
            }

            var newPetType = new PetType { PType = petType };
            _context.PetTypes.Add(newPetType);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Pet Type added successfully!";
            return RedirectToAction("Index");
        }

        // Action to edit a Pet Type
        [HttpPost]
        public IActionResult EditPetType(int id, string petType)
        {
            if (string.IsNullOrEmpty(petType))
            {
                TempData["ErrorMessage"] = "Pet Type cannot be empty.";
                return RedirectToAction("Index");
            }

            var petTypeEntity = _context.PetTypes.FirstOrDefault(pt => pt.TypeId == id);
            if (petTypeEntity == null)
            {
                TempData["ErrorMessage"] = "Pet Type not found.";
                return RedirectToAction("Index");
            }

            petTypeEntity.PType = petType;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Pet Type updated successfully!";
            return RedirectToAction("Index");
        }

        // Action to delete a Pet Type
        [HttpPost]
        public IActionResult DeletePetType(int id)
        {
            var petTypeEntity = _context.PetTypes.FirstOrDefault(pt => pt.TypeId == id);
            if (petTypeEntity == null)
            {
                TempData["ErrorMessage"] = "Pet Type not found.";
                return RedirectToAction("Index");
            }

            _context.PetTypes.Remove(petTypeEntity);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Pet Type deleted successfully!";
            return RedirectToAction("Index");
        }

        // Action to add a new Breed
        [HttpPost]
        public IActionResult AddBreed(string breed, int typeId)
        {
            if (string.IsNullOrEmpty(breed) || typeId <= 0)
            {
                ModelState.AddModelError("breed", "Breed name and type must be valid.");
                return RedirectToAction("Index");
            }

            var newBreed = new Breed { BreedType = breed, TypeId = typeId };
            _context.Breeds.Add(newBreed);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Breed added successfully!";
            return RedirectToAction("Index");
        }

        // Similarly, implement Edit and Delete for Breeds
        [HttpPost]
        public IActionResult EditBreed(int id, string breed, int typeId)
        {
            if (string.IsNullOrEmpty(breed) || typeId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Breed or Type.";
                return RedirectToAction("Index");
            }

            var breedEntity = _context.Breeds.FirstOrDefault(b => b.BreedId == id);
            if (breedEntity == null)
            {
                TempData["ErrorMessage"] = "Breed not found.";
                return RedirectToAction("Index");
            }

            breedEntity.BreedType = breed;
            breedEntity.TypeId = typeId;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Breed updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteBreed(int id)
        {
            var breedEntity = _context.Breeds.FirstOrDefault(b => b.BreedId == id);
            if (breedEntity == null)
            {
                TempData["ErrorMessage"] = "Breed not found.";
                return RedirectToAction("Index");
            }

            _context.Breeds.Remove(breedEntity);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Breed deleted successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddHabitat(string habitat)
        {
            if (string.IsNullOrEmpty(habitat))
            {
                ModelState.AddModelError("habitat", "Habitat cannot be empty.");
                return RedirectToAction("Index");
            }

            var newHabitat = new Habitat { HabitatType = habitat };
            _context.Habitats.Add(newHabitat);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Habitat added successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditHabitat(int id, string habitatName)
        {
            if (string.IsNullOrEmpty(habitatName))
            {
                TempData["ErrorMessage"] = "Habitat Name cannot be empty.";
                return RedirectToAction("Index");
            }

            var habitat = _context.Habitats.FirstOrDefault(h => h.HabitatId == id);
            if (habitat == null)
            {
                TempData["ErrorMessage"] = "Habitat not found.";
                return RedirectToAction("Index");
            }

            habitat.HabitatType = habitatName;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Habitat updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteHabitat(int id)
        {
            var habitat = _context.Habitats.FirstOrDefault(h => h.HabitatId == id);
            if (habitat == null)
            {
                TempData["ErrorMessage"] = "Habitat not found.";
                return RedirectToAction("Index");
            }

            _context.Habitats.Remove(habitat);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Habitat deleted successfully!";
            return RedirectToAction("Index");
        }


        // Action to add a new Tag
        [HttpPost]
        public IActionResult AddTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                ModelState.AddModelError("tag", "Tag cannot be empty.");
                return RedirectToAction("Index");
            }

            var newTag = new TagType { TagTypeName = tag };
            _context.TagTypes.Add(newTag);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Tag added successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditTag(int id, string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                TempData["ErrorMessage"] = "Tag Name cannot be empty.";
                return RedirectToAction("Index");
            }

            var tag = _context.TagTypes.FirstOrDefault(t => t.TagId == id);
            if (tag == null)
            {
                TempData["ErrorMessage"] = "Tag not found.";
                return RedirectToAction("Index");
            }

            tag.TagTypeName = tagName;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Tag updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteTag(int id)
        {
            var tag = _context.TagTypes.FirstOrDefault(t => t.TagId == id);
            if (tag == null)
            {
                TempData["ErrorMessage"] = "Tag not found.";
                return RedirectToAction("Index");
            }

            _context.TagTypes.Remove(tag);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Tag deleted successfully!";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult AddVaccineType(string vaccineType)
        {
            if (string.IsNullOrEmpty(vaccineType))
            {
                ModelState.AddModelError("vaccineType", "Vaccine Type cannot be empty.");
                return RedirectToAction("Index");
            }

            var newVaccineType = new VaccineType { VType = vaccineType };
            _context.VaccineTypes.Add(newVaccineType);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Vaccine Type added successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditVaccineType(int id, string vaccineTypeName)
        {
            if (string.IsNullOrEmpty(vaccineTypeName))
            {
                TempData["ErrorMessage"] = "Vaccine Type Name cannot be empty.";
                return RedirectToAction("Index");
            }

            var vaccineType = _context.VaccineTypes.FirstOrDefault(v => v.VTypeId == id);
            if (vaccineType == null)
            {
                TempData["ErrorMessage"] = "Vaccine Type not found.";
                return RedirectToAction("Index");
            }

            vaccineType.VType = vaccineTypeName;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Vaccine Type updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteVaccineType(int id)
        {
            var vaccineType = _context.VaccineTypes.FirstOrDefault(v => v.VTypeId == id);
            if (vaccineType == null)
            {
                TempData["ErrorMessage"] = "Vaccine Type not found.";
                return RedirectToAction("Index");
            }

            _context.VaccineTypes.Remove(vaccineType);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Vaccine Type deleted successfully!";
            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult AddVaccineSource(string vaccineSource)
        {
            if (string.IsNullOrEmpty(vaccineSource))
            {
                ModelState.AddModelError("vaccineSource", "Vaccine Source cannot be empty.");
                return RedirectToAction("Index");
            }

            var newVaccineSource = new VaccineSource { VSource = vaccineSource };
            _context.VaccineSources.Add(newVaccineSource);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Vaccine Source added successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditVaccineSource(int id, string sourceName)
        {
            if (string.IsNullOrEmpty(sourceName))
            {
                TempData["ErrorMessage"] = "Source Name cannot be empty.";
                return RedirectToAction("Index");
            }

            var source = _context.VaccineSources.FirstOrDefault(v => v.VSourceId == id);
            if (source == null)
            {
                TempData["ErrorMessage"] = "Source not found.";
                return RedirectToAction("Index");
            }

            source.VSource = sourceName;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Source updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteVaccineSource(int id)
        {
            var source = _context.VaccineSources.FirstOrDefault(v => v.VSourceId == id);
            if (source == null)
            {
                TempData["ErrorMessage"] = "Source not found.";
                return RedirectToAction("Index");
            }

            _context.VaccineSources.Remove(source);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Source deleted successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddService(string service)
        {
            if (string.IsNullOrEmpty(service))
            {
                ModelState.AddModelError("service", "Service cannot be empty.");
                return RedirectToAction("Index");
            }

            var newService = new Service { ServiceName = service };
            _context.Services.Add(newService);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Service added successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditService(int id, string service)
        {
            if (string.IsNullOrEmpty(service))
            {
                TempData["ErrorMessage"] = "Service Name cannot be empty.";
                return RedirectToAction("Index");
            }

            var serviceEntity = _context.Services.FirstOrDefault(v => v.ServiceId == id);
            if (serviceEntity == null)
            {
                TempData["ErrorMessage"] = "Service not found.";
                return RedirectToAction("Index");
            }

            serviceEntity.ServiceName = service;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Service updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteService(int id)
        {
            var service = _context.Services.FirstOrDefault(v => v.ServiceId == id);
            if (service == null)
            {
                TempData["ErrorMessage"] = "Service not found.";
                return RedirectToAction("Index");
            }

            _context.Services.Remove(service);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "S deleted successfully!";
            return RedirectToAction("Index");
        }

        // Add a new inventory item
        [HttpPost]
        public IActionResult AddInventoryItem(string itemName, int quantity)
        {
            var inventoryItem = new InventoryItem
            {
                Name = itemName,
                Quantity = quantity
             
            };

            _context.InventoryItems.Add(inventoryItem);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Item added successfully!";
            return RedirectToAction("Index");
        }

        // Edit an inventory item
        [HttpPost]
        public IActionResult EditInventoryItem(int id, int quantity)
        {
            var item = _context.InventoryItems.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                item.Quantity = quantity;
            
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Item updated successfully!";
            }
            return RedirectToAction("Index");
        }

        // Delete an inventory item
        [HttpPost]
        public IActionResult DeleteInventoryItem(int id)
        {
            var item = _context.InventoryItems.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                _context.InventoryItems.Remove(item);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Item deleted successfully!";
            }
            return RedirectToAction("Index");
        }

        // Add a new inventory item
        [HttpPost]
        public IActionResult AddPenaltyFine(string penalty, decimal fine)
        {
            var penaltyFine = new PenaltyFine
            {
                Name = penalty,
                FineAmount = fine

            };

            _context.PenaltyFines.Add(penaltyFine);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Penalty and Fines added successfully!";
            return RedirectToAction("Index");
        }

        // Edit an inventory item
        [HttpPost]
        public IActionResult EditPenaltyFine(int id, decimal fines)
        {
            var fine = _context.PenaltyFines.FirstOrDefault(i => i.Id == id);
            if (fine != null)
            {
                fine.FineAmount = fines;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "FineAmount updated successfully!";
            }
            return RedirectToAction("Index");
        }

        // Delete an inventory item
        [HttpPost]
        public IActionResult DeletePenaltyFine(int id)
        {
            var fine = _context.PenaltyFines.FirstOrDefault(i => i.Id == id);
            if (fine != null)
            {
                _context.PenaltyFines.Remove(fine);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Penalty and Fines deleted successfully!";
            }
            return RedirectToAction("Index");
        }
    }
}
