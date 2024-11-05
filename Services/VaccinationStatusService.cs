using System.Collections.Generic;
using System.Linq;
using Pawsome.Models;
using Pawsome.Data; // Assuming you have a data context or repository for accessing the database

namespace Pawsome.Services
{
    public class VaccinationStatusService : IVaccinationStatusService
    {
        private readonly ApplicationDbContext _context;

        public VaccinationStatusService(ApplicationDbContext context)
        {
            _context = context;
        }


        public IEnumerable<VaccinationStatusPvet> GetVaccinationStatusData()
        {
            var petCounts = _context.Cities
                .Select(city => new VaccinationStatusPvet
                {
                    Municipality = city.CityName,
                    DogsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog" && p.IsVerified && p.IsArchived == false),
                    CatsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat" && p.IsVerified && p.IsArchived == false),
                    VaccinatedDogsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog" && p.VaccinationStatus == "Vaccinated" && p.IsVerified && p.IsArchived == false),
                    VaccinatedCatsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat" && p.VaccinationStatus == "Vaccinated" && p.IsVerified && p.IsArchived == false),
                    VaccinatedDogsPercentage = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog" && p.VaccinationStatus == "Vaccinated" && p.IsVerified && p.IsArchived == false)
                    * 100.0 /
                                              (_context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog" && p.IsVerified) == 0 ? 1 : _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog" && p.IsVerified)),
                    VaccinatedCatsPercentage = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat" && p.VaccinationStatus == "Vaccinated" && p.IsVerified && p.IsArchived == false)
                    * 100.0 /
                                              (_context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat" && p.IsVerified && p.IsArchived == false) == 0 ? 1 : _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat" && p.IsVerified)),
                    CapturedDogsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Captured" && sr.StrayType == "Dog"),
                    CapturedCatsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Captured" && sr.StrayType == "Cat"),
                    EuthanizedDogsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Euthanized" && sr.StrayType == "Dog"),
                    EuthanizedCatsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Euthanized" && sr.StrayType == "Cat"),
                    VerifiedRabiesIncidentsCount = _context.RabiesIncidents.Count(ri => ri.City == city.CityName && ri.IsVerified)
                })
                .ToList();

            return petCounts;
        }
    }
}
