using API.Commons;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

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
                string userId = _userService.GetConnectedUser(User);

                if ((await _matchService.CheckIfMatchRequestExist(userId, data.MatchedUser)).Status) return BadRequest("User match request already exists");
                if ((await _matchService.CheckIfUserSendMatchRequest(userId, data.MatchedUser, (int)MatchStateEnum.inititated)).Status)
                {
                    return BadRequest("A previous request was already sent");
                }

                BooleanReturnDto receivedRequest = await _matchService.CheckIfUserReceivedMatchRequest(userId, data.MatchedUser, (int)MatchStateEnum.inititated);

                if (receivedRequest.Status)
                {
                    if (receivedRequest?.Data != null)
                    {
                        AppMatch currentRequest = receivedRequest.Data;
                        currentRequest.State = (int)MatchStateEnum.approved;
                        await _dataContext.SaveChangesAsync();

                        var rs = _mapper.Map<MatchesResultDto>(currentRequest);
                        return Ok(rs);
                    }
                }

                var newMatch = _mapper.Map<AppMatch>(data);
                newMatch.User = ObjectId.Parse(userId);
                newMatch.State = (int)MatchStateEnum.inititated;

                _dataContext.Add(newMatch);
                await _dataContext.SaveChangesAsync();

                var result = _mapper.Map<MatchesResultDto>(newMatch);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest("An error occurred when trying to add " + e);
            }
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<MatchesPaginateResultDto>>> GetUserMatches(int skip = 0, int limit = 0, string sort = "desc")
        {
            try
            {
                string userId = _userService.GetConnectedUser(User);

                var query = _dataContext.Matches.Where(m => m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.approved && (m.User.ToString() == userId || m.MatchedUser.ToString() == userId));

                IQueryable<AppMatch> orderedQuery;
                if (sort == "desc")
                {
                    orderedQuery = query.OrderByDescending(x => x.CreatedAt);
                }
                else
                {
                    orderedQuery = query.OrderBy(x => x.CreatedAt);
                }

                var _result = await orderedQuery.Skip(skip).Take(limit).ToListAsync();
                var result = _mapper.Map<IEnumerable<MatchesResultDto>>(_result);
                var matches = new MatchesPaginateResultDto
                {
                    Data = result,
                    Limit = limit,
                    Skip = skip,
                    Total = query.Count()
                };

                return Ok(matches);
            }
            catch (Exception e)
            {
                return BadRequest("An error occurred or no matches found " + e);
            }
        }
    }
}