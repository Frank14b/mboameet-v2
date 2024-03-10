using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
 
namespace API.Entities;

[Table("feeds")]
public class Feed
{
    public int Id { get; set; }

    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    [EnumDataType(typeof(StatusEnum))]
    public int Status { get; set; } = (int) StatusEnum.enable;
    
    public int UserId { get; set; }

    public User? User {get; set;}

    public ICollection<FeedComment>? FeedComments {get; set;}

    public ICollection<FeedFiles>? FeedFiles { get; set; }
}