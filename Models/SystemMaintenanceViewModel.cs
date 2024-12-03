using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Pawsome.Models
{
    public class SystemMaintenanceViewModel
    {
        public List<SelectListItem> PetTypes { get; set; }
        public List<SelectListItem> Breeds { get; set; }
        public List<SelectListItem> Habitats { get; set; }
        public List<SelectListItem> TagTypes { get; set; }
        public List<SelectListItem> VaccineTypes { get; set; }
        public List<SelectListItem> VaccineSources { get; set; }
        public List<SelectListItem> Services { get; set; }
        public List<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        public List<PenaltyFine> PenaltyFines { get; set; } = new List<PenaltyFine>();
        public int CapturedLimit { get; set; }
        public int EuthanasiaDays { get; set; }
        public string BarangayName { get; set; }
        public IFormFile Document { get; set; }
        

    }

  

    public class InventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
      
    }

    public class PenaltyFine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal FineAmount { get; set; }
    }

}
