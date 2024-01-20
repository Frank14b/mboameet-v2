using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.Services;
using API.Data;
using API.DTOs;
using API.Commons;
using API.Entities;
using AutoMapper;

namespace API.Controllers
{
    [Authorize(Policy = "IsUser")]
    [Route("/api/v1/users")]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly UsersCommon _userCommon;
        private readonly EmailsCommon _emailsCommon;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public UsersController(DataContext context, ITokenService tokenService, IMapper mapper, IConfiguration configuration, IMailService mailService, IUserService userService)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
            _userCommon = new UsersCommon(context);
            _configuration = configuration;
            _emailsCommon = new EmailsCommon(mailService);
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<ActionResult<ResultloginDto>> RegisterUsers(RegisterDto data)
        {
            if (await _userCommon.UserNameExist(data.Username)) return BadRequest("Username already in used");

            if (await _userCommon.UserEmailExist(data?.Email ?? "")) return BadRequest("Email Address already in used");

            if (!_userCommon.IsValidPassword(data?.Password ?? "")) return BadRequest("Password should have at least 1 lowercase letter, 1 uppercase letter, 1 digit, 1 special character, and at least 8 characters long");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = data?.Username ?? "",
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data?.Password ?? "")),
                PasswordSalt = hmac.Key,
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

            finalresult.Token = _tokenService.CreateToken(user.Id.ToString(), user.Role);

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
                    using var hmac = new HMACSHA512(result.PasswordSalt);

                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data.Password));

                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != result.PasswordHash[i]) return Unauthorized("Invalid Login / Password, User not found");
                    }

                    var finalresult = _mapper.Map<ResultloginDto>(result);

                    if (result.Status == (int)StatusEnum.disable) return Ok("Your account is disabled. Please Contact the admin");

                    var data_email = new EmailRequestDto
                    {
                        ToEmail = result?.Email ?? "",
                        ToName = result?.FirstName ?? "",
                        SubTitle = "Login Attempts",
                        ReplyToEmail = "",
                        Subject = "Login Attempts",
                        Body = _emailsCommon.UserLoginBody(finalresult),
                        Attachments = { }
                    };
                    await _emailsCommon.SendMail(data_email);

                    finalresult.Token = _tokenService.CreateToken(result?.Id.ToString() ?? "", result?.Role ?? 0);

                    return Ok(finalresult);
                }
                else
                {
                    return BadRequest("An error occured or user not found");
                }

            }
            catch (Exception e)
            {
                return BadRequest("An error occured or user not found " + e);
            }
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<ResultUsersPaginate>>> GetUsers(int skip = 0, int limit = 50, string sort = "desc")
        {
            try
            {
                var query = _context.Users
                    .Where(x => x.Role != (int)RoleEnum.suadmin && x.Status != (int)StatusEnum.delete);

                // Apply sorting directly in the query
                query = sort == "desc"
                    ? query.OrderByDescending(x => x.CreatedAt)
                    : query.OrderBy(x => x.CreatedAt);

                // Include matches in a single query for better performance
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
                var result = _mapper.Map<IEnumerable<ResultAllUserDto>>(users);

                // Count before applying pagination for accuracy
                var totalCount = await query.CountAsync();
                //
                return Ok(new ResultUsersPaginate
                {
                    Data = result,
                    Limit = limit,
                    Skip = skip,
                    Total = totalCount
                });
            }
            catch (Exception e)
            {
                return BadRequest("An error occurred or user not found " + e); // Log exception details separately
            }
        }

        [HttpPost("validate-token")]
        public async Task<ActionResult<ResultAllUserDto>> ValidateToken()
        {
            string id = _userService.GetConnectedUser(User);

            if (!await _userCommon.UserIdExist(id)) return BadRequest("User not found");

            var user = await _context.Users.Where(x => x.Id.ToString() == id).FirstAsync();

            var result = _mapper.Map<ResultAllUserDto>(user);

            return Ok(result);
        }
    }
}