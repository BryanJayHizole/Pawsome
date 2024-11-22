using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pawsome.Models
{
    public class TransferOwnershipViewModel
    {
        public int PetId { get; set; }
        public List<SelectListItem> Users { get; set; } // Populate with users from the database
        public int NewOwnerId { get; set; } // Selected new owner's ID
    }

}
