using Microsoft.EntityFrameworkCore;
using Pawsome.Models;
using System.Diagnostics.Metrics;

namespace Pawsome.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<PetType> PetTypes { get; set; }
        public DbSet<Habitat> Habitats { get; set; }
        public DbSet<TagType> TagTypes { get; set; }
        public DbSet<VaccinationStatus> VaccinationStatuses { get; set; }
        public DbSet<VaccineType> VaccineTypes { get; set; }
        public DbSet<VaccineSource> VaccineSources { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Barangay> Barangays { get; set; }
        public DbSet<LostPetReport> LostPetReports { get; set; } 
        public DbSet<StrayReport> StrayReports { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentType> AppointmentTypes { get; set; }
        public DbSet<AvailableDate> AvailableDates { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<VaccinationStatusPvet> VaccinationStatusPvet { get; set; }
        public DbSet<VaccinationHistory> VaccinationHistories { get; set; } 
        public DbSet<RabiesIncident> RabiesIncidents { get; set; }
        public DbSet<TransferRequest> TransferRequests { get; set; }
    }


}
