using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    [Table("Matches")]
    public class AppMatch
    {
        [BsonId]
        // [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        // [BsonId]
        public required ObjectId UserId { get; set; }

        public AppUser? User { get; set; }
        // [BsonId]
        public required ObjectId MatchedUserId { get; set; }

        // public AppUser? MatchedUser { get; set; }

        [EnumDataType(typeof(MatchStateEnum))]
        public int State { get; set; } = (int)MatchStateEnum.inititated;

        [EnumDataType(typeof(StatusEnum))]
        public int Status { get; set; } = (int)StatusEnum.enable;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }
    }

    public enum MatchStateEnum
    {
        inititated = 0,
        approved = 1,
        rejected = 2,
    }
}