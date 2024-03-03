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

public class ResultPaginate<T>
{
    public required IEnumerable<T> Data { get; set; } // good practice
    public required int Limit { get; set; }
    public required int Skip { get; set; }
    public required int Total { get; set; }
}

public static class AppConstants
{
    public const int TokenValidity = 10;
    public const string Deletedkeyword = "deleted_";
    public const string PasswordRegularExp = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
    public const int PasswordMinLength = 8;
}

public class SingleFileUpload {
    public required IFormFile File {get; set;}
}