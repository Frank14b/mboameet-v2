using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class SendMessageDto
    {
        [Required]
        public required string Message { get; set; }

        [EnumDataType(typeof(EnumMessageType))]
        public required int MessageType { get; set; }

        public required string Receiver { get; set; }

        public List<string>? Files { get; set; }
    }

    public class MessageResultDto
    {
        public required string Id { get; set; }

        [Required]
        public required string Message { get; set; }

        [EnumDataType(typeof(EnumMessageType))]
        public required int MessageType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        [EnumDataType(typeof(StatusEnum))]
        public int Status { get; set; }

        public required string Sender { get; set; }

        public required string Receiver { get; set; }

        public List<string>? Files { get; set; }
    }

    public class MessagePaginateResultDto
    {
        public required IEnumerable<MessageResultDto> Data { get; set; }
        public required int Limit { get; set; }
        public required int Skip { get; set; }
        public required int Total { get; set; }
    }
}