﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pawsome.Data;
using Pawsome.Migrations;
using Pawsome.Models;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pawsome.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display calendar with appointments and announcements
        [HttpGet]
        public async Task<IActionResult> Calendar()
        {
           
            var services = _context.Services.Select(c => new SelectListItem { Value = c.ServiceId.ToString(), Text = c.ServiceName }).ToList();
            ViewBag.Services = services;

            var userId = HttpContext.Session.GetString("UserId");
            var pets = _context.Pets
                .Where(p => p.OwnerId.ToString() == userId && p.IsArchived == false && p.PetStatus != "Lost")
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            ViewBag.Pets = pets;

            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin") == "True";
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Fetch all appointments for display
            var appointments = await _context.Appointments.Include(a => a.User).ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> EditCalendar(string AvailableDates, List<int> AvailableServices)
        {
            // Parse the dates and save them in the database
            var dateList = AvailableDates.Split(',').ToList();

            foreach (var date in dateList)
            {
                // Create or update available date record
                var availableDate = new AvailableDate
                {
                    Date = DateTime.Parse(date)
                };

                _context.AvailableDates.Add(availableDate);
                await _context.SaveChangesAsync();

                // Link the services with this available date
                foreach (var serviceId in AvailableServices)
                {
                    var availableDateService = new AvailableDateService
                    {
                        AvailableDateId = availableDate.Id,
                        ServiceId = serviceId
                    };
                    _context.AvailableDateServices.Add(availableDateService);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Calendar"); // or another appropriate redirect
        }




        public IActionResult GetAvailableDates()
        {
            var availableDates = _context.AvailableDates.Select(d => d.Date.ToString("yyyy-MM-dd")).ToList();
            return Json(availableDates);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableServices(DateTime date)
        {
            // Find the available dates that match the provided date
            var availableDate = await _context.AvailableDates
                .Include(ad => ad.AvailableDateServices)
                .ThenInclude(ads => ads.Service)
                .Where(ad => ad.Date.Date == date.Date)
                .FirstOrDefaultAsync();

            if (availableDate == null)
            {
                // If no available dates found, return an empty list or handle accordingly
                return Json(new List<object>());
            }

            // Extract services associated with the available date
            var availableServices = availableDate.AvailableDateServices
                .Select(ads => new
                {
                    ads.ServiceId,
                    ads.Service.ServiceName
                })
                .ToList();

            return Json(availableServices);
        }


        // Regular User: Book an appointment
        [HttpGet]
        public async Task<IActionResult> BookAppointment(int AppointmentId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var pets = _context.Pets
                .Where(p => p.OwnerId.ToString() == userId && p.IsArchived == false && p.PetStatus != "Lost")
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            ViewBag.Pets = pets;

            var services = _context.Services.Select(c => new SelectListItem { Value = c.ServiceId.ToString(), Text = c.ServiceName }).ToList();
            ViewBag.Services = services;
            var appointmenttypes = _context.AppointmentTypes.Select(c => new SelectListItem { Value = c.AppointTypeId.ToString(), Text = c.AppointType }).ToList();
            ViewBag.AppointmentTypes = appointmenttypes;



            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }



            var appointment = await _context.Appointments.FindAsync(AppointmentId);
            if (appointment == null)
            {
                return NotFound();
            }



            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment(Appointment appointment, Service servic, AppointmentType atyp, Pet pett)
        {

            var email = HttpContext.Session.GetString("Email");
            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin") == "True";
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (isPvetAdmin)
            {
                appointment.Status = "Approved"; // Automatically approve the appointment
            }
            else
            {
                appointment.Status = "Pending"; // Default to Pending for other users
            }

            var service = _context.Services.SingleOrDefault(c => c.ServiceId == servic.ServiceId);
            appointment.Service = service?.ServiceName;
            var pet = _context.Pets.SingleOrDefault(c => c.Id == pett.Id);
            appointment.PetName = pet?.Name;
            var appointmenttype = _context.AppointmentTypes.SingleOrDefault(c => c.AppointTypeId == atyp.AppointTypeId);
            appointment.AppointmentType = appointmenttype?.AppointType;

            // Check the number of existing appointments for the selected date
            var appointmentCount = await _context.Appointments
                .Where(a => a.Date.Date == appointment.Date.Date)
                .CountAsync();

            if (appointmentCount >= 10)
            {
                // If 10 or more appointments exist, return an error
                ModelState.AddModelError("", "The maximum number of appointments for this day has been reached.");
                return View(appointment);
            }

            if (isPvetAdmin)
            {
                // Logic to set ClientName if it's a PvetAdmin (for example, from the input)
                appointment.ClientName = appointment.ClientName; // Update this according to your input field for PvetAdmin
                // Logic to set ClientName if it's a PvetAdmin (for example, from the input)
                appointment.AppointmentType = appointment.AppointmentType; // Update this according to your input field for PvetAdmin
            }
            else
            {
                // Logic for regular user - the ClientName is set in the hidden field
                appointment.ClientName = HttpContext.Session.GetString("Firstname");
                // Ensure that the AppointmentType is set to "Online"
                appointment.AppointmentType = "Online";
            }

            appointment.UserId = user.Id;
            appointment.PetId = pet.Id;
            appointment.PetType = pet.PetType;
            appointment.Breed = pet.Breed;
            appointment.Age = pet.Age;
            appointment.DateOfBirth = pet.DateOfBirth;
            appointment.Gender = pet.Gender;
            appointment.Weight = pet.Weight;
            appointment.VaccinationStatus = pet.VaccinationStatus;
            appointment.VaccineType = pet.VaccineType;
            appointment.VaccinationDate = pet.VaccinationDate;
            appointment.NextDueDate = pet.NextDueDate;
            appointment.VaccinatedBy = pet.VaccinatedBy;
            appointment.VaccineBlockNo = pet.VaccineBlockNo;
            appointment.VaccineSource = pet.VaccineSource;
            appointment.PetPhoto = pet.Photo;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments");
        }

       


        [HttpGet]
        public async Task<IActionResult> GetAppointmentCount(DateTime date)
        {
            var count = await _context.Appointments
                .Where(a => a.Date.Date == date.Date)
                .CountAsync();

            return Json(count);
        }


        // PvetAdmin: View all pending appointments
        [HttpGet]
        public async Task<IActionResult> PendingAppointments()
        {
            var services = _context.Services.Select(c => new SelectListItem { Value = c.ServiceId.ToString(), Text = c.ServiceName }).ToList();
            ViewBag.Services = services;
            var appointmenttypes = _context.AppointmentTypes.Select(c => new SelectListItem { Value = c.AppointTypeId.ToString(), Text = c.AppointType }).ToList();
            ViewBag.AppointmentTypes = appointmenttypes;

            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");
            if (isPvetAdmin != "True")
            {
                return RedirectToAction("Index", "Home");
            }

            var pendingAppointments = await _context.Appointments
                .Where(a => a.Status == "Pending")
                .Include(a => a.User)
                .ToListAsync();

            return View(pendingAppointments);
        }

        // PvetAdmin: Approve an appointment
        [HttpPost]
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            var services = _context.Services.Select(c => new SelectListItem { Value = c.ServiceId.ToString(), Text = c.ServiceName }).ToList();
            ViewBag.Services = services;
            var appointmenttypes = _context.AppointmentTypes.Select(c => new SelectListItem { Value = c.AppointTypeId.ToString(), Text = c.AppointType }).ToList();
            ViewBag.AppointmentTypes = appointmenttypes;

            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");
            if (isPvetAdmin != "True")
            {
                return RedirectToAction("Index", "Home");
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            appointment.Status = "Approved";
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            // Create a notification for the user
            var notification = new NotificationModel
            {
                UserId = appointment.UserId, // Assuming Pet has an OwnerId property linking to the User
                Message = $"Your appoitment for '{appointment.Service}' has been Approved!",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification); // Add the notification to the context
            _context.SaveChanges(); // Save changes to the database

            return RedirectToAction("PendingAppointments");
        }

        [HttpPost]
        public IActionResult ApproveAllAppointments()
        {
            var pendingAppointments = _context.Appointments.Where(a => a.Status == "Pending").ToList();
            foreach (var appointment in pendingAppointments)
            {
                appointment.Status = "Approved";
            }

            _context.SaveChanges();
            return RedirectToAction("PendingAppointments");
        }

        [HttpPost]
        public IActionResult DeclineAllAppointments()
        {
            var pendingAppointments = _context.Appointments.Where(a => a.Status == "Pending").ToList();
            foreach (var appointment in pendingAppointments)
            {
                appointment.Status = "Declined";
            }

            _context.SaveChanges();
            return RedirectToAction("PendingAppointments");
        }

        [HttpPost]
        public IActionResult DeclineAppointment(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null)
            {
                // Decline the appointment
                appointment.Status = "Declined";  // Assuming "Declined" is a valid status
                _context.SaveChanges();
            }

            // Create a notification for the user
            var notification = new NotificationModel
            {
                UserId = appointment.UserId, // Assuming Pet has an OwnerId property linking to the User
                Message = $"Your appoitment for '{appointment.Service}' has been Declined!",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification); // Add the notification to the context
            _context.SaveChanges(); // Save changes to the database

            return RedirectToAction("PendingAppointments");
        }


        // PvetAdmin: View all approved appointments
        [HttpGet]
        public async Task<IActionResult> ApprovedAppointments()
        {
            var services = _context.Services.Select(c => new SelectListItem { Value = c.ServiceId.ToString(), Text = c.ServiceName }).ToList();
            ViewBag.Services = services;
            var appointmenttypes = _context.AppointmentTypes.Select(c => new SelectListItem { Value = c.AppointTypeId.ToString(), Text = c.AppointType }).ToList();
            ViewBag.AppointmentTypes = appointmenttypes;

            var vaccinationstatuses = _context.VaccinationStatuses.Select(c => new SelectListItem { Value = c.VStatusId.ToString(), Text = c.VStatusType }).ToList();
            ViewBag.VaccinationStatuses = vaccinationstatuses;
            var vaccinetypes = _context.VaccineTypes.Select(c => new SelectListItem { Value = c.VTypeId.ToString(), Text = c.VType }).ToList();
            ViewBag.VaccineTypes = vaccinetypes;
            var vaccinesources = _context.VaccineSources.Select(c => new SelectListItem { Value = c.VSourceId.ToString(), Text = c.VSource }).ToList();
            ViewBag.VaccineSources = vaccinesources;

            var isPvetAdmin = HttpContext.Session.GetString("IsPvetAdmin");
            if (isPvetAdmin != "True")
            {
                return RedirectToAction("Index", "Home");
            }

            var approvedAppointments = await _context.Appointments
                .Where(a => a.Status == "Approved")
                .Include(a => a.User)
                .ToListAsync();

            return View(approvedAppointments);
        }


        // Regular User: View my appointments
        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var appointments = await _context.Appointments
            
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

            return View(appointments);
        }

        // Regular User: Delete a pending appointment
        [HttpGet]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null || user.IsPvetAdmin)
            {
                return Unauthorized();
            }

            var appointment = await _context.Appointments
                .SingleOrDefaultAsync(a => a.AppointmentId == id && a.UserId == user.Id && a.Status == "Pending");

            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments");
        }

        public IActionResult History()
        {
            // Fetch all appointments with the status 'Done'
            var historyAppointments = _context.Appointments
                                              .Where(a => a.Status == "Done")
                                              .ToList();

            return View(historyAppointments);
        }

    }
}
