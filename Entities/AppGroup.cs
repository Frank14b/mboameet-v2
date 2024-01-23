using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities;

[Table("Groups")]
public class AppGroup
{
    [BsonId]
    public ObjectId Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public required string Type { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    [EnumDataType(typeof(StatusEnum))]
    public int Status { get; set; } = (int)StatusEnum.enable;

    [BsonId]
    [Required]
    public required ObjectId UserId { get; set; }

    public List<string>? Files { get; set; }
}