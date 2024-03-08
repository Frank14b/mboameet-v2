using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;

namespace API.Entities;

[Table("authtokens")]
public class AppAuthToken
{
    public int Id { get; set; }

    public int? UserId { get; set; }

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
