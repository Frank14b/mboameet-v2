using API.Entities;

namespace API.DTOs
{
    public class BooleanReturnDto
    {
        public required bool Status { get; set; }
        public dynamic? Data { get; set; }
        public string? Message { get; set; }
    }

    public enum StatusEnum
    {
        disable = 0,
        enable = 1,
        delete = 2
    }

    public enum TokenUsageTypeEnum
    {
        login = 0,
        forgetPassword = 1,
        resetPassword = 2,
    }
}