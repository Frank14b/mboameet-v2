using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailRequestDto mailRequest);
        // string GetEmailTemplate(string emailTemplate, EmailRequestDto mailRequest);
        string UserLoginBody(User? data);
        string UserRegisterBody(User? data);
        string UserForgetPasswordBody(ForgetPasswordEmailDto data);
        string ChangePasswordBody(User? data);
        string DeleteAccountBody(User? data);
    }
}