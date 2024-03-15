using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ResultUserFeedDto
{
    public required int Id { get; set; }
    public required string UserName { get; set; }
    public required string FirstName { get; set; } = "";
    public string? LastName { get; set; } = "";

    [EmailAddress]
    public string? Email { get; set; } = "";
    public string? Photo { get; set; }
    
}