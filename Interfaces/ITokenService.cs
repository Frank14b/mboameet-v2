
using API.Entities;

namespace API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(string Id, int Role, bool authToken);

        string CreateAdminToken(AppUser user);
    }
}