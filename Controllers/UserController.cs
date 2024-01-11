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

        public UsersController(DataContext context, ITokenService tokenService, IMapper mapper, IConfiguration configuration, IMailService mailService)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
            _userCommon = new UsersCommon(context);
            _configuration = configuration;
            _emailsCommon = new EmailsCommon(mailService);
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
                CreatedAt = DateTime.UtcNow,
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

            return finalresult;
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<ActionResult<ResultloginDto>> LoginUsers(LoginDto data)
        {
            try
            {
                var result = await _context.Users.SingleOrDefaultAsync(x => ((x.UserName == data.Login) || (x.Email == data.Login)) && x.Role != (int)RoleEnum.suadmin && x.Status != (int)StatusEnum.delete);

                if (result == null) Unauthorized("Invalid Login / Password, User not found");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                using var hmac = new HMACSHA512(result.PasswordSalt);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

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

                return finalresult;
            }
            catch (Exception e)
            {
                return BadRequest("An error occured or user not found " + e);
            }
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<ResultAllUserDto>>> GetUsers()
        {
            var users = await _context.Users.Where(x => x.Role != (int)RoleEnum.suadmin && x.Status != (int)StatusEnum.delete).ToListAsync();

            var result = _mapper.Map<IEnumerable<ResultAllUserDto>>(users);

            return Ok(result);
        }

        [HttpPost("validate-token")]
        public async Task<ActionResult<ResultAllUserDto>> ValidateToken()
        {
            ClaimsPrincipal currentUser = this.User;
            var id = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (!await _userCommon.UserIdExist(id)) return BadRequest("User not found");

            var user = await _context.Users.Where(x => x.Id.ToString() == id).FirstAsync();

            var result = _mapper.Map<ResultAllUserDto>(user);

            return Ok(result);
        }
    }
}