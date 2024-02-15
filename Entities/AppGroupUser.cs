using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities;

[Table("groupusers")]
public class AppGroupUser
{
    [BsonId]
    public ObjectId Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    [EnumDataType(typeof(StatusEnum))]
    public int Status { get; set; }

    [BsonId]
    public ObjectId UserId { get; set; }

    [BsonId]
    public ObjectId GroupId { get; set; }

    [BsonId]
    public ObjectId GroupAccesId { get; set; }
}