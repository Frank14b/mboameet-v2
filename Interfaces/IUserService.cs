using System.Security.Claims;
using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IUserService
{
    string GetConnectedUser(ClaimsPrincipal User);
    bool IsUserConnected(ClaimsPrincipal User);
    Task<bool> IsUserAlreadyExist(CreateUserDto data);
    Task<AppAuthToken?> CreateAuthToken(CreateAuthTokenDto data);
    Task<AppUser?> GetUserByEmail(string email);
    PassWordGeneratedDto GeneratePassword(string password);
    Task<AppAuthToken?> AuthTokenIsValid(int? otp, string token, int type, int status = (int)StatusEnum.enable);
    Task<bool> UserIdExist(string id);
    Task<bool> UserNameExist(string username, string? userId);
    Task<bool> UserEmailExist(string useremail, string? userId);
    Task<AppUser?> GetUserById(string id);
    Task<List<AppUser>> GetUsers();
    bool IsValidPassword(string password);
    Task<bool> CheckGoogleAuthToken(string token = "", string urlHost = "");
    bool UserPasswordIsValid(byte[] passwordSalt, byte[] passwordHash, string password);
    Task<AppUser?> CreateUserAccount(RegisterDto? data);
    Task<AppUser?> AuthenticateUser(LoginDto data);
    Task<BooleanReturnDto?> DeleteUserAccount(DeleteProfile data, string userId);
    Task<ResultForgetPasswordDto?> ForgetPassword(AppUser user);
    Task<BooleanReturnDto?> ChangeForgetPassword(AppUser user, ChangePasswordDto data);
    string? GetUserIpAddress(HttpContext httpContext);
}
