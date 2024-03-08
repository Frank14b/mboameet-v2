
using API.Entities;

namespace API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(int Id, int Role, bool authToken);

        string CreateAdminToken(AppUser user);
    }
}