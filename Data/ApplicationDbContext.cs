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
        public DbSet<Breed> Breeds { get; set; }
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
        public DbSet<AvailableDateService> AvailableDateServices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<VaccinationStatusPvet> VaccinationStatusPvet { get; set; }
        public DbSet<VaccinationHistory> VaccinationHistories { get; set; }
        public DbSet<RabiesIncident> RabiesIncidents { get; set; }
        public DbSet<TransferRequest> TransferRequests { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<PenaltyFine> PenaltyFines { get; set; }
        public DbSet<PenaltyAssignment> PenaltyAssignments { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<ServiceInventoryItem> ServiceInventoryItems { get; set; }
        public DbSet<ServicePetType> ServicePetTypes { get; set; }
        public DbSet<AdoptionRequest> AdoptionRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Setting up the relationship between AvailableDate and AvailableDateService
            modelBuilder.Entity<AvailableDateService>()
                .HasKey(ads => new { ads.AvailableDateId, ads.ServiceId });

            modelBuilder.Entity<AvailableDateService>()
                .HasOne(ads => ads.AvailableDate)
                .WithMany(ad => ad.AvailableDateServices)
                .HasForeignKey(ads => ads.AvailableDateId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: Delete associated records when the date is deleted

            modelBuilder.Entity<AvailableDateService>()
                .HasOne(ads => ads.Service)
                .WithMany(s => s.AvailableDateServices)
                .HasForeignKey(ads => ads.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Breed>()
                .HasOne(b => b.PetType)
                .WithMany(p => p.Breeds)
                .HasForeignKey(b => b.TypeId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: Set delete behavior

            modelBuilder.Entity<PenaltyAssignment>()
            .HasOne(pa => pa.User)
            .WithMany(u => u.PenaltyAssignments)
            .HasForeignKey(pa => pa.UserId);

            modelBuilder.Entity<PenaltyAssignment>()
                .HasOne(pa => pa.PenaltyFine)
                .WithMany()
                .HasForeignKey(pa => pa.PenaltyFineId);

            modelBuilder.Entity<ServiceInventoryItem>()
           .HasKey(s => new { s.ServiceId, s.InventoryItemId });

            modelBuilder.Entity<ServiceInventoryItem>()
                .HasOne(s => s.Service)
                .WithMany(si => si.ServiceInventoryItems)
                .HasForeignKey(s => s.ServiceId);

            modelBuilder.Entity<ServiceInventoryItem>()
                .HasOne(i => i.InventoryItem)
                .WithMany(ii => ii.ServiceInventoryItems)
                .HasForeignKey(i => i.InventoryItemId);

            // Many-to-Many Relationship: Service - PetType
            modelBuilder.Entity<ServicePetType>()
                .HasKey(sp => new { sp.ServiceId, sp.PetTypeId });

            modelBuilder.Entity<ServicePetType>()
                .HasOne(sp => sp.Service)
                .WithMany(s => s.ServicePetTypes)
                .HasForeignKey(sp => sp.ServiceId);

            modelBuilder.Entity<ServicePetType>()
                .HasOne(sp => sp.PetType)
                .WithMany(pt => pt.ServicePetTypes)
                .HasForeignKey(sp => sp.PetTypeId);

            modelBuilder.Entity<AdoptionRequest>()
                .HasOne(ar => ar.User)
                .WithMany(u => u.AdoptionRequests)
                .HasForeignKey(ar => ar.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AdoptionRequest>()
                .HasOne(ar => ar.StrayReport)
                .WithMany(sr => sr.AdoptionRequests)
                .HasForeignKey(ar => ar.StrayReportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RabiesIncident>()
            .HasOne(ri => ri.Pet)
            .WithMany(p => p.RabiesIncidents)
            .HasForeignKey(ri => ri.PetId);

        }

    }


}
