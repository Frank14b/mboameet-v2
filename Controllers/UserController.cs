using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.Data;
using API.DTOs;
using API.Commons;
using API.Entities;
using AutoMapper;
using System.Reactive.Linq;

namespace API.Controllers;

[Authorize(Policy = "IsUser")]
[Route("/api/v1/users")]
public class UsersController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly EmailsCommon _emailsCommon;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    public readonly ILogger _logger;

    public UsersController(
        DataContext context,
        ITokenService tokenService,
        IMapper mapper,
        IConfiguration configuration,
        IMailService mailService,
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
        _configuration = configuration;
        _emailsCommon = new EmailsCommon(mailService, logger);
        _userService = userService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("")]
    public async Task<ActionResult<ResultloginDto>> RegisterUsers(RegisterDto data)
    {
        if (await _userService.UserNameExist(data.Username, null)) return BadRequest("Username already in used");

        if (await _userService.UserEmailExist(data?.Email ?? "", null)) return BadRequest("Email Address already in used");

        if (!_userService.IsValidPassword(data?.Password ?? "")) return BadRequest("Password should have at least 1 lowercase letter, 1 uppercase letter, 1 digit, 1 special character, and at least 8 characters long");

        PassWordGeneratedDto password = _userService.GeneratePassword(data?.Password ?? "");

        var user = new AppUser
        {
            UserName = data?.Username ?? "",
            PasswordHash = password.PasswordHash,
            PasswordSalt = password.PasswordSalt,
            FirstName = data?.Firstname,
            LastName = data?.Lastname,
            Email = data?.Email,
            Age = 18,
            Role = (int)RoleEnum.user,
            Status = (int)StatusEnum.enable,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        var finalresult = _mapper.Map<ResultloginDto>(user);

        finalresult.Token = _tokenService.CreateToken(user.Id.ToString(), user.Role, true);

        var data_email = new EmailRequestDto
        {
            ToEmail = user?.Email ?? "",
            ToName = user?.FirstName ?? "",
            SubTitle = "User Registration",
            ReplyToEmail = "",
            Subject = "User Registration",
            Body = _emailsCommon.UserRegisterBody(user),
            Attachments = { }
        };
        await _emailsCommon.SendMail(data_email);

        return Ok(finalresult);
    }

    [AllowAnonymous]
    [HttpPost("auth")]
    public async Task<ActionResult<ResultloginDto>> LoginUsers(LoginDto data)
    {
        try
        {
            var result = await _context.Users.SingleOrDefaultAsync(x => ((x.UserName == data.Login) || (x.Email == data.Login)) && x.Role != (int)RoleEnum.suadmin && x.Status != (int)StatusEnum.delete);

            if (result == null) Unauthorized("Invalid Login / Password, User not found");

            if (result?.PasswordSalt != null)
            {
                if (!_userService.UserPasswordIsValid(result.PasswordSalt, result.PasswordHash, data.Password)) return Unauthorized("Invalid Login / Password, User not found");

                var finalresult = _mapper.Map<ResultloginDto>(result);

                if (result.Status == (int)StatusEnum.disable) return Ok("Your account is disabled. Please Contact the admin");

                _ = _emailsCommon.SendMail(new EmailRequestDto
                {
                    ToEmail = result?.Email ?? "",
                    ToName = result?.FirstName ?? "",
                    SubTitle = "Login Attempts",
                    ReplyToEmail = "",
                    Subject = "Login Attempts",
                    Body = _emailsCommon.UserLoginBody(finalresult),
                    Attachments = { }
                });

                finalresult.Token = _tokenService.CreateToken(result?.Id.ToString() ?? "", result?.Role ?? 0, true);

                return Ok(finalresult);
            }
            else
            {
                return BadRequest("An error occured or user not found");
            }
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured during login", e.Message);
            return BadRequest("An error occured or user not found");
        }
    }

    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<ResultPaginate<ResultUserDto>>>> GetUsers(int skip = 0, int limit = 50, string sort = "desc")
    {
        try
        {
            var query = _context.Users
                .Where(x => x.Role != (int)RoleEnum.suadmin && x.Status != (int)StatusEnum.delete);

            // Apply sorting directly in the query
            query = sort == "desc"
                ? query.OrderByDescending(x => x.CreatedAt)
                : query.OrderBy(x => x.CreatedAt);

            var users = await query
                // .Include(u => u.Match.OrderByDescending(m => m.CreatedAt).Take(10))
                .Skip(skip)
                .Take(limit)
                .ToListAsync();

            foreach (var user in users)
            {
                user.Match = await _context.Matches
                    .Where(m => m.UserId == user.Id)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToListAsync();
            }

            // Map to DTO after fetching with included matches
            var result = _mapper.Map<IEnumerable<ResultUserDto>>(users);

            // Count before applying pagination for accuracy
            var totalCount = await query.CountAsync();
            //
            return Ok(new ResultPaginate<ResultUserDto>
            {
                Data = result,
                Limit = limit,
                Skip = skip,
                Total = totalCount
            });
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured while getting users", e.Message);
            return BadRequest("An error occurred or user not found"); // Log exception details separately
        }
    }

    [HttpPost("validate-token")]
    public async Task<ActionResult<ResultUserDto>> ValidateToken()
    {
        string id = _userService.GetConnectedUser(User);

        if (!await _userService.UserIdExist(id)) return BadRequest("User not found");

        var user = await _context.Users.Where(x => x.Id.ToString() == id).FirstAsync();

        var result = _mapper.Map<ResultUserDto>(user);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("forget-password")]
    public async Task<ActionResult<ResultForgetPasswordDto>> FogetPassword(ForgetPasswordDto data)
    {
        try
        {
            AppUser? user = await _userService.GetUserByEmail(data.Email);

            if (user == null) return BadRequest("The provided email is not found");

            // create new otp code token for user verification
            var otpData = await _userService.CreateAuthToken(new CreateAuthTokenDto
            {
                Email = user.Email,
            });

            if (otpData == null) return BadRequest("An error occured please retry later");

            //send the otp code token trough email
            await _emailsCommon.SendMail(new EmailRequestDto
            {
                ToEmail = user?.Email ?? "",
                ToName = user?.FirstName ?? "",
                SubTitle = "Forget Password",
                ReplyToEmail = "",
                Subject = "Forget Password Request",
                Body = _emailsCommon.UserForgetPasswordBody(new ForgetPasswordEmailDto
                {
                    UserName = user?.UserName ?? "",
                    Otp = otpData.Otp,
                    Link = ""
                }),
                Attachments = { }
            });

            return Ok(new ResultForgetPasswordDto
            {
                OtpToken = otpData?.Token,
                AccessToken = _tokenService.CreateToken(user?.Id.ToString() ?? "", (int)RoleEnum.user, false),
                Message = "An email containing an otp code has been sent to you"
            });
        }
        catch (Exception e)
        {
            return BadRequest("An error occured " + e);
        }
    }

    [HttpPost("verify-token")]
    public async Task<ActionResult<BooleanReturnDto>> VerifyAuthToken(VerifyAuthTokenDto data)
    {
        try
        {
            var existingToken = await _userService.AuthTokenIsValid(data.Otp, data.Token, data.Type);
            if (existingToken == null) return NotFound("Invalid token or otp code");

            // validate and update the token usage/status for reusability purposes
            if (existingToken.UsageType == (int)TokenUsageTypeEnum.forgetPassword)
            {
                string token = Guid.NewGuid().ToString(); //generate a new token and replace the current

                existingToken.UsageType = (int)TokenUsageTypeEnum.resetPassword;
                existingToken.Token = token;
                existingToken.ExpireAt = DateTime.UtcNow.AddMinutes(AppConstants.TokenValidity);
            }
            else
            {
                existingToken.Status = (int)StatusEnum.disable;
            }

            await _context.SaveChangesAsync(); // save new changes

            return Ok(new BooleanReturnDto
            {
                Status = true,
                Message = "Otp code validated successfully",
            });
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured", e.Message);
            return BadRequest("An error occured or invalid token ");
        }
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<BooleanReturnDto>> ChangePassword(ChangePasswordDto data)
    {
        try
        {
            var existingToken = await _userService.AuthTokenIsValid(null, data.Token, (int)TokenUsageTypeEnum.resetPassword);
            if (existingToken == null) return NotFound("Invalid token");

            existingToken.Status = (int)StatusEnum.disable;
            await _context.SaveChangesAsync();

            AppUser? user = await _userService.GetUserByEmail(existingToken.Email);

            if (user == null) return BadRequest("An error occured or invalid token"); // check if user exist or not

            PassWordGeneratedDto password = _userService.GeneratePassword(data.Password);

            user.PasswordHash = password.PasswordHash;
            user.PasswordSalt = password.PasswordSalt;
            await _context.SaveChangesAsync();


            _ = _emailsCommon.SendMail(new EmailRequestDto
            {
                ToEmail = user?.Email ?? "",
                ToName = user?.FirstName ?? "",
                SubTitle = "Password Change",
                ReplyToEmail = "",
                Subject = "Password Change Confirmation",
                Body = _emailsCommon.ChangePasswordBody(user),
                Attachments = { }
            });

            return Ok(new BooleanReturnDto
            {
                Status = true,
                Message = "Password has been changed successfully",
            });
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured", e.Message);
            return BadRequest("An error occured ");
        }
    }

    [HttpPut("")]
    public async Task<ActionResult<ResultUserDto>> Update(UpdateProfile data)
    {
        try
        {
            string userId = _userService.GetConnectedUser(User);

            AppUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null) return BadRequest("An error occured");

            if (data?.UserName != null)
            {
                if (await _userService.UserNameExist(data.UserName, userId)) return BadRequest("UserName already exists");

                user.UserName = data.UserName;
            }

            if (data?.Email != null)
            {
                if (data?.Password == null) return BadRequest("Password is required to update the profile email"); // return error if trying to update email without providing current password

                if (!_userService.UserPasswordIsValid(user.PasswordSalt, user.PasswordHash, data.Password)) return Unauthorized("Invalid Password");

                if (await _userService.UserEmailExist(data.Email, userId)) return BadRequest("Email already exists"); // return error if email already exist

                user.Email = data.Email;
            }

            user.FirstName = data?.FirstName ?? user.FirstName;
            user.LastName = data?.LastName ?? user.LastName;
            user.Age = data?.Age ?? user.Age;

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ResultUserDto>(user));
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured during profile update", e.Message);
            return BadRequest("An error occured ");
        }
    }

    [HttpDelete("")]
    public async Task<ActionResult<BooleanReturnDto>> DeleteAccount(DeleteProfile data)
    {
        try
        {
            string userId = _userService.GetConnectedUser(User);

            AppUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null) return BadRequest("An error occured");

            if (!_userService.UserPasswordIsValid(user.PasswordSalt, user.PasswordHash, data.Password)) return Unauthorized("Invalid Password");

            user.Status = (int)StatusEnum.delete;
            user.Email = AppConstants.Deletedkeyword + user.Email;
            user.UserName = AppConstants.Deletedkeyword + user.UserName;
            user.FirstName = "";
            user.LastName = "";

            await _context.SaveChangesAsync();

            _ = _emailsCommon.SendMail(new EmailRequestDto
            {
                ToEmail = user?.Email ?? "",
                ToName = user?.FirstName ?? "",
                SubTitle = "Account Deleted",
                ReplyToEmail = "",
                Subject = "Account Delition Confirmation",
                Body = _emailsCommon.ChangePasswordBody(user),
                Attachments = { }
            });

            return Ok(new BooleanReturnDto
            {
                Status = true,
                Message = "User account has been deleted"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured while deleting account", e.Message);
            return BadRequest("An error occured ");
        }
    }
}