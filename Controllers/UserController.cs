using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.Data;
using API.DTOs;
using API.Entities;
using AutoMapper;
using System.Reactive.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace API.Controllers;

[Authorize(Policy = "IsUser")]
[Route("/api/v1/users")]
public class UsersController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    public readonly ILogger _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IAppFileService _appFileService;

    public UsersController(
        DataContext context,
        ITokenService tokenService,
        IMapper mapper,
        IConfiguration configuration,
        IUserService userService,
        ILogger<UsersController> logger,
        IMemoryCache memoryCache,
        IAppFileService appFileService)
    {
        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
        _configuration = configuration;
        _userService = userService;
        _logger = logger;
        _memoryCache = memoryCache;
        _appFileService = appFileService;
    }

    [AllowAnonymous]
    [HttpPost("")]
    public async Task<ActionResult<ResultloginDto>> RegisterUsers(RegisterDto data)
    {
        if (await _userService.UserNameExist(data.UserName, null)) return BadRequest("Username already in used");

        if (await _userService.UserEmailExist(data?.Email ?? "", null)) return BadRequest("Email Address already in used");

        if (!_userService.IsValidPassword(data?.Password ?? "")) return BadRequest("Password should have at least 1 lowercase letter, 1 uppercase letter, 1 digit, 1 special character, and at least 8 characters long");

        AppUser? user = await _userService.CreateUserAccount(data);

        if (user == null) return BadRequest("Couldn't create user account. Please try again later.");

        ResultloginDto result = _mapper.Map<ResultloginDto>(user);

        result.Token = _tokenService.CreateToken(user.Id, user.Role, true);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("auth")]
    public async Task<ActionResult<ResultloginDto>> LoginUsers(LoginDto data)
    {
        if (data is null) return BadRequest("Invalid Username / Password, User not found");

        AppUser? user = await _userService.AuthenticateUser(data);

        // string? ip = _userService.GetUserIpAddress(HttpContext); // get user ip address from http

        if (user is null) return BadRequest("Invalid Username / Password, User not found");

        if (user.Status == (int)StatusEnum.disable) return Ok("Your account is disabled. Please Contact the admin");

        // create user auth token
        var result = _mapper.Map<ResultloginDto>(user);
        result.Token = _tokenService.CreateToken(result.Id, user.Role, true);

        return Ok(result);
    }

    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<ResultPaginate<ResultUserDto>>>> GetUsers(int skip = 0, int limit = 50, string sort = "desc")
    {
        int userId = _userService.GetConnectedUser(User);

        var query = _context.Users
            .Where(x => x.Role != (int)RoleEnum.suadmin && x.Status != (int)StatusEnum.delete && x.Id != userId);

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

    [HttpPost("validate-token")]
    public async Task<ActionResult<ResultUserDto>> ValidateToken()
    {
        int id = _userService.GetConnectedUser(User);

        if (!await _userService.UserIdExist(id)) return BadRequest("User not found");

        //check if the user data is cached
        string cacheKey = "user_" + id;
        var cachedMatches = _memoryCache.Get(cacheKey);

        if (cachedMatches is not null)
        {
            return Ok(cachedMatches);
        }

        var user = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

        var result = _mapper.Map<ResultUserDto>(user);

        _memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) }); //add the user data in cached

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("forget-password")]
    public async Task<ActionResult<ResultForgetPasswordDto>> FogetPassword(ForgetPasswordDto data)
    {
        AppUser? user = await _userService.GetUserByEmail(data.Email);

        if (user is null) return BadRequest("The provided email is not found");

        ResultForgetPasswordDto? result = await _userService.ForgetPassword(user);

        if (result is null) return BadRequest("An error occured please retry later");

        return result;
    }

    [HttpPost("verify-token")]
    public async Task<ActionResult<BooleanReturnDto>> VerifyAuthToken(VerifyAuthTokenDto data)
    {
        var existingToken = await _userService.AuthTokenIsValid(data.Otp, data.Token, data.Type);
        if (existingToken is null) return NotFound("Invalid token or otp code");

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

    [HttpPost("change-password")]
    public async Task<ActionResult<BooleanReturnDto>> ChangePassword(ChangePasswordDto data)
    {
        var existingToken = await _userService.AuthTokenIsValid(null, data.Token, (int)TokenUsageTypeEnum.resetPassword);
        if (existingToken == null) return NotFound("Invalid or expired token");

        existingToken.Status = (int)StatusEnum.disable;
        await _context.SaveChangesAsync();

        AppUser? user = await _userService.GetUserByEmail(existingToken.Email);

        if (user == null) return BadRequest("An error occured or invalid token"); // check if user exist or not

        BooleanReturnDto? result = await _userService.ChangeForgetPassword(user, data);

        if (result == null) return BadRequest("An error occured please try again later");

        return result;
    }

    [HttpPut("")]
    public async Task<ActionResult<ResultUserDto>> Update(UpdateProfile data)
    {
        int userId = _userService.GetConnectedUser(User);

        AppUser? user = await _userService.GetUserById(userId);

        if (user == null) return BadRequest("An error occured");

        if (data.UserName is not null)
        {
            if (await _userService.UserNameExist(data.UserName, userId)) return BadRequest("UserName already exists");

            user.UserName = data.UserName;
        }

        if (data.Email is not null)
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

    [HttpDelete("")]
    public async Task<ActionResult<BooleanReturnDto>> DeleteAccount(DeleteProfile data)
    {
        int userId = _userService.GetConnectedUser(User);

        BooleanReturnDto? result = await _userService.DeleteUserAccount(data, userId);
        if (result == null) return BadRequest("An error occured or user not found");

        return result;
    }

    [HttpPut("picture")]
    public async Task<ActionResult<ResultUserDto>> UpdatePicture([FromForm] IFormFile image)
    {
        int userId = _userService.GetConnectedUser(User);

        AppUser? user = await _userService.GetUserById(userId);

        if (user == null) return BadRequest("An error occured");

        ResultUserDto? result = await _userService.UpdateProfileImage(user, image, "gallery");

        if(result is null) return BadRequest("Couldn't update the profile image. Retry later");

        return Ok(result);
    }
}