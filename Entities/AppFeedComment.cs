using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
 
namespace API.Entities;

[Table("feedcomments")]
public class FeedComment
{
    public int Id { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    [EnumDataType(typeof(StatusEnum))]
    public int Status { get; set; } = (int) StatusEnum.enable;
    
    public int UserId { get; set; }

    public int FeedId { get; set; }
}