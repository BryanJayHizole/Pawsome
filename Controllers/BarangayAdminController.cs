using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Pawsome.Data;
using Pawsome.Models;
using Pawsome.Services;
using System.Drawing;
using System.Security.Claims;

namespace Pawsome.Controllers
{
    public class BarangayAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public BarangayAdminController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> ManagePenalties()
        {
            // Assuming the BarangayAdmin is logged in and their BarangayId is in the session or user context
            var isBarangayAdmin = HttpContext.Session.GetString("IsBarangayAdmin");

            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

           
            var users = _context.Users
                 
                .AsQueryable(); // Use IQueryable for better query composition

            var userBarangay = _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.Barangay)
                .FirstOrDefault();

            if (isBarangayAdmin == "True")
            {
                // If the user is a Barangay Admin, show all pets in their barangay
                users = users.Where(p => p.Barangay == userBarangay);
            }

            // Get all penalty assignments that are not paid
            var penaltyAssignments = await _context.PenaltyAssignments
                .Include(pa => pa.PenaltyFine)
                .Where(pa => pa.User.Barangay == userBarangay && pa.Status == PenaltyStatus.Unpaid)
                .ToListAsync();

            // Get all paid penalty assignments
            var paidPenalties = await _context.PenaltyAssignments
                .Include(pa => pa.PenaltyFine)
                .Where(pa => pa.User.Barangay == userBarangay && pa.Status == PenaltyStatus.Paid)
                .ToListAsync();

            // Convert users to a list before passing it to the view
            var userList = await users.ToListAsync(); // This materializes the query into a list

            // Get the available penalty fines
            var penaltyFines = await _context.PenaltyFines.ToListAsync();

            var viewModel = new ManagePenaltiesViewModel
            {
                Users = userList,
                PenaltyAssignments = penaltyAssignments,
                PaidPenalties = paidPenalties,
                PenaltyFines = penaltyFines
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignPenalty(int userId, int penaltyId)
        {
            var user = await _context.Users.FindAsync(userId);
            var penalty = await _context.PenaltyFines.FindAsync(penaltyId);

            if (user == null || penalty == null)
                return NotFound("User or Penalty not found.");

            // Logic to assign penalty to the user (e.g., logging in a separate table)
            var penaltyAssignment = new PenaltyAssignment
            {
                UserId = user.Id,
                PenaltyFineId = penalty.Id,
                AssignedDate = DateTime.Now,
                Status = PenaltyStatus.Unpaid // Initially set the penalty status to Unpaid
            };

           
            _context.Add(penaltyAssignment);
            await _context.SaveChangesAsync();

            // Prepare the email content
            var subject = "Penalty Assigned to Your Account";
            var body = $"Dear {user.Firstname},\n\n" +
                       $"We would like to inform you that a penalty has been assigned to your account due to the following reason:\n\n" +
                       $"- **Penalty**: {penalty.Name}\n" +
                       $"- **Amount**: {penalty.FineAmount:C}\n" +
                       $"- **Assigned Date**: {penaltyAssignment.AssignedDate}\n\n" +
                       $"Please make sure to address this penalty as per the guidelines.\n\n" +
                       $"If you have any questions or need further assistance, feel free to reach out to us.\n\n" +
                       $"Thank you for your attention.\n\n" +
                       $"Best regards,\n";
        

            // Send the email
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return RedirectToAction("ManagePenalties");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsPaid(int penaltyId)
        {
            var userId = HttpContext.Session.GetString("UserId");

            var penalty = _context.PenaltyAssignments
                         .Include(pa => pa.PenaltyFine) // Make sure PenaltyFine is loaded
                         .FirstOrDefault(pa => pa.Id == penaltyId);

            if (penalty == null)
            {
                return NotFound("Penalty Assignment not found.");
            }

            // Mark as paid
            penalty.Status = PenaltyStatus.Paid;
            await _context.SaveChangesAsync();

            // Send an email to the user informing them that the penalty has been marked as paid
            var user = await _context.Users.FindAsync(penalty.UserId);
            if (user != null)
            {
                var subject = "Penalty Status Update: Paid";
                var body = $"Dear {user.Firstname},\n\n" +
                           $"We would like to inform you that your penalty with the following details has been marked as **Paid**:\n\n" +
                           $"- **Penalty**: {penalty.PenaltyFine.Name}\n" +
                           $"- **Amount**: {penalty.PenaltyFine.FineAmount:C}\n" +
                           $"- **Assigned Date**: {penalty.AssignedDate}\n" +
                           $"- **Status**: Paid\n\n" +
                           $"Thank you for addressing this penalty.\n\n" +
                           $"Best regards,\n";
                          

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            // Set success message in TempData
            TempData["SuccessMessage"] = "The penalty has been successfully marked as Paid.";

            return RedirectToAction("ManagePenalties");
        }

        // GET: SystemMaintenance/SetCapturedLimit
        public IActionResult Index()
        {
          
            var barangayAdminId = HttpContext.Session.GetString("UserId");

            
           
            var barangay = HttpContext.Session.GetString("Barangay");
            if (barangay == null)
            {
                return NotFound("Barangay not found.");
            }

            // Get or create SystemSetting for this barangay
            var systemSetting = _context.SystemSettings.FirstOrDefault(s => s.BarangayName == barangay);
            if (systemSetting == null)
            {
                systemSetting = new SystemSetting
                {
                    BarangayName = barangay,
                    CapturedLimit = 10,  
                    EuthanasiaDays = 7   
                };
                _context.SystemSettings.Add(systemSetting);
                _context.SaveChanges();
            }

            var model = new SystemMaintenanceViewModel
            {
                CapturedLimit = systemSetting.CapturedLimit,
                EuthanasiaDays = systemSetting.EuthanasiaDays,
                BarangayName = barangay
            };

            return View(model);
        }

        // POST: SystemMaintenance/SetCapturedLimit
        [HttpPost]
        public IActionResult Index(SystemMaintenanceViewModel model)
        {
            if (model.CapturedLimit <= 0)
            {
                ViewData["ErrorMessage"] = "Captured limit must be a positive number.";
                return View(model);
            }

            // Get the system setting for the barangay of the logged-in admin
            var barangayAdmin = HttpContext.Session.GetString("Barangay");
            var barangay = _context.Barangays.FirstOrDefault(b => b.BarangayName == barangayAdmin);
            if (barangay == null)
            {
                return NotFound("Barangay not found.");
            }

            var systemSetting = _context.SystemSettings.FirstOrDefault(s => s.BarangayName == barangay.BarangayName);
            if (systemSetting != null)
            {
                systemSetting.CapturedLimit = model.CapturedLimit;
                systemSetting.EuthanasiaDays = model.EuthanasiaDays;
                _context.SaveChanges();  // Save the updated setting to the database
                return RedirectToAction("Index");
            }

            // If no system setting found, create one
            systemSetting = new SystemSetting
            {
                BarangayName = barangay.BarangayName,
                CapturedLimit = model.CapturedLimit,
                 EuthanasiaDays = model.EuthanasiaDays
            };
            _context.SystemSettings.Add(systemSetting);
            _context.SaveChanges();

            return RedirectToAction("SetCapturedLimit");
        }

    }
}
