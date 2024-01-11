
using API.Entities;
using MongoDB.Bson;

namespace API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(String Id, int Role);

        string CreateAdminToken(AppUser user);
    }
}