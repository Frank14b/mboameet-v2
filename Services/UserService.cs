using System;
using System.Data.Common;
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
        private readonly ILogger _logger;

        public UserService(DataContext dataContext, ILogger logger)
        {
            _dataContext = dataContext;
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
                _logger.LogError("Error when claiming current user", ex.Message);
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
                _logger.LogError("Error when claiming current user", ex.Message);
                return false;
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
                _logger.LogError("Error when checking if user exist", ex.Message);
                return true;
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
                _logger.LogError("Error when creating auth token", ex.Message);
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
                _logger.LogError("Error when validating auth token", ex.Message);
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
            var result = await _dataContext.Users.Where(x => x.Status == (int)StatusEnum.enable || x.Id.ToString() == id).AnyAsync();
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

        public AppUser? GetUserById(string id)
        {
            var result = _dataContext.Users.Where(x => x.Status == (int)StatusEnum.enable || x.Id.ToString() == id).FirstOrDefault();
            return result;
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
    }
}