using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using API.DTOs;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.EntityFrameworkCore;

namespace API.Entities
{
    [Table("users")]
    [Index(nameof(Email), nameof(UserName), IsUnique = true)]
    public class AppUser
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [MinLength(3)]
        public required string UserName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [EnumDataType(typeof(StatusEnum))]
        public int Status { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public required byte[] PasswordHash { get; set; }

        public required byte[] PasswordSalt { get; set; }

        [EnumDataType(typeof(RoleEnum))]
        public int Role { get; set; }

        public int Age { get; set; }

        public ICollection<AppMatch>? Match { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        public bool IsVerified { get; set; } = false;

        public string? Photo {get; set;}
    }

    public enum RoleEnum
    {
        user = 2, // Properties Admin
        suadmin = 1, // Super admin 
        custom = 3 // Other User 
    }
}
