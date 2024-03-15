using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ResultUpdateUserDto
{
    public required int Id { get; set; }
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
    public string? Photo { get; set; }
}

public class UpdateStatusUserDto
{
    [Required]
    public required int Id { get; set; }

    [Required]
    [EnumDataType(typeof(StatusEnum))]
    public int Status { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class UpdateProfileDto
{
    [MinLength(1)]
    public required string UserName { get; set; }

    [MinLength(1)]
    public string? FirstName { get; set; }

    [MinLength(1)]
    public string? LastName { get; set; }

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
    public required int Id { get; set; }

    [MinLength(1)]
    public required string UserName { get; set; }

    [MinLength(1)]
    public string? FirstName { get; set; }

    [MinLength(1)]
    public string? LastName { get; set; }

    [MinLength(5)]
    [EmailAddress]
    public string? Email { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
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
    public string? Photo { get; set; }
}