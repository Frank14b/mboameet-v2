namespace API.Interfaces
{
    public interface IMatchService
    {
        Task<bool> CheckIfUserIsMatch(string user, string matchUser);
    }
}