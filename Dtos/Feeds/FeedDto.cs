using System.ComponentModel.DataAnnotations;
using Api.DTOs;

namespace API.DTOs;

public class CreateFeedDto
{
    [MinLength(2)]
    public required string Message { get; set; }
    public IFormFileCollection? Images { get; set; }
    public IFormFile? VideoLink { get; set; }
}

public class FeedResultDto
{
    public required int Id { get; set; }
    public string? Message { get; set; }
    public int Likes { get; set; }
    public int Views { get; set; }
    public List<ResultFeedFileDto>? FeedFiles { get; set; }
    public List<ResultFeedCommentDto>? FeedComments { get; set; }
    public required ResultUserFeedDto User { get; set; }
    public List<ResultFeedLike>? FeedLikes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateFeedDto 
{
    [MinLength(2)]
    public required string Message { get; set; }
}