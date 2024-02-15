using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities;

[Table("authtokens")]
public class AppAuthToken
{
    [BsonId]
    public ObjectId Id { get; set; }

    public ObjectId? UserId { get; set; }

    public required string Email { get; set; }

    public required int Otp { get; set; }

    public required string Token { get; set; }

    public DateTime ExpireAt { get; set; } = DateTime.UtcNow.AddMinutes(AppConstants.TokenValidity);

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    [EnumDataType(typeof(StatusEnum))]
    public int Status { get; set; } = (int)StatusEnum.enable;

    [EnumDataType(typeof(TokenUsageTypeEnum))]
    public int UsageType { get; set; } = (int)TokenUsageTypeEnum.login;
}
