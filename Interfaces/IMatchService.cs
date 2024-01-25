using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IMatchService
    {
        Task<BooleanReturnDto> CheckIfUserIsMatch(string user, string matchUser);
        Task<BooleanReturnDto> CheckIfUserReceivedMatchRequest(string user, string matchUser, int type);
        Task<BooleanReturnDto> CheckIfUserSendMatchRequest(string user, string matchUser, int type);
        Task<BooleanReturnDto> CheckIfMatchRequestExist(string user, string matchUser);
        Task<AppMatch?> GetUserMatchSendRequest(string userId, string? id);
        Task<AppMatch?> GetUserMatchReceivedRequest(string userId, string? id);
        Task<BooleanReturnDto> DeleteUserMatchRequest(string userId, string id);
        Task<BooleanReturnDto> ReplyMatchRequest(string userId, string action, string id);
        Task<BooleanReturnDto> CancelMatchRequest(string userId, string id);
        Task<MatchesResultDto?> CreateUserMatch(string userId, AddMatchDto data);
        Task<ResultPaginate<MatchesResultDto>?> GetUserMatches(string userId, int skip = 0, int limit = 50, string sort = "desc");
    }
}