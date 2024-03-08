using API.Data;
using API.DTOs;
using API.Entities;
using System.Security.Cryptography;
using System.Text;

namespace API.Seeders;

public class UserSeeder
{
    private readonly DataContext _context;
    public UserSeeder(DataContext context)
    {
        _context = context;
    }

    public void SeedData()
    {
        try
        {
            if (!_context.Users.Any())
            {
                using var hmac = new HMACSHA512();

                // var user = new AppUser
                // {
                //     UserName = "frank",
                //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("33@Elrangers")),
                //     PasswordSalt = hmac.Key,
                //     FirstName = "Frank",
                //     LastName = "Fontcha",
                //     Email = "franckfontcha@gmail.com",
                //     Role = (int)RoleEnum.user,
                //     Status = (int)StatusEnum.enable,
                //     CreatedAt = DateTime.UtcNow,
                //     UpdatedAt = DateTime.UtcNow,
                // };

                // _context.Users.Add(user);

                // _context.SaveChanges();
            }
        }
        catch (Exception)
        { }
    }
}
