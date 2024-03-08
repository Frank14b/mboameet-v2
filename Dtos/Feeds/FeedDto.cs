using API.Entities;

namespace API.DTOs;

public class CreateFeedDto
{
    public string? Message { get; set; }
    public List<AppImage>? Images { get; set; }
    public string? VideoLink { get; set; }
    public int Status { get; set; }
    public required int UserId { get; set; }
}

public class FeedResultDto
{
    public required int Id { get; set; }
    public string? Message { get; set; }
    public List<AppImage>? Images { get; set; }
    public string? VideoLink { get; set; }
    public int Status { get; set; }
    // public required string UserId { get; set; }
    public required ResultUserDto User {get; set;}
}