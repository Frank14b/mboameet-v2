using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admin
{
    [Route("/api/v1/admin")]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UsersController(DataContext dataContext, IUserService userService, IMapper mapper)
        {
            _dataContext = dataContext;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ActionResult<ResultAllUserDto>> CreateUserAccount(CreateUserDto data)
        {
            try
            {
                if (await _userService.IsUserAlreadyExist(data)) return BadRequest("User already exists");

                using var hmac = new HMACSHA512();

                var newUser = _mapper.Map<AppUser>(data);
                newUser.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(AppHelper.GenerateRandomString(50)));
                newUser.PasswordSalt = hmac.Key;
                newUser.Status = (int)StatusEnum.enable;
                newUser.Age = 18;
                newUser.Role = (int)RoleEnum.user;

                _dataContext.Users.Add(newUser);
                await _dataContext.SaveChangesAsync();

                var result = _mapper.Map<ResultAllUserDto>(newUser);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest("Couldn't create the user account " + e);
            }
        }
    }
}