using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.DTOs
{
    public class AddMatchDto
    {
        [Required]
        public required string MatchedUser { get; set; }
    }

    public class MatchesResultDto
    {
        public required string Id { get; set; }

        public required string User { get; set; }

        public required string MatchedUser { get; set; }

        [EnumDataType(typeof(MatchStateEnum))]
        public required int State { get; set; }

        [EnumDataType(typeof(StatusEnum))]
        public required int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class MatchesPaginateResultDto
    {
        public required IEnumerable<MatchesResultDto> Data { get; set; }
        public required int Limit { get; set; }
        public required int Skip { get; set; }
        public required int Total { get; set; }
    }

    public class MatchesRequestAction
    {
        public enum Actions
        {
            approved = 0,
            declined = 1
        }
    }
}