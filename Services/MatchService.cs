using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;

namespace API.Services;

public class MatchService : IMatchService
{

    private readonly DataContext _dataContext;
    private readonly ILogger<MatchService> _logger;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly IUserService _userService;


    public MatchService(
        DataContext context,
        ILogger<MatchService> logger,
        IMapper mapper,
        IMemoryCache memoryCache,
        IUserService userService)
    {
        _dataContext = context;
        _logger = logger;
        _mapper = mapper;
        _memoryCache = memoryCache;
        _userService = userService;
    }

    public async Task<BooleanReturnDto> CheckIfUserIsMatch(int user, int matchUser)
    {
        try
        {
            var query = _dataContext.Matches.Where(x => x.State == (int)MatchStateEnum.approved && x.Status == (int)StatusEnum.enable && (x.UserId.Equals(user) && x.MatchedUserId.Equals(matchUser) || x.MatchedUserId.Equals(user) && x.UserId.Equals(matchUser)));

            var _result = await query.FirstOrDefaultAsync();

            if (_result == null) return new BooleanReturnDto
            {
                Status = false
            };

            return new BooleanReturnDto
            {
                Status = true,
                Data = _result
            };
        }
        catch (Exception)
        {
            return new BooleanReturnDto
            {
                Status = false
            };
        }
    }

    public async Task<BooleanReturnDto> CheckIfMatchRequestExist(int user, int matchUser)
    {
        try
        {
            var query = _dataContext.Matches.Where(x => x.Status == (int)StatusEnum.enable && (x.UserId.Equals(user) && x.MatchedUserId.Equals(matchUser) || x.MatchedUserId.Equals(user) && x.UserId.Equals(matchUser)));

            var _result = await query.CountAsync();

            if (_result == 0) return new BooleanReturnDto
            {
                Status = false
            };

            return new BooleanReturnDto
            {
                Status = true,
                // Data = _result
            };
        }
        catch (Exception)
        {
            return new BooleanReturnDto
            {
                Status = false
            };
        }
    }

    public async Task<BooleanReturnDto> CheckIfUserSendMatchRequest(int user, int matchUser, int type = (int)MatchStateEnum.inititated)
    {
        try
        {
            var query = _dataContext.Matches.Where(x => x.Status == (int)StatusEnum.enable);

            var _result = await query.FirstOrDefaultAsync();

            if (_result == null) return new BooleanReturnDto
            {
                Status = false
            };

            if (_result.State != type)
            {
                return new BooleanReturnDto
                {
                    Status = false
                };
            }

            return new BooleanReturnDto
            {
                Status = true,
                Data = _result
            };
        }
        catch (Exception)
        {
            return new BooleanReturnDto
            {
                Status = false
            };
        }
    }

    public async Task<BooleanReturnDto> CheckIfUserReceivedMatchRequest(int user, int matchUser, int type = (int)MatchStateEnum.inititated)
    {
        try
        {
            var query = _dataContext.Matches.Where(x => x.Status == (int)StatusEnum.enable && x.MatchedUserId.Equals(user) && x.UserId.Equals(matchUser));

            var _result = await query.FirstOrDefaultAsync();

            if (_result == null) return new BooleanReturnDto
            {
                Status = false
            };

            if (_result.State != type)
            {
                return new BooleanReturnDto
                {
                    Status = false
                };
            }

            return new BooleanReturnDto
            {
                Status = true,
                Data = _result
            };
        }
        catch (Exception)
        {
            return new BooleanReturnDto
            {
                Status = false
            }; ;
        }
    }

    public async Task<AppMatch?> GetUserMatchSendRequest(int userId, int? id = null)
    {
        if (id != null)
        {
            AppMatch? match = await _dataContext.Matches.FirstOrDefaultAsync(m => m.UserId == userId && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated && m.Id == id);
            return match;
        }
        else
        {
            AppMatch? match = await _dataContext.Matches.FirstOrDefaultAsync(m => m.UserId == userId && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated);
            return match;
        }

    }

    public async Task<AppMatch?> GetUserMatchReceivedRequest(int userId, int? id = null)
    {
        if (id != null)
        {
            AppMatch? match = await _dataContext.Matches.FirstOrDefaultAsync(m => m.MatchedUserId == userId && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated && m.Id == id);
            return match;
        }
        else
        {
            AppMatch? match = await _dataContext.Matches.FirstOrDefaultAsync(m => m.MatchedUserId == userId && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated);
            return match;
        }
    }

