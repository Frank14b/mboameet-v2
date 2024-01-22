using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace API.Commons
{
    public class EmailsCommon : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly ILogger _logger;

        public EmailsCommon(IMailService mailService, ILogger logger)
        {
            _mailService = mailService;
            _logger = logger;
        }

        public async Task<bool> SendMail([FromForm] EmailRequestDto request)
        {
            try
            {
                await _mailService.SendEmailAsync(request);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred: {message}", e.Message);
                return false;
            }
        }

        public string UserLoginBody(ResultloginDto data)
        {
            string body = "<div> <p>Dear " + data.FirstName + ",</p><br/>"
                           + "<p>We are pleased to inform you that you have successfully logged in to your account on " + DateTime.UtcNow
                           + " GMT from .This email is to confirm that the login was authorized by you to access your account.</p>"
                           + "<br/> <p>If you did not authorize this login, please contact us immediately at support.auth@homemanag.net"
                           + " We take the security of your account very seriously and will investigate any suspicious activity.</p>"
                           + "<br/><p>Thank you for choosing our services.</p>"
                           + "</div>";

            return body;
        }

        public string UserRegisterBody(AppUser? data)
        {
            string body = "<div> <p>Dear " + data?.FirstName + ",</p><br/>"
                           + "<p>Thank you for signing up for our services! We are thrilled to have you as a part of our community. Your account has been successfully created the " + DateTime.UtcNow
                           + " GMT from .This email is to confirm that the registration was authorized by you.</p>"
                           + "<br/> <p>If you did not perform this action, please contact us immediately at support.auth@homemanag.net"
                           + " We take the security very seriously and will investigate any suspicious activity.</p>"
                           + "<br/><p>Thank you for choosing our services.</p>"
                           + "</div>";

            return body;
        }

        // public string BusinessCreate(BusinessResultListDto data)
        // {
        //     string body = "<div><p>We are pleased to inform you that the requested Business <b>" + data.Name + "</b> has been successfully created and is now ready for use.</p></div>";

        //     return body;
        // }

        // public string PropertyCreate(PropertiesResultListDto data)
        // {
        //     string body = "<div><p>We are pleased to inform you that the requested Property <b>" + data.Name + "</b> has been successfully created and is now ready for use.</p></div>";

        //     return body;
        // }
    }
}