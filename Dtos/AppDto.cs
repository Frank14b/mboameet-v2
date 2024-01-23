namespace API.DTOs;

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

public enum EnumMessageType
{
    text = 0,
    voice = 1,
    file = 2,
    sticker = 3,
    callnotification = 4
}

public class ResultPaginate
{
    public required IEnumerable<dynamic> Data { get; set; }
    public required int Limit { get; set; }
    public required int Skip { get; set; }
    public required int Total { get; set; }
}