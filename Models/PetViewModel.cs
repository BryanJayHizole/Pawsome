namespace Pawsome.Models
{
    public class PetViewModel
    {
        public IEnumerable<Pet> Pets { get; set; } = new List<Pet>(); // Initialize to avoid null reference
        public IEnumerable<City> Cities { get; set; } = new List<City>(); // Initialize to avoid null reference
        public IEnumerable<Barangay> Barangays { get; set; } = new List<Barangay>(); // Initialize to avoid null reference
        public IEnumerable<Pet> TransferRequests { get; set; } = new List<Pet>(); // Add this line to hold transfer requests
        public List<Pet> ArchivedPets { get; set; }
        public List<Pet> AllArchivedPets { get; set; }
        public List<Pet> ArchivedPetsByUser { get; set; }
        public List<Pet> ArchivedPetsByBarangay { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
