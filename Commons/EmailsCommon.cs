using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace API.Commons;

public class EmailsCommon : ControllerBase
{
    private readonly IMailService _mailService;
    private readonly ILogger _logger;
    private readonly string supportEmail = "support.auth@mboameet.net";

    public EmailsCommon(IMailService mailService, ILogger logger)
    {
        _mailService = mailService;
        _logger = logger;
    }

    public async Task SendMail([FromForm] EmailRequestDto request)
    {
        try
        {
            await _mailService.SendEmailAsync(request);
        }
        catch (Exception e)
        {
            _logger.LogError("An error occurred: {message}", e.Message);
        }
    }

    public string UserLoginBody(User? data)
    {
        string body = "<div> <p>Dear " + data?.UserName + ",</p><br/>"
                       + "<p>We are pleased to inform you that you have successfully logged in to your account on " + DateTime.UtcNow
                       + " GMT from .This email is to confirm that the login was authorized by you to access your account.</p>"
                       + "<br/> <p>If you did not authorize this login, please contact us immediately at " + supportEmail
                       + " We take the security of your account very seriously and will investigate any suspicious activity.</p>"
                       + "<br/><p>Thank you for choosing our services.</p>"
                       + "</div>";

        return body;
    }

    public string UserRegisterBody(User? data)
    {
        string body = "<div> <p>Dear " + data?.FirstName + ",</p><br/>"
                       + "<p>Thank you for signing up for our services! We are thrilled to have you as a part of our community. Your account has been successfully created the " + DateTime.UtcNow
                       + " GMT from .This email is to confirm that the registration was authorized by you.</p>"
                       + "<br/> <p>If you did not perform this action, please contact us immediately at " + supportEmail
                       + " We take the security very seriously and will investigate any suspicious activity.</p>"
                       + "<br/><p>Thank you for choosing our services.</p>"
                       + "</div>";

        return body;
    }

    public string UserForgetPasswordBody(ForgetPasswordEmailDto data)
    {
        string body = "<div> <p>Dear " + data.UserName + ",</p><br/>"
                       + "<p>A request to change your password has been initiated in to your account on " + DateTime.UtcNow
                       + " GMT .This email is to confirm that the forget password was initiated by you.</p>"
                       + "<br/> <p>To proceed with the password change, use following code <span>" + data?.Otp + "</span> or click on the link bellow <br/> <p><a href='" + data?.Link + "'>Forget Password</a><p></p>"
                       + "<br/> <p>If you did not authorize this login, please contact us immediately at " + supportEmail
                       + " We take the security of your account very seriously and will investigate any suspicious activity.</p>"
                       + "<br/><p>Thank you for choosing our services.</p>"
                       + "</div>";

        return body;
    }

    public string ChangePasswordBody(User? data)
    {
        string body = "<div> <p>Dear " + data?.UserName + ",</p><br/>"
                       + "<p>A request to change your password has been completed in to your account on " + DateTime.UtcNow
                       + " GMT .This email is to confirm that the change password was done by you.</p>"
                       + "<br/>"
                       + "<br/> <p>If you did not authorize this login, please contact us immediately at " + supportEmail
                       + " We take the security of your account very seriously and will investigate any suspicious activity.</p>"
                       + "<br/><p>Thank you for choosing our services.</p>"
                       + "</div>";

        return body;
    }

    public string DeleteAccountBody(User? data)
    {
        string body = "<div> <p>Dear " + data?.UserName + ",</p><br/>"
                       + "<p>A request to delete your acount has been received the " + DateTime.UtcNow
                       + " GMT. your request is under progress and your account will be totally deleted from our server in 30 days <br/><br/> you can still revert this operation within that time by using the following link: </p>"
                       + "<br/>"
                       + "<br/> <p>If you did not authorize this action, please contact us immediately at " + supportEmail
                       + " We take the security of your account very seriously and will investigate any suspicious activity.</p>"
                       + "<br/><p>Thank you for choosing our services.</p>"
                       + "</div>";

        return body;
    }
}
