using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace API.Services;

public class UserService : IUserService
{
    private readonly DataContext _dataContext;
    private readonly IMailService _mailService;
    private readonly ILogger<UserService> _logger;
    private readonly IAppFileService _appFileService;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public UserService(
        DataContext dataContext,
        IMailService mailService,
        ILogger<UserService> logger,
        ITokenService tokenService,
        IAppFileService appFileService,
        IMapper mapper
    )
    {
        _appFileService = appFileService;
        _dataContext = dataContext;
        _mailService = mailService;
        _logger = logger;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public int GetConnectedUser(ClaimsPrincipal User)
    {
        try
        {
            ClaimsPrincipal currentUser = User;
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            return int.Parse(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError("GetConnectedUser ${message}", ex.Message);
            throw;
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
        catch (Exception ex)
        {
            _logger.LogError("IsUserConnected ${message}", ex.Message);
            throw;
        }
    }

    public async Task<bool> IsUserAlreadyExist(CreateUserDto data)
    {
        try
        {
            var query = _dataContext.Users.Where(u => u.Status != (int)StatusEnum.delete && (u.Email == data.Email || u.UserName == data.UserName));
            var user = await query.FirstOrDefaultAsync();

            if (user == null) return true;

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError("IsUserAlreadyExist ${message}", ex.Message);
            throw;
        }
    }

    public async Task<AppAuthToken?> CreateAuthToken(CreateAuthTokenDto data)
    {
        try
        {
            if (data?.UserId == null && data?.Email == null) return null;

            string otp = await GenerateAuthToken();
            string token = Guid.NewGuid().ToString();

            var authToken = new AppAuthToken
            {
                Otp = int.Parse(otp),
                Token = token,
                Email = data?.Email ?? "",
                UserId = data?.UserId,
                UsageType = data?.UsageType ?? 0
            };

            await _dataContext.AuthTokens.AddAsync(authToken);  // Add to database directly
            await _dataContext.SaveChangesAsync();

            return authToken;
        }
        catch (Exception ex)
        {
            _logger.LogError("CreateAuthToken ${message}", ex);
            return null;
        }
    }

    public async Task<AppAuthToken?> AuthTokenIsValid(int? otp, string token, int type, int status = (int)StatusEnum.enable)
    {
        try
        {
            AppAuthToken? existingToken = null;

            if (otp != null)
            {
                existingToken = await _dataContext.AuthTokens.FirstOrDefaultAsync(t => t.Token == token && t.Otp == otp && t.UsageType == type && t.Status == status && t.ExpireAt > DateTime.UtcNow);
            }
            else
            {
                existingToken = await _dataContext.AuthTokens.FirstOrDefaultAsync(t => t.Token == token && t.UsageType == type && t.Status == status && t.ExpireAt > DateTime.UtcNow);
            }

            return existingToken;
        }
        catch (Exception ex)
        {
            _logger.LogError("AuthTokenIsValid ${message}", ex.Message);
            return null;
        }
    }

    private async Task<string> GenerateAuthToken()
    {
        string otpCode = string.Concat("", Enumerable.Range(0, 9).Select(_ => RandomNumberGenerator.GetBytes(1)[0]));

        var existingToken = await _dataContext.AuthTokens.FirstOrDefaultAsync(a => a.Otp == int.Parse(otpCode));

        if (existingToken != null)
        {
            otpCode = await GenerateAuthToken();
        }

        return otpCode;
    }

    public async Task<AppUser?> GetUserByEmail(string email)
    {
        return await _dataContext.Users.FirstOrDefaultAsync((x) => x.Email != null && x.Email.ToLower() == email.ToLower() && x.Status == (int)StatusEnum.enable);
    }

    public PassWordGeneratedDto GeneratePassword(string password)
    {
        // encrypt password and return the hashed value and the salt
        using var hmac = new HMACSHA512();

        byte[] PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        byte[] PasswordSalt = hmac.Key;

        return new()
        {
            PasswordHash = PasswordHash,
            PasswordSalt = PasswordSalt
        };
    }

    public async Task<bool> UserIdExist(int id)
    {
        var result = await _dataContext.Users.AnyAsync(x => x.Status == (int)StatusEnum.enable && x.Id == id);
        return result;
    }

    public async Task<bool> UserNameExist(string username, int? userId)
    {
        if (userId != null)
        {
            return await _dataContext.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower() && x.Id != userId);
        }

        return await _dataContext.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }

    public async Task<bool> UserEmailExist(string useremail, int? userId)
    {
        if (userId != null)
        {
            return await _dataContext.Users.AnyAsync((x) => x.Email != null && x.Email.ToLower() == useremail.ToLower() && x.Id != userId);
        }

        return await _dataContext.Users.AnyAsync((x) => x.Email != null && x.Email.ToLower() == useremail.ToLower());
    }

    public async Task<AppUser?> GetUserById(int id)
    {
        try
        {
            var result = await _dataContext.Users.Select(u => new AppUser
            {
                Id = u.Id,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Status = u.Status,
                Email = u.Email,
                PasswordHash = u.PasswordHash,
                PasswordSalt = u.PasswordSalt,
                Photo = null
            }).Where(x => x.Status == (int)StatusEnum.enable && x.Id == id).FirstAsync();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error GetUserById ${message}", ex);
            return null;
        }
    }

    public async Task<List<AppUser>> GetUsers()
    {
        var result = await _dataContext.Users.Where(x => x.Status == (int)StatusEnum.enable).ToListAsync();
        return result;
    }

    public bool IsValidPassword(string password)
    {
        // Validate strong password
        Regex validateGuidRegex = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
        var test = validateGuidRegex.IsMatch(password);
        return test;
    }

    public byte[] ConvertHexToByte(string hexString)
    {
        string[] byteStringArray = hexString.Split('-');

        // Create a byte array to store the converted values
        byte[] byteArray = new byte[byteStringArray.Length];

        // Convert each substring to a byte and store it in the array
        for (int i = 0; i < byteStringArray.Length; i++)
        {
            byteArray[i] = Convert.ToByte(byteStringArray[i], 16);
        }

        return byteArray;
    }

    public bool UserPasswordIsValid(byte[] passwordSalt, byte[] passwordHash, string password)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        Console.WriteLine(passwordSalt[0]);
        Console.WriteLine(computedHash[1]);

        // Console.WriteLine(ConvertHexToByte(passwordSalt)[0]);
        // Console.WriteLine(ConvertHexToByte(passwordSalt)[1]);

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != passwordHash[i]) return false;
        }

