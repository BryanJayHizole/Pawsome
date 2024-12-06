using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pawsome.Data;
using Pawsome.Models;
using Pawsome.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Drawing.Printing;

namespace Pawsome.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;


        public UserController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _emailService = emailService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user, bool rememberMe = false)
        {
            var usr = _context.Users.SingleOrDefault(u => u.Email == user.Email);
            if (usr == null)
            {
                ViewBag.Message = "Invalid email address.";
                return View();
            }

            if (!usr.IsEmailVerified)
            {
                ViewBag.Message = "Please verify your email before logging in.";
                return View();
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(usr, usr.Password, user.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                ViewBag.Message = "Wrong password.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usr.Firstname),
                new Claim(ClaimTypes.NameIdentifier, usr.Id.ToString()), // Ensure Id is a string
                new Claim(ClaimTypes.Role, usr.IsBarangayAdmin ? "Barangay Admin" : "User"),
                new Claim(ClaimTypes.Role, usr.IsPvetAdmin ? "PVET Admin" : "User"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : (DateTimeOffset?)null
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            HttpContext.Session.SetString("Email", usr.Email);
            HttpContext.Session.SetString("UserId", usr.Id.ToString()); // Ensure Id is a string
            HttpContext.Session.SetString("Firstname", usr.Firstname);
            HttpContext.Session.SetString("Barangay", usr.Barangay);
            HttpContext.Session.SetString("IsBarangayAdmin", usr.IsBarangayAdmin.ToString());
            HttpContext.Session.SetString("IsPvetAdmin", usr.IsPvetAdmin.ToString());

            return RedirectToAction("Index", "Home");
        }


       


        public IActionResult Register(int cityId)
        {
            // Get the list of countries from the database
            var countries = _context.Countries.Select(c => new SelectListItem { Value = c.CountryId.ToString(), Text = c.CountryName }).ToList();
            ViewBag.Countries = countries;
            var provinces = _context.Provinces.Select(c => new SelectListItem { Value = c.ProvinceId.ToString(), Text = c.ProvinceName }).ToList();
            ViewBag.Provinces = provinces;
            var cities = _context.Cities.Select(c => new SelectListItem { Value = c.CityId.ToString(), Text = c.CityName }).ToList();
            ViewBag.Cities = cities;
            var barangays = _context.Barangays.Select(c => new SelectListItem { Value = c.BarangayId.ToString(), Text = c.BarangayName }).ToList();
            ViewBag.Barangays = barangays;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(User user, Country county, Province prov, City municipality, Barangay barangy)
        {
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            user.VerificationToken = GenerateToken();

            if (user.Birthday.HasValue)
            {
                var today = DateTime.Today;
                var age = today.Year - user.Birthday.Value.Year;
                if (user.Birthday.Value.Date > today.AddYears(-age)) age--;
                user.Age = age;
            }

            var country = _context.Countries.SingleOrDefault(c => c.CountryId == county.CountryId);
            var province = _context.Provinces.SingleOrDefault(p => p.ProvinceId == prov.ProvinceId);
            var city = _context.Cities.SingleOrDefault(c => c.CityId == municipality.CityId);
            var barangay = _context.Barangays.SingleOrDefault(b => b.BarangayId == barangy.BarangayId);

            user.Country = country?.CountryName;
            user.Province = province?.ProvinceName;
            user.City = city?.CityName;
            user.Barangay = barangay?.BarangayName;
            user.ProfilePhoto = null;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Send verification email
            var verificationLink = Url.Action("VerifyEmail", "User", new { token = user.VerificationToken }, Request.Scheme);
            var subject = "Pawsome Account Verification";
            var body = $"Please verify your email by clicking on the link: <a href='{verificationLink}'>Verify Email</a>";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return RedirectToAction("Login", new { success = true });
        }

        // In your UserController or AccountController
        public IActionResult CheckEmailAvailability(string email)
        {
            bool emailExists = _context.Users.Any(u => u.Email == email);
            return Json(new { exists = emailExists });
        }



        private string GenerateToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var tokenData = new byte[32];
            rng.GetBytes(tokenData);
            return Convert.ToBase64String(tokenData);
        }

        public async Task<IActionResult> VerifyEmail(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

            if (user == null || user.IsEmailVerified)
            {
                return NotFound("Invalid or expired verification link.");
            }

            user.IsEmailVerified = true;
            user.VerificationToken = null; // Clear the token once verified
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return View("EmailVerified"); // Create a view that confirms verification
        }



        [HttpGet]
        public JsonResult GetBarangaysByCity(int cityId)
        {
            var barangays = _context.Barangays
                .Where(b => b.CityId == cityId)
                .Select(b => new { b.BarangayId, b.BarangayName })
                .ToList();

            return Json(barangays);
        }



        public IActionResult Profile()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = _context.Users.SingleOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound(); // Or handle the case where the user is not found
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(User updatedUser, IFormFile profilePhoto)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = _context.Users.SingleOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound(); // Or handle the case where the user is not found
            }

            // Update the user's information
            user.Firstname = updatedUser.Firstname;
            user.Middlename = updatedUser.Middlename;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.Gender = updatedUser.Gender;
            user.ContactNumber = updatedUser.ContactNumber;
            user.Birthday = updatedUser.Birthday;
            user.Age = updatedUser.Age;
            user.Country = updatedUser.Country;
            user.Province = updatedUser.Province;
            user.City = updatedUser.City;
            user.Barangay = updatedUser.Barangay;
            // Update other properties as needed

            // Check if a new profile photo is uploaded
            if (profilePhoto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    profilePhoto.CopyTo(memoryStream);
                    user.ProfilePhoto = memoryStream.ToArray();  // Save the uploaded photo to the user's profile
                }
            }

            _context.SaveChanges(); // Save the changes to the database

            return RedirectToAction("Profile");
        }

        // View to manage users
        public async Task<IActionResult> ManageUsers(string searchTerm, int page = 1, int pageSize = 10)
        {

            // Only show non-admin users
            var users = await _context.Users
                .ToListAsync();

            // Filter users if searchTerm is provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u =>
                    u.Firstname.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var totalUsers = users.Count();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var usersToDisplay = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(users); // Returns a view displaying a list of users
        }

        // POST: Method to assign roles
        [HttpPost]
        public IActionResult AssignRole(int userId, string role)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                switch (role)
                {
                    case "BarangayAdmin":
                        user.IsBarangayAdmin = true;
                        user.IsPvetAdmin = false; // Make sure to revoke PvetAdmin if user becomes BarangayAdmin
                        break;
                    case "RegularUser":
                        user.IsBarangayAdmin = false;
                        user.IsPvetAdmin = false; // Revoke both roles if user becomes regular
                        break;
                    case "PvetAdmin":
                        user.IsPvetAdmin = true;
                        user.IsBarangayAdmin = false; // Ensure the user cannot have both roles at the same time
                        break;
                    default:
                        break;
                }

                _context.SaveChanges(); // Save the changes to the database
            }

            return RedirectToAction("ManageUsers"); // Redirect to refresh the user list
        }

        public IActionResult GetUserInfo(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            return PartialView("_UserInfoPartial", user); // Replace with actual partial view name
        }

        public IActionResult Logout()
        {
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");

            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
    }
}
