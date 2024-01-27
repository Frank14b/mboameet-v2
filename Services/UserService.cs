using System;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        private readonly IMailService _mailService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            DataContext dataContext,
            IMailService mailService,
            ILogger<UserService> logger
        )
        {
            _dataContext = dataContext;
            _mailService = mailService;
            _logger = logger;
        }

        public string GetConnectedUser(ClaimsPrincipal User)
        {
            try
            {
                ClaimsPrincipal currentUser = User;
                var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetConnectedUser ${message}", ex.Message);
                throw;
            }
        }

        public bool IsUserConnected(ClaimsPrincipal User)
        {
            try
            {
                ClaimsPrincipal currentUser = User;
                var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null) return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("IsUserConnected ${message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> IsUserAlreadyExist(CreateUserDto data)
        {
            try
            {
                var query = _dataContext.Users.Where(u => u.Status == (int)StatusEnum.enable && (u.Email == data.Email || u.UserName == data.Username));
                var user = await query.FirstOrDefaultAsync();

                if (user == null) return true;

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("IsUserAlreadyExist ${message}", ex.Message);
                throw;
            }
        }

        public async Task<AppAuthToken?> CreateAuthToken(CreateAuthTokenDto data)
        {
            try
            {
                if (data?.UserId == null && data?.Email == null) return null;

                string otp = await GenerateAuthToken();
                string token = Guid.NewGuid().ToString();

                var authToken = new AppAuthToken
                {
                    Otp = int.Parse(otp),
                    Token = token,
                    Email = data?.Email ?? "",
                    UserId = data?.UserId,
                    UsageType = data?.UsageType ?? 0
                };

                await _dataContext.AuthTokens.AddAsync(authToken);  // Add to database directly
                await _dataContext.SaveChangesAsync();

                return authToken;
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateAuthToken ${message}", ex.Message);
                return null;
            }
        }

        public async Task<AppAuthToken?> AuthTokenIsValid(int? otp, string token, int type, int status = (int)StatusEnum.enable)
        {
            try
            {
                AppAuthToken? existingToken = null;

                if (otp != null)
                {
                    existingToken = await _dataContext.AuthTokens.FirstOrDefaultAsync(t => t.Token == token && t.Otp == otp && t.UsageType == type && t.Status == status && t.ExpireAt > DateTime.UtcNow);
                }
                else
                {
                    existingToken = await _dataContext.AuthTokens.FirstOrDefaultAsync(t => t.Token == token && t.UsageType == type && t.Status == status && t.ExpireAt > DateTime.UtcNow);
                }

                return existingToken;
            }
            catch (Exception ex)
            {
                _logger.LogError("AuthTokenIsValid ${message}", ex.Message);
                return null;
            }
        }

        private async Task<string> GenerateAuthToken()
        {
            string otpCode = string.Concat("", Enumerable.Range(0, 9).Select(_ => RandomNumberGenerator.GetBytes(1)[0]));

            var existingToken = await _dataContext.AuthTokens.FirstOrDefaultAsync(a => a.Otp == int.Parse(otpCode));

            if (existingToken != null)
            {
                otpCode = await GenerateAuthToken();
            }

            return otpCode;
        }

        public async Task<AppUser?> GetUserByEmail(string email)
        {
            return await _dataContext.Users.FirstOrDefaultAsync((x) => x.Email != null && x.Email.ToLower() == email.ToLower());
        }

        public PassWordGeneratedDto GeneratePassword(string password)
        {
            // encrypt password and return the hashed value and the salt
            using var hmac = new HMACSHA512();

            byte[] PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            byte[] PasswordSalt = hmac.Key;

            return new()
            {
                PasswordHash = PasswordHash,
                PasswordSalt = PasswordSalt
            };
        }

        public async Task<bool> UserIdExist(string id)
        {
            var result = await _dataContext.Users.AnyAsync(x => x.Status == (int)StatusEnum.enable && x.Id.ToString() == id);
            return result;
        }

        public async Task<bool> UserNameExist(string username, string? userId)
        {
            if (userId != null)
            {
                return await _dataContext.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower() && x.Id.ToString() != userId);
            }

            return await _dataContext.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }

        public async Task<bool> UserEmailExist(string useremail, string? userId)
        {
            if (userId != null)
            {
                return await _dataContext.Users.AnyAsync((x) => x.Email != null && x.Email.ToLower() == useremail.ToLower() && x.Id.ToString() != userId);
            }

            return await _dataContext.Users.AnyAsync((x) => x.Email != null && x.Email.ToLower() == useremail.ToLower());
        }

        public async Task<AppUser?> GetUserById(string id)
        {
            try
            {
                var result = await _dataContext.Users.Where(x => x.Status == (int)StatusEnum.enable && x.Id.ToString() == id).FirstAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetUserById ${message}", ex);
                return null;
            }
        }

        public bool IsValidPassword(string password)
        {
            // Validate strong password
            Regex validateGuidRegex = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            var test = validateGuidRegex.IsMatch(password);
            return test;
        }

        public bool UserPasswordIsValid(byte[] passwordSalt, byte[] passwordHash, string password)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i]) return false;
            }

            return true;
        }

        public async Task<bool> CheckGoogleAuthToken(string token = "", string urlHost = "")
        {
            try
            {
                string url = urlHost + "/tokeninfo?id_token=" + token;

                var client = new HttpClient();
                var result = await client.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<AppUser?> CreateUserAccount(RegisterDto? data)
        {
            try
            {
                PassWordGeneratedDto password = GeneratePassword(data?.Password ?? "");

                var user = new AppUser
                {
                    UserName = data?.Username ?? "",
                    PasswordHash = password.PasswordHash,
                    PasswordSalt = password.PasswordSalt,
                    FirstName = data?.Firstname,
                    LastName = data?.Lastname,
                    Email = data?.Email,
                    Age = 18,
                    Role = (int)RoleEnum.user,
                    Status = (int)StatusEnum.enable,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _dataContext.Users.AddAsync(user);
                await _dataContext.SaveChangesAsync();

                _ = _mailService.SendEmailAsync(new EmailRequestDto
                {
                    ToEmail = user?.Email ?? "",
                    ToName = user?.FirstName ?? "",
                    SubTitle = "User Registration",
                    ReplyToEmail = "",
                    Subject = "User Registration",
                    Body = _mailService.UserRegisterBody(user),
                    Attachments = { }
                });

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateUserAccount ${message}", ex.Message);
                return null;
            }
        }

        public async Task<AppUser?> AuthenticateUser(LoginDto data)
        {
            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => ((x.UserName == data.Username) || (x.Email == data.Username)) && x.Role != (int)RoleEnum.suadmin && x.Status != (int)StatusEnum.delete);

                if (user?.PasswordSalt != null)
                {
                    if (!UserPasswordIsValid(user.PasswordSalt, user.PasswordHash, data.Password)) return null;

                    _ = _mailService.SendEmailAsync(new EmailRequestDto
                    {
                        ToEmail = user?.Email ?? "",
                        ToName = user?.FirstName ?? "",
                        SubTitle = "Login Attempts",
                        ReplyToEmail = "",
                        Subject = "Login Attempts",
                        Body = _mailService.UserLoginBody(user),
                        Attachments = { }
                    });

                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Authentication ${message}", ex.Message);
                return null;
            }
        }

        public async Task<BooleanReturnDto?> DeleteUserAccount(DeleteProfile data, string userId)
        {
            try
            {
                AppUser? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
                
                if (user == null) return null;
                
                if (!UserPasswordIsValid(user.PasswordSalt, user.PasswordHash, data.Password)) {
                    return new BooleanReturnDto {
                        Status = false,
                        Message = "Invalid current password",
                    };
                }

                user.Status = (int)StatusEnum.delete;
                user.Email = AppConstants.Deletedkeyword + user.Email;
                user.UserName = AppConstants.Deletedkeyword + user.UserName;
                user.FirstName = "";
                user.LastName = "";
                
                await _dataContext.SaveChangesAsync();

                _ = _mailService.SendEmailAsync(new EmailRequestDto
                {
                    ToEmail = user?.Email ?? "",
                    ToName = user?.FirstName ?? "",
                    SubTitle = "Account Deleted",
                    ReplyToEmail = "",
                    Subject = "Account Delition Confirmation",
                    Body = _mailService.DeleteAccountBody(user),
                    Attachments = { }
                });
                
                return new BooleanReturnDto {
                    Status = true,
                    Message = "Your account has been deleted",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete user account ${message}", ex.Message);
                return null;
            }
        }
    }
}