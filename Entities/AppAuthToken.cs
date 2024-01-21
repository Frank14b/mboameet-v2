namespace API.Entities {
[Table("AuthTokens")]
public class AppAuthToken
{
    [BsonId]
        public ObjectId Id { get; set; }

        public ObjectId? UserId { get; set; }

        public string? Email {get; set;}

        public int Otp {get; set;}

        public string Token {get; set;}

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        [EnumDataType(typeof(StatusEnum))]
        public int Status { get; set; } = StatusEnum.enable;

        [EnumDataType(typeof(TokenUsageTypeEnum))]
        public int UsageType {get; set;}
}
}