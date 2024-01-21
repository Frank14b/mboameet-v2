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

        public UserService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public string GetConnectedUser(ClaimsPrincipal User)
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
                string token = Guid uniqueGuid = Guid.NewGuid();

                var data = new AppAuthtoken{
                    Otp = otp,
                    Token = token,
                    Email = data?.Email,
                    UserId = data?.UserId,
                    UsageType = data.UsageType
                }
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<string> GenerateAuthToken() {
            string otpCode = string.Join("", Enumerable.Range(0, 9)
                      .Select(_ => RandomNumberGenerator.Create().GetBytes(1)[0].ToString()));

            var query = await _dataContext.AuthTokens.Where(a => a.otp == otpCode).FirstAsync();

            if(query != null) {
                otpCode = await GenerateAuthToken();
            }

            return otpCode;
        }

        public async Task<AppUser?> GetUserByEmail(string email)
        {
            return await _context.Users.AnyAsync((x) => x.Email != null && x.Email.ToLower() == email.ToLower());
        }
    }
}