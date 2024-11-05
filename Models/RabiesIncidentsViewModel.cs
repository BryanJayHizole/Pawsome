namespace Pawsome.Models
{
    public class RabiesIncidentsViewModel
    {
        public IEnumerable<RabiesIncident> RabiesIncidents { get; set; } = new List<RabiesIncident>(); // Initialize to avoid null reference
        public IEnumerable<City> Cities { get; set; } = new List<City>(); // Initialize to avoid null reference
        public IEnumerable<Barangay> Barangays { get; set; } = new List<Barangay>(); // Initialize to avoid null reference
    }
}
