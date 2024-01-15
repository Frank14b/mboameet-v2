using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    }
}