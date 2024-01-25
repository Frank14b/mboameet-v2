using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailRequestDto mailRequest);
        // string GetEmailTemplate(string emailTemplate, EmailRequestDto mailRequest);
        string UserLoginBody(AppUser? data);
        string UserRegisterBody(AppUser? data);
        string UserForgetPasswordBody(ForgetPasswordEmailDto data);
        string ChangePasswordBody(AppUser? data);
        string DeleteAccountBody(AppUser? data);
    }
}