using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;

namespace API.Entities;

[Table("feedfiles")]
public class AppFeedFiles
{
    public int Id { get; set; }

    public required string Url { get; set; }
    
    public required string PreviewUrl { get; set; }

    public required string Type { get; set; }

    public required string DisplayMode {get; set;}

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    [EnumDataType(typeof(StatusEnum))]
    public int Status { get; set; } = (int)StatusEnum.enable;

    public int FeedId { get; set; }
}