using System.Text;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RazorEngineCore;

namespace API.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        private readonly string supportEmail = "support.auth@mboameet.net";

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(EmailRequestDto mailRequest)
        {
            var email = new MimeMessage
            {
                Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail)
            };
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = GetEmailTemplate("users", mailRequest);

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.Auto);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public string GetEmailTemplate(string emailTemplate, EmailRequestDto mailRequest)
        {
            string mailTemplate = LoadTemplate(emailTemplate);

            IRazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate modifiedMailTemplate = razorEngine.Compile(mailTemplate);

            return modifiedMailTemplate.Run(mailRequest);
        }

        public string LoadTemplate(string emailTemplate)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string templateDir = Path.Combine("", "/Users/kamgafrank/Documents/projects/homemanag/API/Views/Emails");
            string templatePath = Path.Combine(templateDir, $"{emailTemplate}.cshtml");

            using FileStream fileStream = new(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader streamReader = new(fileStream, Encoding.Default);

            string mailTemplate = streamReader.ReadToEnd();
            streamReader.Close();

            return mailTemplate;
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
}