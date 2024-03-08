using System.Security.Claims;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces;

public interface IUserService
{
    int GetConnectedUser(ClaimsPrincipal User);
    bool IsUserConnected(ClaimsPrincipal User);
    Task<bool> IsUserAlreadyExist(CreateUserDto data);
    Task<AppAuthToken?> CreateAuthToken(CreateAuthTokenDto data);
    Task<AppUser?> GetUserByEmail(string email);
    PassWordGeneratedDto GeneratePassword(string password);
    Task<AppAuthToken?> AuthTokenIsValid(int? otp, string token, int type, int status = (int)StatusEnum.enable);
    Task<bool> UserIdExist(int id);
    Task<bool> UserNameExist(string username, int? userId);
    Task<bool> UserEmailExist(string useremail, int? userId);
    Task<AppUser?> GetUserById(int id);
    Task<List<AppUser>> GetUsers();
    bool IsValidPassword(string password);
    Task<bool> CheckGoogleAuthToken(string token = "", string urlHost = "");
    bool UserPasswordIsValid(byte[] passwordSalt, byte[] passwordHash, string password);
    Task<AppUser?> CreateUserAccount(RegisterDto? data);
    Task<AppUser?> AuthenticateUser(LoginDto data);
    Task<BooleanReturnDto?> DeleteUserAccount(DeleteProfile data, int userId);
    Task<ResultForgetPasswordDto?> ForgetPassword(AppUser user);
    Task<BooleanReturnDto?> ChangeForgetPassword(AppUser user, ChangePasswordDto data);
    string? GetUserIpAddress(HttpContext httpContext);
    Task<ResultUserDto?> UpdateProfileImage(AppUser user, [FromForm] IFormFile image, string folder);
}
