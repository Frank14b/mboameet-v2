namespace API.Interfaces
{
    public interface IMatchService
    {
        Task<bool> CheckIfUserIsMatch(string user, string matchUser);
        Task<bool> CheckIfUserReceivedMatchRequest(string user, string matchUser, int type);
        Task<bool> CheckIfUserSendMatchRequest(string user, string matchUser, int type);
    }
}