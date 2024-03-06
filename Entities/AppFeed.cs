using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
 
namespace API.Entities;

[Table("feeds")]
public class AppFeed
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string? Message { get; set; }

    public List<AppImage>? Images { get; set; }

    public string? VideoLink { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    [EnumDataType(typeof(StatusEnum))]
    public int Status { get; set; } = (int) StatusEnum.enable;
    
    public ObjectId UserId { get; set; }
}