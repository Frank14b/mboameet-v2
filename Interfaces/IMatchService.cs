using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IMatchService
    {
        Task<BooleanReturnDto> CheckIfUserIsMatch(int user, int matchUser);
        Task<BooleanReturnDto> CheckIfUserReceivedMatchRequest(int user, int matchUser, int type);
        Task<BooleanReturnDto> CheckIfUserSendMatchRequest(int user, int matchUser, int type);
        Task<BooleanReturnDto> CheckIfMatchRequestExist(int user, int matchUser);
        Task<AppMatch?> GetUserMatchSendRequest(int userId, int? id);
        Task<AppMatch?> GetUserMatchReceivedRequest(int userId, int? id);
        Task<BooleanReturnDto> DeleteUserMatchRequest(int userId, int id);
        Task<BooleanReturnDto> ReplyMatchRequest(int userId, string action, int id);
        Task<BooleanReturnDto> CancelMatchRequest(int userId, int id);
        Task<MatchesResultDto?> CreateUserMatch(int userId, AddMatchDto data);
        Task<ResultPaginate<MatchesResultDto>?> GetUserMatches(int userId, int skip = 0, int limit = 50, string sort = "desc");
    }
}