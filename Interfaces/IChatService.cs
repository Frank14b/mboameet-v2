namespace API.Interfaces
{
    public interface IChatService
    {
        Task<bool> CheckIfUserIsMatch(string user, string matchUser);
    }
}