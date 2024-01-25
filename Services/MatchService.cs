using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace API.Services;

public class MatchService : IMatchService
{

    private readonly DataContext _dataContext;
    private readonly ILogger _logger;

    public MatchService(DataContext context, ILogger logger)
    {
        _dataContext = context;
        _logger = logger;
    }

    public async Task<BooleanReturnDto> CheckIfUserIsMatch(string user, string matchUser)
    {
        try
        {
            var query = _dataContext.Matches.Where(x => x.State == (int)MatchStateEnum.approved && x.Status == (int)StatusEnum.enable && (x.UserId.Equals(ObjectId.Parse(user)) && x.MatchedUserId.Equals(ObjectId.Parse(matchUser)) || x.MatchedUserId.Equals(ObjectId.Parse(user)) && x.UserId.Equals(ObjectId.Parse(matchUser))));

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

    public async Task<BooleanReturnDto> CheckIfMatchRequestExist(string user, string matchUser)
    {
        try
        {
            var query = _dataContext.Matches.Where(x => x.Status == (int)StatusEnum.enable && (x.UserId.Equals(ObjectId.Parse(user)) && x.MatchedUserId.Equals(ObjectId.Parse(matchUser)) || x.MatchedUserId.Equals(ObjectId.Parse(user)) && x.UserId.Equals(ObjectId.Parse(matchUser))));

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

    public async Task<BooleanReturnDto> CheckIfUserSendMatchRequest(string user, string matchUser, int type = (int)MatchStateEnum.inititated)
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

    public async Task<BooleanReturnDto> CheckIfUserReceivedMatchRequest(string user, string matchUser, int type = (int)MatchStateEnum.inititated)
    {
        try
        {
            var query = _dataContext.Matches.Where(x => x.Status == (int)StatusEnum.enable && x.MatchedUserId.Equals(ObjectId.Parse(user)) && x.UserId.Equals(ObjectId.Parse(matchUser)));

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

    public async Task<AppMatch?> GetUserMatchSendRequest(string userId, string? id = null)
    {
        if (id != null)
        {
            AppMatch? match = await _dataContext.Matches.FirstOrDefaultAsync(m => m.UserId.ToString() == userId && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated && m.Id.ToString() == id);
            return match;
        }
        else
        {
            AppMatch? match = await _dataContext.Matches.FirstOrDefaultAsync(m => m.UserId.ToString() == userId && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated);
            return match;
        }

    }

    public async Task<AppMatch?> GetUserMatchReceivedRequest(string userId, string? id = null)
    {
        if (id != null)
        {
            AppMatch? match = await _dataContext.Matches.FirstOrDefaultAsync(m => m.MatchedUserId.ToString() == userId && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated && m.Id.ToString() == id);
            return match;
        }
        else
        {
            AppMatch? match = await _dataContext.Matches.FirstOrDefaultAsync(m => m.MatchedUserId.ToString() == userId && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated);
            return match;
        }
    }

    public async Task<BooleanReturnDto> DeleteUserMatchRequest(string userId, string id)
    {
        try
        {
            AppMatch? match = await _dataContext.Matches.FirstOrDefaultAsync(m => m.UserId.ToString() == userId && m.Id.ToString() == id && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated);
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

    public async Task<BooleanReturnDto> ReplyMatchRequest(string userId, string action, string id)
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

    public async Task<BooleanReturnDto> CancelMatchRequest(string userId, string id)
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
}
