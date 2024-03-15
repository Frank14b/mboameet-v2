using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class LoginDto
{
    [Required]
    [MinLength(3)]
    public required string UserName { get; set; }

    [Required]
    [MinLength(AppConstants.PasswordMinLength)]
    [RegularExpression(AppConstants.PasswordRegularExp)]
    public required string Password { get; set; }
}

public class RegisterDto
{
    [Required]
    [MinLength(3)]
    public required string UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MinLength(AppConstants.PasswordMinLength)]
    [RegularExpression(AppConstants.PasswordRegularExp)]
    public required string Password { get; set; }
}

public class SocialAuthDto
{
    [Required]
    [MinLength(3)]
    public required string UserName { get; set; }

    [Required]
    [MinLength(3)]
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

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

public class VerifyAuthTokenDto
{
    [Required]
    public required string Token { get; set; }

    [MinLength(6)]
    [MaxLength(6)]
    public required int Otp { get; set; }

    [EnumDataType(typeof(TokenUsageTypeEnum))]
    public int Type { get; set; } = (int)TokenUsageTypeEnum.login;
}

public class ChangePasswordDto
{
    [Required]
    public required string Token { get; set; }

    [Required]
    [MinLength(AppConstants.PasswordMinLength)]
    [RegularExpression(AppConstants.PasswordRegularExp)]
    public required string Password { get; set; }
}

public class ForgetPasswordDto
{
    public required string Email { get; set; }
}

public class ResultloginDto
{
    public required int Id { get; set; }
    public required string UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Status { get; set; }
    public string? Email { get; set; }
    public DateTime LastLogin { get; set; }
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string Token { get; set; }
    public string? Photo { get; set; }
}

public class CreateAuthTokenDto
{
    public int? UserId { get; set; }

    public string? Email { get; set; }

    [EnumDataType(typeof(TokenUsageTypeEnum))]
    public int UsageType { get; set; }
}

public class ResultForgetPasswordDto
{
    public string? OtpToken { get; set; }

    public string? AccessToken { get; set; }

    public required string Message { get; set; }
}

public class ForgetPasswordEmailDto
{
    public required string UserName { get; set; }
    public string? Link { get; set; }
    public int? Otp { get; set; }
}

public class PassWordGeneratedDto
{
    public required string PasswordHash { get; set; }

    public required string PasswordSalt { get; set; }
}