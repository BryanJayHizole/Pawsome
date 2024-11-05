using Microsoft.AspNetCore.Mvc;
using Pawsome.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Pawsome.Services;
using Microsoft.EntityFrameworkCore;
using Pawsome.Data;

using Microsoft.Extensions.Caching.Memory;

namespace Pawsome.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVaccinationStatusService _vaccinationStatusService;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
      
        public HomeController(ILogger<HomeController> logger, IVaccinationStatusService vaccinationStatusService, ApplicationDbContext context, IMemoryCache cache)
        {
            _logger = logger;
            _vaccinationStatusService = vaccinationStatusService;
           
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            // Check if the user is not logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Email")))
            {
                var vaccinationData = _vaccinationStatusService.GetVaccinationStatusData(); // Make this async if possible
                return View("VaccinationGraphs", vaccinationData); // Ensure you have this view created
            }

            var currentUserEmail = HttpContext.Session.GetString("Email");
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == currentUserEmail);

            // Use caching for announcements to reduce database calls
            const string cacheKey = "announcementsCacheKey";
            if (!_cache.TryGetValue(cacheKey, out List<Announcement> announcements))
            {
                announcements = await _context.Announcements
                                              
                                              .OrderByDescending(a => a.CreatedAt)
                                              .AsNoTracking() // Use AsNoTracking for better performance
                                              .ToListAsync();

                // Set cache options
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache for 5 minutes
                };

                _cache.Set(cacheKey, announcements, cacheEntryOptions);
            }

            

            var viewModel = new HomeViewModel
            {
                User = user,
                Announcements = announcements,
                
            };

            return View(viewModel);
        }

        public IActionResult TermsAndConditions()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}