    public async Task<BooleanReturnDto> DeleteUserMatchRequest(int userId, int id)
    {
        try
        {
            AppMatch? match = await _dataContext.Matches.FirstOrDefaultAsync(m => m.UserId == userId && m.Id == id && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated);
            if (match == null)
            {
                return new BooleanReturnDto
                {
                    Status = false,
                    Message = "User match request not found"
                };
            }

            match.Status = (int)StatusEnum.delete;
            await _dataContext.SaveChangesAsync();

            return new BooleanReturnDto
            {
                Status = true,
                Message = "The provided request has been deleted"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("error when deleting user match", ex.Message);
            return new BooleanReturnDto
            {
                Status = false,
                Message = "Error when deleting user match"
            };
        }
    }

    public async Task<BooleanReturnDto> ReplyMatchRequest(int userId, string action, int id)
    {
        try
        {
            var match = await GetUserMatchReceivedRequest(userId, id);
            if (match == null)
            {
                return new BooleanReturnDto
                {
                    Status = false,
                    Message = "User match request not found"
                };
            }

            match.State = (action == "approved") ? (int)MatchStateEnum.approved : (int)MatchStateEnum.rejected;
            await _dataContext.SaveChangesAsync();

            return new BooleanReturnDto
            {
                Status = true,
                Message = "The provided match request has been " + action
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("error when replying to match request", ex.Message);
            return new BooleanReturnDto
            {
                Status = false,
                Message = "error when replying to match request"
            };
        }
    }

    public async Task<BooleanReturnDto> CancelMatchRequest(int userId, int id)
    {
        try
        {
            var match = await GetUserMatchSendRequest(userId, id);

            if (match == null) return new BooleanReturnDto
            {
                Status = false,
                Message = "User match request not found"
            };

            match.Status = (int)StatusEnum.delete;
            await _dataContext.SaveChangesAsync();

            return new BooleanReturnDto
            {
                Status = true,
                Message = "The provided request has been cancelled"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("error when cancelling user match", ex.Message);
            return new BooleanReturnDto
            {
                Status = false,
                Message = "Error when cancelling user match"
            };
        }
    }

    public async Task<MatchesResultDto?> CreateUserMatch(int userId, AddMatchDto data)
    {
        try
        {
            BooleanReturnDto receivedRequest = await CheckIfUserReceivedMatchRequest(userId, data.MatchedUserId, (int)MatchStateEnum.inititated);

            if (receivedRequest.Status)
            {
                if (receivedRequest?.Data != null)
                {
                    AppMatch currentRequest = receivedRequest.Data;
                    currentRequest.State = (int)MatchStateEnum.approved;
                    await _dataContext.SaveChangesAsync();

                    var match = _mapper.Map<MatchesResultDto>(currentRequest);
                    return match;
                }
            }

            var newMatch = new AppMatch
            {
                MatchedUserId = data.MatchedUserId,
                UserId = userId,
            };

            _dataContext.Add(newMatch);
            await _dataContext.SaveChangesAsync();

            return _mapper.Map<MatchesResultDto>(newMatch);
        }
        catch (Exception ex)
        {
            _logger.LogError("error when cancelling user match", ex.Message);
            return null;
        }
    }

    public async Task<ResultPaginate<MatchesResultDto>?> GetUserMatches(int userId, int skip = 0, int limit = 50, string sort = "desc")
    {
        try
        {
            //check if matches data are cached
            string cacheKey = "matches_" + userId + "_" + sort + "_" + skip + "_" + limit;
            ///
            dynamic? cachedMatches = _memoryCache.Get(cacheKey);

            if (cachedMatches != null)
            {
                return cachedMatches;
            }

            //////
            var query = _dataContext.Matches.Where(m => m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.approved && (m.UserId == userId || m.MatchedUserId == userId));

            // Apply sorting directly in the query
            query = sort == "desc"
                ? query.OrderByDescending(x => x.CreatedAt)
                : query.OrderBy(x => x.CreatedAt);

            var totalMatches = await query.CountAsync();

            var matches = await query.Skip(skip).Take(limit).ToListAsync(); //.Include(p => p.Users)

            foreach (var match in matches)
            {
                match.User = await _userService.GetUserById(match.UserId);
                match.MatchedUser = await _userService.GetUserById(match.MatchedUserId);
            }

            var result = _mapper.Map<IEnumerable<MatchesResultDto>>(matches);

            ResultPaginate<MatchesResultDto> response = new()
            {
                Data = result,
                Limit = limit,
                Skip = skip,
                Total = totalMatches
            };

            _memoryCache.Set(cacheKey, response, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });  //add the matches data in cached
            _memoryCache.Set(cacheKey + "_entities", result, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("error when getting all users matches ${message}", ex.Message);
            return null;
        }
    }
}
