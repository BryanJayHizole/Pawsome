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
                var isBarangayAdmin = HttpContext.Session.GetString("IsBarangayAdmin");
                var barangay = HttpContext.Session.GetString("Barangay");

                var email = HttpContext.Session.GetString("Email");
                if (email == null)
                {
                    return RedirectToAction("Login", "User");
                }

                // Get the penalties created by the current BarangayAdmin
                var penaltiesCreatedByAdmin = _context.PenaltyFines
                    .Where(p => p.BarangayName == barangay)
                    .ToList();

                var users = _context.Users
                 
                    .AsQueryable(); // Use IQueryable for better query composition

                var userBarangay = _context.Users
                    .Where(u => u.Email == email)
                    .Select(u => u.Barangay)
                    .FirstOrDefault();

                if (isBarangayAdmin == "True")
                {
                    // If the user is a Barangay Admin, show all users in their barangay
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

                // Get all paid penalty assignments
                var resolvedPenalties = await _context.PenaltyAssignments
                    .Include(pa => pa.PenaltyFine)
                    .Where(pa => pa.User.Barangay == userBarangay && pa.Status == PenaltyStatus.Resolved)
                    .ToListAsync();

                // Convert users to a list before passing it to the view
                var userList = await users.ToListAsync(); // This materializes the query into a list

            

                var viewModel = new ManagePenaltiesViewModel
                {
                    Users = userList,
                    PenaltyAssignments = penaltyAssignments,
                    PaidPenalties = paidPenalties,
                    ResolvedPenalties = resolvedPenalties,
                    PenaltyFines = penaltiesCreatedByAdmin
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

                // Create a notification for the user
                var notification = new NotificationModel
                {
                    UserId = user.Id, 
                    Message = $"You have been assigned a Penalty: '{penalty.Name}'\n\n" +
                              $"with an fine amount of '{penalty.FineAmount}' by your Barangay Admin.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };

                _context.Notifications.Add(notification); // Add the notification to the context
                _context.SaveChanges(); // Save changes to the database

                return RedirectToAction("ManagePenalties");
            }

            [HttpPost]
            public async Task<IActionResult> MarkAsPaid(int penaltyId)
            {
           
            
                var userId = HttpContext.Session.GetString("UserId");

                var penalty = _context.PenaltyAssignments
                             .Include(pa => pa.PenaltyFine) // Make sure PenaltyFine is loaded
                             .FirstOrDefault(pa => pa.Id == penaltyId);

                if (penalty != null)
                {
                    // Mark as paid
                    penalty.Status = PenaltyStatus.Paid;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Penalty marked as paid successfully.";

                }

           
                
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

                // Create a notification for the user
                var notification = new NotificationModel
                {
                    UserId = user.Id,
                    Message = $"Your Penalty: '{penalty.PenaltyFine.Name}'\n\n" +
                              $"with an fine amount of '{penalty.PenaltyFine.FineAmount}' has been Marked as Paid.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };

                _context.Notifications.Add(notification); // Add the notification to the context
                _context.SaveChanges(); // Save changes to the database

                return RedirectToAction("ManagePenalties");
            }

            [HttpPost]
            public IActionResult ResolvePenalty(int penaltyId, string description)
            {
                // Find the penalty assignment by ID and update its status
                var penaltyAssignment = _context.PenaltyAssignments
                    .FirstOrDefault(pa => pa.Id == penaltyId);

                if (penaltyAssignment != null)
                {
                    penaltyAssignment.Status = PenaltyStatus.Resolved;
                    penaltyAssignment.Description = description; // Assuming you add a 'Description' property to your model
                    _context.SaveChanges();
                }

                TempData["SuccessMessage"] = "Penalty has been resolved successfully.";
                return RedirectToAction("ManagePenalties");
            }


        // GET: SystemMaintenance/SetCapturedLimit
        public async Task<IActionResult> Index()
        {
            var barangayAdminId = HttpContext.Session.GetString("UserId");
            var barangay = HttpContext.Session.GetString("Barangay");

            if (barangay == null)
            {
                return NotFound("Barangay not found.");
            }

            // Get or create SystemSetting for this barangay
            var systemSetting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.BarangayName == barangay);

            if (systemSetting == null)
            {
                systemSetting = new SystemSetting
                {
                    BarangayName = barangay,
                    CapturedLimit = 10, // Default value
                    EuthanasiaDays = 7 // Default value
                };
                _context.SystemSettings.Add(systemSetting);
                await _context.SaveChangesAsync();
            }

            var penalties = await _context.PenaltyFines
                .Where(p => p.BarangayName == barangay)
                .ToListAsync(); // Fetch penalties specific to the current Barangay

            var model = new SystemMaintenanceViewModel
            {
                CapturedLimit = systemSetting.CapturedLimit,
                EuthanasiaDays = systemSetting.EuthanasiaDays,
                BarangayName = barangay,
                PenaltyFines = penalties
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SystemMaintenanceViewModel model)
        {
            var barangayName = HttpContext.Session.GetString("Barangay"); // Get the barangay from session

            if (barangayName == null)
            {
                return NotFound("Barangay not found.");
            }

            // Save captured limit and euthanasia days logic
            var systemSettings = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.BarangayName == barangayName);
            if (systemSettings != null)
            {
                systemSettings.CapturedLimit = model.CapturedLimit;
                systemSettings.EuthanasiaDays = model.EuthanasiaDays;
            }
            else
            {
                systemSettings = new SystemSetting
                {
                    BarangayName = barangayName,
                    CapturedLimit = model.CapturedLimit,
                    EuthanasiaDays = model.EuthanasiaDays
                };
                _context.SystemSettings.Add(systemSettings);
            }

            // Handle new penalties and fines
            if (model.PenaltyFines != null)
            {
                foreach (var penalty in model.PenaltyFines)
                {
                    // Set the BarangayName for the new penalty
                    penalty.BarangayName = barangayName;

                    // Add the new penalty to the context
                    _context.PenaltyFines.Add(penalty);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "System settings and penalties have been saved successfully.";

            return RedirectToAction("Index");
        }


       

    }
}
