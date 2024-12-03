using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Pawsome.Models; 

namespace Pawsome.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderPassword"]),
                EnableSsl = true, // Make sure SSL is enabled
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:SenderEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Set to true if you're sending HTML email
            };
            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"SMTP Exception: {ex.Message}");
                throw;
            }
        }



        public async Task SendVaccinationReminderAsync(Pet pet)
        {
            if (pet.NextDueDate.HasValue && pet.VaccinationDate.HasValue)
            {
                // Strip time from both the current date and the NextDueDate
                var currentDate = DateTime.Now.Date;
                var nextDueDate = pet.NextDueDate.Value.Date;

                // Calculate the number of days between the current date and the next due date
                var daysBeforeDue = (nextDueDate - currentDate).Days;

                // If the due date is within the next 7 days (inclusive)
                if (daysBeforeDue >= 1 && daysBeforeDue <= 7)
                {
                    var subject = "Vaccination Reminder for Your Registered Pet";
                    var body = $"Dear {pet.OwnerName},<br><br>Your pet's vaccination is due in {daysBeforeDue} day(s). Please make sure to get your pet vaccinated before the due date: {pet.NextDueDate:MMMM dd, yyyy}.<br><br>Thank you for using Pawsome!";

                    // Send the email to the pet owner
                    await SendEmailAsync(pet.OwnerEmail, subject, body);
                }
            }
        }


    }
}
