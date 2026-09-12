using Shared.DTOs;

namespace Core.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string plainTextBody, string htmlBody);     
        Task SendAppointmentEmailAsync(AppointmentListDto appointment);
    }
}
