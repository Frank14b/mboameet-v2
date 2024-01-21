using System.Data.Common;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        public readonly ILogger _logger;

        public UserService(DataContext dataContext)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public string GetConnectedUser(ClaimsPrincipal User, ILogger logger)
        {
            try
            {
                ClaimsPrincipal currentUser = User;
                var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

                return userId;
            }
            catch (Exception)
            {
                return "";
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
            catch (Exception)
            {
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
            catch (Exception)
            {
                return true;
            }
        }

        public async Task<AppAuthtoken?> CreateAuthToken (CreateAuthTokenDto data) {    
            try
            {
                if(data?.UserId == null && data?.Email == null) return null;

                int otp = (int)(await GenerateAuthToken());
                string token = Guid uniqueGuid = Guid.NewGuid().ToString();

                var authToken = new AppAuthtoken{
                    Otp = otp,
                    Token = token,
                    Email = data?.Email,
                    UserId = data?.UserId,
                    UsageType = data.UsageType
                }

                await _dataContext.AuthTokens.AddAsync(authToken);  // Add to database directly
                await _dataContext.SaveChangesAsync();

                return authToken;
            }
            catch (Exception)
            {
                _logger.LogError(ex, "Failed to create auth token");
                return null;
            }
        }

        private async Task<string> GenerateAuthToken() {
            string otpCode = string.Concat("", Enumerable.Range(0, 9).Select(_ => RandomNumberGenerator.Create().GetBytes(1)[0]));

            var existingToken = await _dataContext.AuthTokens.FirstOrDefaultAsync(a => a.Otp == (int)otpCode);

            if(existingToken != null) {
                existingToken = await GenerateAuthToken();
            }

            return otpCode;
        }

        public async Task<AppUser?> GetUserByEmail(string email)
        {
            return await _context.Users.AnyAsync((x) => x.Email != null && x.Email.ToLower() == email.ToLower());
        }
    }
}