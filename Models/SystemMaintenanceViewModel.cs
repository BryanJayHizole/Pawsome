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
        // New property to hold service details including inventory items and quantity used
        public List<ServiceDetail> ServiceDetails { get; set; } = new List<ServiceDetail>();




    }

    public class ServiceDetail
    {
        public string ServiceName { get; set; }
        public int ServiceId { get; set; } // Add this if it's missing
        public List<InventoryItemQuantity> RequiredItems { get; set; }
        public List<string> PetTypes { get; set; }
        public string GenderAvailability { get; set; } // Add this
    }

    public class InventoryItemQuantity
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string VSource { get; set; } // Add this line for VSource
    }


    public class InventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; } 
        public string? VSource { get; set; } 
        public int? VaccineSourceId { get; set; }
        public VaccineSource VaccineSource { get; set; }
        public DateTime? ExpirationDate { get; set; } // New field
        public ICollection<ServiceInventoryItem> ServiceInventoryItems { get; set; }
        public bool Consumable { get; set; }
    }

    public class PenaltyFine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal FineAmount { get; set; }
        public string BarangayName { get; set; } // Add this property
    }

}
