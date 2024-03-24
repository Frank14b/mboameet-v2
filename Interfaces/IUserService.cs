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
    Task<AuthToken?> CreateAuthToken(CreateAuthTokenDto data);
    Task<User?> GetUserByEmail(string email);
    PassWordGeneratedDto GeneratePassword(string password);
    Task<AuthToken?> AuthTokenIsValid(int? otp, string token, int type, int status = (int)StatusEnum.enable);
    Task<bool> UserIdExist(int id);
    Task<bool> UserNameExist(string username, int? userId);
    Task<bool> UserEmailExist(string useremail, int? userId);
    Task<User?> GetUserByIdAsync(int id);
    Task<ResultPaginate<ResultUserDto>> GetUsersAsync(int currentUserId, int page, int limit, string sort);
    bool IsValidPassword(string password);
    Task<bool> CheckGoogleAuthToken(string token = "", string urlHost = "");
    bool UserPasswordIsValid(string passwordSalt, string passwordHash, string password);
    Task<User?> CreateUserAccount(RegisterDto? data);
    Task<User?> AuthenticateUser(LoginDto data);
    Task<BooleanReturnDto?> DeleteUserAccount(DeleteProfile data, int userId);
    Task<ResultForgetPasswordDto?> ForgetPassword(User user);
    Task<BooleanReturnDto?> ChangeForgetPassword(User user, ChangePasswordDto data);
    string? GetUserIpAddress(HttpContext httpContext);
    Task<ResultUserDto?> UpdateProfileImage(User user, [FromForm] IFormFile image, string folder);
}
