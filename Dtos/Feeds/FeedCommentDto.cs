namespace API.DTOs;

public class ResultFeedCommentDto
{
    public int Id { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    
    public required ResultUserFeedDto User { get; set; }
}

public class CreateCommentDto {
    public required string Context { get; set; }
}