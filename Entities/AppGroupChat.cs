using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities;

[Table("GroupChats")]
public class AppGroupChat
{
    [BsonId]
    public ObjectId Id { get; set; }

    [Required]
    public required string Message { get; set; }

    [EnumDataType(typeof(EnumMessageType))]
    public required int MessageType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    [EnumDataType(typeof(StatusEnum))]
    public int Status { get; set; }

    [BsonId]
    public ObjectId Sender { get; set; }

    [BsonId]
    public ObjectId GroupId { get; set; }

    public List<string>? Files { get; set; }
}