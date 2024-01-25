using System.Net.Mime;
using Internal;
using System;
using System.Text.RegularExpressions;
using System.Reflection.Metadata;
using System.Data.Common;
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
using Microsoft.Extensions.Caching.Memory;


namespace API.Controllers;

[Authorize(Policy = "IsUser")]
[Route("/api/v1/matches")]
public class MatchController : BaseApiController
{
    private readonly DataContext _dataContext;
    // private readonly EmailsCommon _emailsCommon;
    private readonly IMapper _mapper;
    private readonly IMatchService _matchService;
    private readonly IUserService _userService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger _logger;

    public MatchController(
        DataContext context,
        IMapper mapper,
        IConfiguration configuration,
        IMailService mailService,
        IMatchService matchService,
        IUserService userService,
        IMemoryCache memoryCache,
        ILogger logger)
    {
        _dataContext = context;
        _mapper = mapper;
        _matchService = matchService;
        _userService = userService;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    [HttpPost("")]
    public async Task<ActionResult<MatchesResultDto>> AddMatch(AddMatchDto data)
    {
        try
        {
            string userId = _userService.GetConnectedUser(User);

            if (data.MatchedUserId == userId) return BadRequest("Invalid user id");

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
    public async Task<ActionResult<IEnumerable<ResultPaginate<MatchesResultDto>>>> GetUserMatches(int skip = 0, int limit = 50, string sort = "desc")
    {
        try
        {
            string userId = _userService.GetConnectedUser(User);

            //check if matches data are cached
            string cacheKey = "matches_" + userId + "_" + sort + "_" + skip + "_" + limit;
            var cachedMatches = _memoryCache.Get(cacheKey);

            if (cachedMatches != null)
            {
                return Ok(cachedMatches);
            }

            var query = _dataContext.Matches.Where(m => m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.approved && (m.UserId.ToString() == userId || m.MatchedUserId.ToString() == userId));

            // Apply sorting directly in the query
            query = sort == "desc"
                ? query.OrderByDescending(x => x.CreatedAt)
                : query.OrderBy(x => x.CreatedAt);

            var totalMatches = await query.CountAsync();

            var matches = await query.Skip(skip).Take(limit).ToListAsync(); //.Include(p => p.Users)

            foreach (var match in matches)
            {
                match.User = await _dataContext.Users.FirstAsync(user => user.Id.Equals(match.UserId));
                match.MatchedUser = await _dataContext.Users.FirstAsync(user => user.Id.Equals(match.MatchedUserId));
            }

            var result = _mapper.Map<IEnumerable<MatchesResultDto>>(matches);

            var response = new ResultPaginate<MatchesResultDto>
            {
                Data = result,
                Limit = limit,
                Skip = skip,
                Total = totalMatches
            };

            _memoryCache.Set(cacheKey, response, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });  //add the matches data in cached
            _memoryCache.Set(cacheKey + "_entities", result, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest("An error occurred or no matches found " + e);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BooleanReturnDto>> CancelMatchRequest(string id)
    {
        try
        {
            string userId = _userService.GetConnectedUser(User);

            BooleanReturnDto result = await _matchService.CancelMatchRequest(userId, id);
            return result;
        }
        catch (Exception e)
        {
            return BadRequest("An error occurred or request found " + e);
        }
    }

    [HttpPatch("{id}/review")]
    public async Task<ActionResult<BooleanReturnDto>> ReplyMatchRequest(string id, string action = "approved")
    {
        try
        {
            string userId = _userService.GetConnectedUser(User);

            if (action != "approved" && action != "declined") return BadRequest("Invalid action : approved or declined");

            BooleanReturnDto result = await _matchService.ReplyMatchRequest(userId, action, id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while reviewing", ex.Message);
            return BadRequest("An error occurred or request found");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BooleanReturnDto>> DeleteMyRequest(string id)
    {
        try
        {
            string userId = _userService.GetConnectedUser(User);

            BooleanReturnDto match = await _matchService.DeleteUserMatchRequest(userId, id);
            return match;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting", ex.Message);
            return BadRequest("An error occurred while deleting");
        }
    }
}
