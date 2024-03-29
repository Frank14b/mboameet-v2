
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using MongoDB.Bson;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly SymmetricSecurityKey _adminKey;

        // private readonly IHostEnvironment? env;

        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"] ?? ""));
            _adminKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["AdminTokenKey"] ?? ""));
        }
        public string CreateToken(int Id, int Role, bool authToken)
        {
            var user_id = Id;
            var role_id = Role;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, $"{user_id}"),
                new("RoleId", $"{role_id}"),
                new("Type", "User"),
                new("Auth", authToken ? "Yes" : "No")
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateAndTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string CreateAdminToken(User user)
        {
            var user_id = user.Id;
            var role_id = user.Role;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, user_id.ToString()),
                new("RoleId", role_id.ToString()),
                new("Type", "Admin")
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateAndTime.Now.AddDays(1),
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}