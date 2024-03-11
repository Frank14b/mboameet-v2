using API.Entities;
using Org.BouncyCastle.Bcpg;

namespace API.DTOs;

public class CreateFeedDto
{
    public string? Message { get; set; }
    public IFormFileCollection? Images { get; set; }
    public IFormFile? VideoLink { get; set; }
}

public class FeedResultDto
{
    public required int Id { get; set; }
    public string? Message { get; set; }
    public ICollection<FeedFiles>? FeedFiles { get; set; }
    public int Status { get; set; }
    public required ResultUserDto User { get; set; }
    public required int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}