using System.Security.Claims;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IUserService
    {
        string GetConnectedUser(ClaimsPrincipal User);

        bool IsUserConnected(ClaimsPrincipal User);

        Task<bool> IsUserAlreadyExist(CreateUserDto data);

        Task<AppAuthToken?> CreateAuthToken(CreateAuthTokenDto data);

        Task<AppUser?> GetUserByEmail(string email);
    }
}