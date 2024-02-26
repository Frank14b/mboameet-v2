using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IChatService
    {
        Task<bool> CheckIfUserIsMatch(string user, string matchUser);
        Task<AppChat?> SendMessage(SendMessageDto data, string userId);
        Task<ResultPaginate<MessageResultDto>?> GetMessages(string sender, string receiver, int skip = 0, int limit = 50, string sort = "desc");
    }
}