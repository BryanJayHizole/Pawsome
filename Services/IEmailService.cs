using Pawsome.Models;

namespace Pawsome.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendVaccinationReminderAsync(Pet pet); 
    }
}
