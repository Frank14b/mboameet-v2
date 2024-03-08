using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;

namespace API.Entities
{
    [Table("matches")]
    public class AppMatch
    {
        public int Id { get; set; }

        public required int UserId { get; set; }

        public AppUser? User { get; set; }

        public required int MatchedUserId { get; set; }

        public AppUser? MatchedUser { get; set; }

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