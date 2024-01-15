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
        [BsonRepresentation(BsonType.ObjectId)]
        public required ObjectId Id { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required ObjectId User { get; set; }

        // public List<AppUser> Users { get; set; } = new();

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId MatchedUser { get; set; }

        [EnumDataType(typeof(MatchStateEnum))]
        public required int State { get; set; }

        [EnumDataType(typeof(StatusEnum))]
        public required int Status { get; set; }
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