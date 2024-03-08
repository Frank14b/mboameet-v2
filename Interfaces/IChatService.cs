using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IChatService
    {
        Task<bool> CheckIfUserIsMatch(int user, int matchUser);
        Task<AppChat?> SendMessage(SendMessageDto data, int userId);
        Task<ResultPaginate<MessageResultDto>?> GetMessages(int sender, int receiver, int skip = 0, int limit = 50, string sort = "desc");
    }
}