using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.DTOs
{
    public class LoginDto
    {
        [Required]
        public required string Login { get; set; }

        [Required]
        public required string Password { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        [MinLength(3)]
        public required string Username { get; set; }

        [Required]
        [MinLength(3)]
        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        public required string Password { get; set; }
    }

    public class SocialAuthDto
    {
        [Required]
        [MinLength(3)]
        public required string Username { get; set; }

        [Required]
        [MinLength(3)]
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string SocialId { get; set; }
        [Required]
        public required string SocialToken { get; set; }
        [Required]
        public string? PhotoUrl { get; set; }
        [Required]
        public required string Provider { get; set; }
    }

    public class DeleteUserDto
    {
        [Required]
        public required string Id { get; set; }
    }

    public class UpdateStatusUserDto
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        [EnumDataType(typeof(StatusEnum))]
        public int Status { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class UpdateProfileDto
    {
        [MinLength(1)]
        public required string Username { get; set; }

        [MinLength(1)]
        public string? Firstname { get; set; }

        [MinLength(1)]
        public string? Lastname { get; set; }

        [MinLength(1)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        public required string CurrentPassword { get; set; }

        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        public string? NewPassword { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class EditUserDto
    {
        public required string Id { get; set; }

        [MinLength(1)]
        public required string Username { get; set; }

        [MinLength(1)]
        public string? Firstname { get; set; }

        [MinLength(1)]
        public string? Lastname { get; set; }

        [MinLength(1)]
        [EmailAddress]
        public string? Email { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}