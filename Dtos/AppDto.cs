using API.Entities;

namespace API.DTOs
{
    public class BooleanReturnDto
    {
        public required bool Status { get; set; }
        public AppMatch? Data { get; set; }
        public string? Message { get; set; }
    }
}