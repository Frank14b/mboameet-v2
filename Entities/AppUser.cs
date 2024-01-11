using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace API.Entities
{
    [Table("Users")]
    public class AppUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [MinLength(3)]
        public required string UserName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [EnumDataType(typeof(StatusEnum))]
        public int Status { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public required byte[] PasswordHash { get; set; }

        public required byte[] PasswordSalt { get; set; }

        [EnumDataType(typeof(RoleEnum))]
        public int Role { get; set; }

        public int Age { get; set; }

        // public List<AppBusiness> Business { get; set; } = new();
        // public List<AppUserProperty> UserProperties { get; set; } = new();

        public DateTime LastLogin { get; set; }

        // public DateTime DateOfBirth { get; set; } = DateTime.DateTime;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }
    }

    public enum RoleEnum
    {
        user = 2, // Properties Admin
        suadmin = 1, // Super admin 
        custom = 3 // Other User 
    }

    public enum StatusEnum
    {
        disable = 0,
        enable = 1,
        delete = 2
    }
}
