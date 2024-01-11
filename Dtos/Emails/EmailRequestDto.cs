using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class EmailRequestDto
    {
        public required string ToEmail { get; set; }
        public required string ToName { get; set; }
        public required string SubTitle { get; set; }
        public string? ReplyToEmail { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }

    public class MailSettings
    {
        public string? Mail { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
    }
}