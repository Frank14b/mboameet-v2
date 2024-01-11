using API.DTOs;

namespace API.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailRequestDto mailRequest);
        // string GetEmailTemplate(string emailTemplate, EmailRequestDto mailRequest);
    }
}