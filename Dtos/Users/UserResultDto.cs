
// using API.DTOs.Business;

using API.Entities;

namespace API.DTOs
{
    public class ResultloginDto
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Status { get; set; }
        public string? Email { get; set; }
        public DateTime LastLogin { get; set; }
        public int Age { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required string Token { get; set; }
    }

    public class ResultDeleteUserDto
    {
        public bool Status { get; set; }

        public required string Message { get; set; }
    }

    public class ResultUpdateUserDto
    {
        public required string Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Status { get; set; }
        public string? Email { get; set; }
        public DateTime LastLogin { get; set; }
        public int Age { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ResultAllUserDto
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Status { get; set; }
        public string? Email { get; set; }
        public DateTime LastLogin { get; set; }
        public int Age { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<MatchesResultDto>? Match { get; set; }
    }

    public class ResultUsersPaginate
    {
        public required IEnumerable<ResultAllUserDto> Data { get; set; }
        public required int Limit { get; set; }
        public required int Skip { get; set; }
        public required int Total { get; set; }
    }

    public class TotalUsersDto
    {
        public int Employees { get; set; }
        public int All { get; set; }
    }

    public class ResultForgetPasswordDto
    {
        public string? OtpToken { get; set; }

        public string? AccessToken {get; set;}

        public required string Message { get; set; }
    }
}