        return true;
    }

    public async Task<bool> CheckGoogleAuthToken(string token = "", string urlHost = "")
    {
        try
        {
            string url = urlHost + "/tokeninfo?id_token=" + token;

            var client = new HttpClient();
            var result = await client.GetAsync(url);

            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<AppUser?> CreateUserAccount(RegisterDto? data)
    {
        try
        {
            PassWordGeneratedDto password = GeneratePassword(data?.Password ?? "");

            var user = new AppUser
            {
                UserName = data?.UserName ?? "",
                PasswordHash = password.PasswordHash,
                PasswordSalt = password.PasswordSalt,
                FirstName = data?.FirstName,
                LastName = data?.LastName,
                Email = data?.Email ?? "",
                Age = 18,
                Role = (int)RoleEnum.user,
                Status = (int)StatusEnum.enable,
                UpdatedAt = DateTime.UtcNow,
            };

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            _ = _mailService.SendEmailAsync(new EmailRequestDto
            {
                ToEmail = user?.Email ?? "",
                ToName = user?.FirstName ?? "",
                SubTitle = "User Registration",
                ReplyToEmail = "",
                Subject = "User Registration",
                Body = _mailService.UserRegisterBody(user),
                Attachments = { }
            });

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError("CreateUserAccount ${message}", ex.Message);
            return null;
        }
    }

    public async Task<AppUser?> AuthenticateUser(LoginDto data)
    {
        try
        {
            var user = await _dataContext.Users.FirstAsync(x => ((x.UserName == data.UserName) || (x.Email == data.UserName)) && x.Role != (int)RoleEnum.suadmin && x.Status != (int)StatusEnum.delete);

            if (user?.PasswordSalt != null)
            {
                if (!UserPasswordIsValid(user.PasswordSalt, user.PasswordHash, data.Password)) return null;

                _ = _mailService.SendEmailAsync(new EmailRequestDto
                {
                    ToEmail = user?.Email ?? "",
                    ToName = user?.FirstName ?? "",
                    SubTitle = "Login Attempts",
                    ReplyToEmail = "",
                    Subject = "Login Attempts",
                    Body = _mailService.UserLoginBody(user),
                    Attachments = { }
                });

                // string? ip = GetUserIpAddressDetails();

                return user;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError("Authentication ${message}", ex.Message);
            return null;
        }
    }

    public async Task<BooleanReturnDto?> DeleteUserAccount(DeleteProfile data, int userId)
    {
        try
        {
            AppUser? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            //check if provided password is valid
            if (!UserPasswordIsValid(user.PasswordSalt, user.PasswordHash, data.Password))
            {
                return new BooleanReturnDto
                {
                    Status = false,
                    Message = "Invalid current password",
                };
            }

            user.Status = (int)StatusEnum.delete;
            user.Email = AppConstants.Deletedkeyword + user.Email;
            user.UserName = AppConstants.Deletedkeyword + user.UserName;
            user.FirstName = "";
            user.LastName = "";

            await _dataContext.SaveChangesAsync();

            //send confirmation email to notifiy the user about the deleting request
            _ = _mailService.SendEmailAsync(new EmailRequestDto
            {
                ToEmail = user?.Email ?? "",
                ToName = user?.FirstName ?? "",
                SubTitle = "Account Deleted",
                ReplyToEmail = "",
                Subject = "Account Delition Confirmation",
                Body = _mailService.DeleteAccountBody(user),
                Attachments = { }
            });

            return new BooleanReturnDto
            {
                Status = true,
                Message = "Your account has been deleted",
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Delete user account ${message}", ex.Message);
            return null;
        }
    }

    public async Task<ResultForgetPasswordDto?> ForgetPassword(AppUser user)
    {
        try
        {
            // create new otp code token for user verification
            var otpData = await CreateAuthToken(new CreateAuthTokenDto
            {
                Email = user.Email,
                UserId = user.Id,
                UsageType = (int)TokenUsageTypeEnum.forgetPassword,
            });

            if (otpData == null) return null;

            // send the otp code token trough email
            await _mailService.SendEmailAsync(new EmailRequestDto
            {
                ToEmail = user?.Email ?? "",
                ToName = user?.FirstName ?? "",
                SubTitle = "Forget Password",
                ReplyToEmail = "",
                Subject = "Forget Password Request",
                Body = _mailService.UserForgetPasswordBody(new ForgetPasswordEmailDto
                {
                    UserName = user?.UserName ?? "",
                    Otp = otpData.Otp,
                    Link = ""
                }),
                Attachments = { }
            });

            return new ResultForgetPasswordDto
            {
                OtpToken = otpData?.Token,
                AccessToken = _tokenService.CreateToken(user?.Id ?? 0, (int)RoleEnum.user, true),
                Message = "An email containing an otp code has been sent to you"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during forgot password ${message}", ex.Message);
            return null;
        }
    }

    public async Task<BooleanReturnDto?> ChangeForgetPassword(AppUser user, ChangePasswordDto data)
    {
        try
        {
            PassWordGeneratedDto password = GeneratePassword(data.Password);

            user.PasswordHash = password.PasswordHash;
            user.PasswordSalt = password.PasswordSalt;
            await _dataContext.SaveChangesAsync();

            // send change password confirmation email to the user
            _ = _mailService.SendEmailAsync(new EmailRequestDto
            {
                ToEmail = user?.Email ?? "",
                ToName = user?.FirstName ?? "",
                SubTitle = "Password Change",
                ReplyToEmail = "",
                Subject = "Password Change Confirmation",
                Body = _mailService.ChangePasswordBody(user),
                Attachments = { }
            });

            return new BooleanReturnDto
            {
                Status = true,
                Message = "Password has been changed successfully",
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during forgot password ${message}", ex.Message);
            return null;
        }
    }

    public async Task<ResultUserDto?> UpdateProfileImage(AppUser user, [FromForm] IFormFile image, string folder)
    {
        try
        {
            string? fileUrl = await _appFileService.UploadFile(image, user.Id.ToString(), "gallery");

            if (fileUrl is not null)
            {
                user.Photo = fileUrl;

                await _dataContext.SaveChangesAsync();
                return _mapper.Map<ResultUserDto>(user);
            }

            return null;
        }

        catch (Exception ex)
        {
            _logger.LogError("Error during forgot password ${message}", ex.Message);
            return null;
        }
    }

    public string? GetUserIpAddress(HttpContext httpContext)
    {
        var userIpAddress = httpContext.Connection.RemoteIpAddress;
        return userIpAddress?.ToString();
    }
}
