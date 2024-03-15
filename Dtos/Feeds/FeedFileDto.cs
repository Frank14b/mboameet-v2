namespace API.DTOs;

public class ResultFeedFileDto
{
    public int Id { get; set; }

    public required string Url { get; set; }
    
    public required string PreviewUrl { get; set; }

    public required string Type { get; set; }

    public required string DisplayMode {get; set;}

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}