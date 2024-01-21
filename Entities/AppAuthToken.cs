using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities {

    [Table("AuthTokens")]
    public class AppAuthToken
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public ObjectId? UserId { get; set; }

        public string? Email {get; set;}

        public required int Otp {get; set;}

        public required string Token {get; set;}

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        [EnumDataType(typeof(StatusEnum))]
        public int Status { get; set; } = (int)StatusEnum.enable;

        [EnumDataType(typeof(TokenUsageTypeEnum))]
        public int UsageType {get; set;} = (int)TokenUsageTypeEnum.login;
    }
}