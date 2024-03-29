using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace API.Entities
{
    [Table("users")]
    [Index(nameof(Email), nameof(UserName), IsUnique = true)]
    [Index(nameof(Status), nameof(Role))]
    public class User
    {
        public int Id { get; set; }

        [MinLength(3)]
        public required string UserName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [EnumDataType(typeof(StatusEnum))]
        public int Status { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

        public required string PasswordSalt { get; set; }

        [EnumDataType(typeof(RoleEnum))]
        public int Role { get; set; }

        public int Age { get; set; }

        public ICollection<Match>? Match { get; set; }

        public ICollection<Feed>? Feeds { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        public int IsVerified { get; set; }

        public string? Photo { get; set; }
    }

    public enum RoleEnum: int
    {
        user = 2, // Properties Admin
        suadmin = 1, // Super admin 
        custom = 3 // Other User 
    }
}
