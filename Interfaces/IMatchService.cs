using API.DTOs;

namespace API.Interfaces
{
    public interface IMatchService
    {
        Task<BooleanReturnDto> CheckIfUserIsMatch(string user, string matchUser);
        Task<BooleanReturnDto> CheckIfUserReceivedMatchRequest(string user, string matchUser, int type);
        Task<BooleanReturnDto> CheckIfUserSendMatchRequest(string user, string matchUser, int type);
        Task<BooleanReturnDto> CheckIfMatchRequestExist(string user, string matchUser);
    }
}