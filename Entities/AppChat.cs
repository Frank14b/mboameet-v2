using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities;

[Table("chats")]
public class Chat
{
    public int Id { get; set; }

    [Required]
    public required string Message { get; set; }

    [EnumDataType(typeof(EnumMessageType))]
    public required int MessageType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    [EnumDataType(typeof(StatusEnum))]
    public int Status { get; set; }

    public int Sender { get; set; }

    public int Receiver { get; set; }
}