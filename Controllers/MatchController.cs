using API.Commons;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Policy = "IsUser")]
    [Route("/api/v1/matches")]
    public class MatchController : BaseApiController
    {
        private readonly DataContext _dataContext;
        // private readonly EmailsCommon _emailsCommon;
        private readonly IMapper _mapper;
        private readonly IMatchService _matchService;
        private readonly IUserService _userService;

        public MatchController(
            DataContext context,
            IMapper mapper,
            IConfiguration configuration,
            IMailService mailService,
            IMatchService matchService,
            IUserService userService)
        {
            _dataContext = context;
            _mapper = mapper;
            _matchService = matchService;
            _userService = userService;
        }

        [HttpPost("")]
        public async Task<ActionResult<MatchesResultDto>> AddMatch(AddMatchDto data)
        {
            try
            {
                if (await _matchService.CheckIfUserReceivedMatchRequest(_userService.GetConnectedUser(User), data.MatchedUser, (int)MatchStateEnum.rejected))
                {
                    return BadRequest("A previous request was rejected");
                }

                if (await _matchService.CheckIfUserReceivedMatchRequest(_userService.GetConnectedUser(User), data.MatchedUser, (int)MatchStateEnum.approved))
                {
                    return Ok("A previous request was approved");
                }

                if (await _matchService.CheckIfUserReceivedMatchRequest(_userService.GetConnectedUser(User), data.MatchedUser, (int)MatchStateEnum.inititated))
                {
                    
                }
            }
            catch (Exception e)
            {
                return BadRequest("An error occurred when trying to add " + e);
            }
        }
    }
}