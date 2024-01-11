using System.Security.Claims;

namespace API.Interfaces
{
    public interface IUserService
    {
        string GetConnectedUser(ClaimsPrincipal User);

        bool IsUserConnected(ClaimsPrincipal User);
    }
}