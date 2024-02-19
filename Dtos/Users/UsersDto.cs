using System.ComponentModel.DataAnnotations;
using API.Entities;
using MongoDB.Bson;

namespace API.DTOs;

public class LoginDto
{
    [Required]
    [MinLength(3)]
    public required string Username { get; set; }

    [Required]
    [MinLength(AppConstants.PasswordMinLength)]
    [RegularExpression(AppConstants.PasswordRegularExp)]
    public required string Password { get; set; }
}

public class RegisterDto
{
    [Required]
    [MinLength(3)]
    public required string Username { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

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
    [MinLength(AppConstants.PasswordMinLength)]
    [RegularExpression(AppConstants.PasswordRegularExp)]
    public required string CurrentPassword { get; set; }

    [RegularExpression(AppConstants.PasswordRegularExp)]
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

    [MinLength(5)]
    [EmailAddress]
    public string? Email { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class CreateUserDto
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
}

public class CreateAuthTokenDto
{
    public ObjectId? UserId { get; set; }

    public string? Email { get; set; }

    [EnumDataType(typeof(TokenUsageTypeEnum))]
    public int UsageType { get; set; }
}

public class ForgetPasswordDto
{
    public required string Email { get; set; }
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

public class ResultloginDto
{
    public required string Id { get; set; }
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
}

public class ResultDeleteUserDto
{
    public bool Status { get; set; }

    public required string Message { get; set; }
}

public class ResultUpdateUserDto
{
    public required string Id { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Status { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
    public DateTime LastLogin { get; set; }
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ResultUserDto
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required string FirstName { get; set; } = "";
    public string? LastName { get; set; } = "";
    public int Status { get; set; }

    [EmailAddress]
    public string? Email { get; set; } = "";
    public DateTime LastLogin { get; set; }

    [Range(18, 200)]
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<MatchesResultDto>? Match { get; set; }
}

public class ResultUsersPaginate
{
    public required IEnumerable<ResultUserDto> Data { get; set; }
    public required int Limit { get; set; }
    public required int Skip { get; set; }
    public required int Total { get; set; }
}

public class TotalUsersDto
{
    public int Employees { get; set; }
    public int All { get; set; }
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
    public required byte[] PasswordHash { get; set; }

    public required byte[] PasswordSalt { get; set; }
}

public class UpdateProfile
{
    [MinLength(3)]
    public string? UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [MinLength(AppConstants.PasswordMinLength)]
    [RegularExpression(AppConstants.PasswordRegularExp)]
    public string? Password { get; set; }

    [Range(18, 200)]
    public int? Age { get; set; }
}

public class DeleteProfile
{
    [Required]
    [MinLength(AppConstants.PasswordMinLength)]
    [RegularExpression(AppConstants.PasswordRegularExp)]
    public required string Password { get; set; }
}
