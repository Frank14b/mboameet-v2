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

                if ((await _matchService.CheckIfMatchRequestExist(userId, data.MatchedUserId)).Status) return BadRequest("User match request already exists");
                if ((await _matchService.CheckIfUserSendMatchRequest(userId, data.MatchedUserId, (int)MatchStateEnum.inititated)).Status)
                {
                    return BadRequest("A previous request was already sent");
                }

                BooleanReturnDto receivedRequest = await _matchService.CheckIfUserReceivedMatchRequest(userId, data.MatchedUserId, (int)MatchStateEnum.inititated);

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

                var newMatch = new AppMatch
                {
                    MatchedUserId = ObjectId.Parse(data.MatchedUserId),
                    UserId = ObjectId.Parse(userId),
                };

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
        public async Task<ActionResult<IEnumerable<MatchesPaginateResultDto>>> GetUserMatches(int skip = 0, int limit = 50, string sort = "desc")
        {
            try
            {
                string userId = _userService.GetConnectedUser(User);

                var query = _dataContext.Matches.Where(m => m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.approved && (m.UserId.ToString() == userId || m.MatchedUserId.ToString() == userId));

                IQueryable<AppMatch> orderedQuery;
                if (sort == "desc")
                {
                    orderedQuery = query.OrderByDescending(x => x.CreatedAt);
                }
                else
                {
                    orderedQuery = query.OrderBy(x => x.CreatedAt);
                }

                var _result = await orderedQuery.Skip(skip).Take(limit).Include(p => p.MatchedUser).ToListAsync(); //.Include(p => p.Users)
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<BooleanReturnDto>> CancelMatchRequest()
        {
            try
            {
                string userId = _userService.GetConnectedUser(User);

                var match = await _matchService.GetUserMatchSendRequest(userId);

                if (match == null) return NotFound("Request not found");

                match.Status = (int)StatusEnum.delete;
                await _dataContext.SaveChangesAsync();

                return new BooleanReturnDto
                {
                    Status = true,
                    Message = "The provided request has been cancelled"
                };
            }
            catch (Exception e)
            {
                return BadRequest("An error occurred or request found " + e);
            }
        }

        [HttpPatch("{id}/review")]
        public async Task<ActionResult<BooleanReturnDto>> ReplyMatchRequest(string action = "approved")
        {
            try
            {
                if (action != "approved" && action != "declined") return BadRequest("Invalid action : approved or declined");

                string userId = _userService.GetConnectedUser(User);

                var match = await _matchService.GetUserMatchReceivedRequest(userId);

                if (match == null) return NotFound("Request not found");

                match.State = (action == "approved") ? (int)MatchStateEnum.approved : (int)MatchStateEnum.rejected;
                await _dataContext.SaveChangesAsync();

                return new BooleanReturnDto
                {
                    Status = true,
                    Message = "The provided request has been " + action
                };
            }
            catch (Exception e)
            {
                return BadRequest("An error occurred or request found " + e);
            }
        }
    }
}