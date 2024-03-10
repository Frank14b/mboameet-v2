using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities;

[Table("groups")]
public class Group
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public required string Type { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    [EnumDataType(typeof(StatusEnum))]
    public int Status { get; set; } = (int)StatusEnum.enable;

    [Required]
    public required int UserId { get; set; }
}