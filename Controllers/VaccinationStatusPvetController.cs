using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Pawsome.Data;
using Pawsome.Models;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Pawsome.Controllers
{
    public class VaccinationStatusPvetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConverter _converter;

        public VaccinationStatusPvetController(ApplicationDbContext context, IConverter converter)
        {
            _context = context;
            _converter = converter;
        }





        public IActionResult PetCountsByCity()
        {
            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");
            if (isPvetAdmin != "True")
            {
                return RedirectToAction("Index", "Home");
            }

            // Fetch pet counts for each city
            var petCounts = _context.Cities
                .Select(city => new VaccinationStatusPvet
                {
                    Municipality = city.CityName,
                    DogsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog"   && p.IsArchived == false),
                    CatsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat"   && p.IsArchived == false),
                    VaccinatedDogsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog" && p.VaccinationStatus == "Vaccinated"   && p.IsArchived == false),
                    VaccinatedCatsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat" && p.VaccinationStatus == "Vaccinated"   && p.IsArchived == false),
                    VaccinatedDogsPercentage = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog" && p.VaccinationStatus == "Vaccinated"   && p.IsArchived == false)
                        * 100.0 /
                        (_context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog"  ) == 0 ? 1 : _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog"  )),
                    VaccinatedCatsPercentage = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat" && p.VaccinationStatus == "Vaccinated"   && p.IsArchived == false)
                        * 100.0 /
                        (_context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat"   && p.IsArchived == false) == 0 ? 1 : _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat"  )),
                    CapturedDogsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Captured" && sr.StrayType == "Dog"),
                    CapturedCatsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Captured" && sr.StrayType == "Cat"),
                    EuthanizedDogsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Euthanized" && sr.StrayType == "Dog"),
                    EuthanizedCatsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Euthanized" && sr.StrayType == "Cat"),
                    VerifiedRabiesIncidentsCount = _context.RabiesIncidents.Count(ri => ri.City == city.CityName && ri.IsVerified)
                })
                .ToList();

            // Fetch Rabies Risk data for each barangay
            var rabiesRisk = _context.Barangays
                .Select(barangay => new RabiesRiskViewModel
                {
                    Barangay = barangay.BarangayName,
                    Municipality = barangay.City.CityName,
                    BarangayDogsCount = _context.Pets.Count(p => p.PetBarangay == barangay.BarangayName && p.PetType == "Dog"   && p.IsArchived == false),
                    BarangayCatsCount = _context.Pets.Count(p => p.PetBarangay == barangay.BarangayName && p.PetType == "Cat"   && p.IsArchived == false),
                    BarangayVaccinatedDogsCount = _context.Pets.Count(p => p.PetBarangay == barangay.BarangayName && p.PetType == "Dog" && p.VaccinationStatus == "Vaccinated"   && p.IsArchived == false),
                    BarangayVaccinatedCatsCount = _context.Pets.Count(p => p.PetBarangay == barangay.BarangayName && p.PetType == "Cat" && p.VaccinationStatus == "Vaccinated"   && p.IsArchived == false),
                    BarangayVerifiedRabiesIncidentsCount = _context.RabiesIncidents.Count(ri => ri.Barangay == barangay.BarangayName && ri.IsVerified),
                    BarangayVaccinatedDogsPercentage = _context.Pets.Count(p => p.PetBarangay == barangay.BarangayName && p.PetType == "Dog" && p.VaccinationStatus == "Vaccinated"   && p.IsArchived == false)
                        * 100.0 /
                        (_context.Pets.Count(p => p.PetBarangay == barangay.BarangayName && p.PetType == "Dog"  ) == 0 ? 1 : _context.Pets.Count(p => p.PetBarangay == barangay.BarangayName && p.PetType == "Dog"  )),
                    BarangayVaccinatedCatsPercentage = _context.Pets.Count(p => p.PetBarangay == barangay.BarangayName && p.PetType == "Cat" && p.VaccinationStatus == "Vaccinated"   && p.IsArchived == false)
                        * 100.0 /
                        (_context.Pets.Count(p => p.PetBarangay == barangay.BarangayName && p.PetType == "Cat"  ) == 0 ? 1 : _context.Pets.Count(p => p.PetBarangay == barangay.BarangayName && p.PetType == "Cat"  )),

               
                })
                .ToList();

            // Create a model that includes both petCounts and rabiesRisk
            var model = new ReportsViewModel
            {
                PetCounts = petCounts,
                RabiesRisk = rabiesRisk
            };

            // Prepare data for high-risk chart
            var months = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            // Prepare data for vaccination counts by month
            var monthlyVaccinationCounts = months.Select(m => new {
                Month = m,
                Count = _context.Pets
    .Where(p => p.VaccinationDate.HasValue &&
                p.VaccinationDate.Value.Month == months.IndexOf(m) + 1 &&
                p.IsArchived == false)
    .Count(),
                UnvaccinatedCount = _context.Pets
        .Where(p => !p.VaccinationDate.HasValue &&
                    p.IsArchived == false)
        .Count()
            }).ToList();

            // Prepare data for rabies incident counts by month
            var monthlyRabiesIncidentCounts = months.Select(m => new {
                Month = m,
                Count = _context.RabiesIncidents
         .Where(r => r.DateVerified.HasValue &&
                     r.DateVerified.Value.Month == months.IndexOf(m) + 1)
         .Count()
            }).ToList();


            // Pass data to the view using ViewData
            ViewData["MonthlyVaccinationCounts"] = JsonConvert.SerializeObject(monthlyVaccinationCounts);
            ViewData["MonthlyRabiesIncidentCounts"] = JsonConvert.SerializeObject(monthlyRabiesIncidentCounts);

            return View(model);
        }


        public IActionResult DownloadPetCountsReport()
        {
            var petCounts = _context.Cities
                .Select(city => new VaccinationStatusPvet
                {
                    Municipality = city.CityName,
                    DogsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog"  && p.IsArchived == false),
                    CatsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat"  && p.IsArchived == false),
                    VaccinatedDogsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog" && p.VaccinationStatus == "Vaccinated"  && p.IsArchived == false),
                    VaccinatedCatsCount = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat" && p.VaccinationStatus == "Vaccinated"  && p.IsArchived == false),
                    VaccinatedDogsPercentage = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog" && p.VaccinationStatus == "Vaccinated" && p.IsArchived == false)
                    * 100.0 /
                                                  (_context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog"  ) == 0 ? 1 : _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Dog"  )),
                    VaccinatedCatsPercentage = _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat" && p.VaccinationStatus == "Vaccinated"   && p.IsArchived == false)
                    * 100.0 /
                                                  (_context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat"   && p.IsArchived == false) == 0 ? 1 : _context.Pets.Count(p => p.PetCity == city.CityName && p.PetType == "Cat"  )),
                    CapturedDogsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Captured" && sr.StrayType == "Dog"),
                    CapturedCatsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Captured" && sr.StrayType == "Cat"),
                    EuthanizedDogsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Euthanized" && sr.StrayType == "Dog"),
                    EuthanizedCatsCount = _context.StrayReports.Count(sr => sr.City == city.CityName && sr.Status == "Euthanized" && sr.StrayType == "Cat"),
                    VerifiedRabiesIncidentsCount = _context.RabiesIncidents.Count(ri => ri.City == city.CityName && ri.IsVerified)
                })
                .ToList();

            var html = GenerateHtmlForPetCounts(petCounts);

            var pdfDocument = new HtmlToPdfDocument
            {
                GlobalSettings = {
            DocumentTitle = "Pet Counts Report",
            PaperSize = PaperKind.A4,
            Orientation = Orientation.Landscape // Set orientation to landscape
        },
                Objects = {
            new ObjectSettings
            {
                HtmlContent = html,
                WebSettings = { DefaultEncoding = "utf-8" }
            }
        }
            };

            var pdf = _converter.Convert(pdfDocument);

            return File(pdf, "application/pdf", "PetCountsReport.pdf");
        }


        private string GenerateHtmlForPetCounts(IEnumerable<VaccinationStatusPvet> petCounts)
        {
            var sb = new StringBuilder();
            sb.Append("<html><body>");
            sb.Append("<h1>Pet Counts by City</h1>");
            sb.Append("<table border='1' cellpadding='5' cellspacing='0'><thead><tr>");
            sb.Append("<th>Municipality</th><th>Number of Dogs</th><th>Number of Cats</th><th>Vaccinated Dogs</th><th>Vaccinated Cats</th><th>Percentage of Vaccinated Dogs (%)</th><th>Percentage of Vaccinated Cats (%)</th><th>No. of Captured Dogs</th><th>No. of Captured Cats</th><th>No. of Euthanized Dogs</th><th>No. of Euthanized Cats</th><th>Verified Rabies Incidents</th>");
            sb.Append("</tr></thead><tbody>");

            foreach (var item in petCounts)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{item.Municipality}</td>");
                sb.Append($"<td>{item.DogsCount}</td>");
                sb.Append($"<td>{item.CatsCount}</td>");
                sb.Append($"<td>{item.VaccinatedDogsCount}</td>");
                sb.Append($"<td>{item.VaccinatedCatsCount}</td>");
                sb.Append($"<td>{item.VaccinatedDogsPercentage:F2}</td>");
                sb.Append($"<td>{item.VaccinatedCatsPercentage:F2}</td>");
                sb.Append($"<td>{item.CapturedDogsCount}</td>");
                sb.Append($"<td>{item.CapturedCatsCount}</td>");
                sb.Append($"<td>{item.EuthanizedDogsCount}</td>");
                sb.Append($"<td>{item.EuthanizedCatsCount}</td>");
                sb.Append($"<td>{item.VerifiedRabiesIncidentsCount}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody>");

            // Calculate totals
            var totalDogs = petCounts.Sum(x => x.DogsCount);
            var totalCats = petCounts.Sum(x => x.CatsCount);
            var totalVaccinatedDogs = petCounts.Sum(x => x.VaccinatedDogsCount);
            var totalVaccinatedCats = petCounts.Sum(x => x.VaccinatedCatsCount);
            var totalCapturedDogs = petCounts.Sum(x => x.CapturedDogsCount);
            var totalCapturedCats = petCounts.Sum(x => x.CapturedCatsCount);
            var totalEuthanizedDogs = petCounts.Sum(x => x.EuthanizedDogsCount);
            var totalEuthanizedCats = petCounts.Sum(x => x.EuthanizedCatsCount);
            var totalVerifiedRabiesIncidents = petCounts.Sum(x => x.VerifiedRabiesIncidentsCount);
            var totalVaccinatedDogsPercentage = totalDogs > 0 ? (double)totalVaccinatedDogs / totalDogs * 100 : 0;
            var totalVaccinatedCatsPercentage = totalCats > 0 ? (double)totalVaccinatedCats / totalCats * 100 : 0;

            // Adding the totals row
            sb.Append("<tfoot><tr><td><strong>Total</strong></td>");
            sb.Append($"<td>{totalDogs}</td>");
            sb.Append($"<td>{totalCats}</td>");
            sb.Append($"<td>{totalVaccinatedDogs}</td>");
            sb.Append($"<td>{totalVaccinatedCats}</td>");
            sb.Append($"<td>{totalVaccinatedDogsPercentage:F2}</td>");
            sb.Append($"<td>{totalVaccinatedCatsPercentage:F2}</td>");
            sb.Append($"<td>{totalCapturedDogs}</td>");
            sb.Append($"<td>{totalCapturedCats}</td>");
            sb.Append($"<td>{totalEuthanizedDogs}</td>");
            sb.Append($"<td>{totalEuthanizedCats}</td>");
            sb.Append($"<td>{totalVerifiedRabiesIncidents}</td>");
            sb.Append("</tr></tfoot>");

            sb.Append("</table>");
            sb.Append("</body></html>");
            return sb.ToString();
        }

    }
}
