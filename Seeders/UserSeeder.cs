using API.Data;
using API.DTOs;
using API.Interfaces;

namespace API.Seeders;

public class UserSeeder
{
    private readonly DataContext _context;
    private readonly IUserService _userService;
    public UserSeeder(DataContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;

        SeedUsersAsync();
    }

    public async void SeedUsersAsync()
    {
        try
        {
            // if (!_context.Users.Any())
            // {
                await _userService.CreateUserAccount(new RegisterDto() {
                    UserName = "test",
                    FirstName = "test",
                    LastName = "test",
                    Email = "test@example.com",
                    Password = "33@Elrangers",
                });
            // }
        }
        catch (Exception)
        { }
    }
}
