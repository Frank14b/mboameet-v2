using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class DeleteUserDto
{
    [Required]
    public required int Id { get; set; }
}

public class ResultDeleteUserDto
{
    public bool Status { get; set; }

    public required string Message { get; set; }
}

public class CreateUserDto
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
    public string? Email { get; set; }
}

public class ResultUserDto
{
    public required int Id { get; set; }
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
    public string? Photo { get; set; }
}

public class DeleteProfile
{
    [Required]
    [MinLength(AppConstants.PasswordMinLength)]
    [RegularExpression(AppConstants.PasswordRegularExp)]
    public required string Password { get; set; }
}
