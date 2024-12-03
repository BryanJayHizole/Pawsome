using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawsome.Data;
using Pawsome.Models;
using System.Drawing;

namespace Pawsome.Controllers
{
    public class AnnouncementsController : Controller
    {
        private readonly ApplicationDbContext _context;
      

        public AnnouncementsController(ApplicationDbContext context)
        {
            _context = context;
       
        }

        // GET: Announcements
        public async Task<IActionResult> Index()
        {
            var announcements = await _context.Announcements.OrderByDescending(a => a.CreatedAt).ToListAsync();
            return View();
        }

        // GET: Announcements/Create
        public IActionResult Create()
        {
            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");
            if (isPvetAdmin != "True")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Announcement announcement, IList<IFormFile> photoFiles) 
        {
            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");

            // Set announcement creation date
            announcement.CreatedAt = DateTime.Now;

            if (photoFiles != null && photoFiles.Count > 0)
            {
                foreach (var photoFile in photoFiles)
                {
                    if (photoFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await photoFile.CopyToAsync(memoryStream);
                            announcement.Photos.Add(memoryStream.ToArray()); // Save each photo as a byte array
                        }
                    }
                }
            }


            // Save the announcement to the database
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();





            // Redirect to the Index action of the HomeController after successful creation
            return RedirectToAction("Index", "Home");
        }
    }
}
