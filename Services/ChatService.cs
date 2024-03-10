using API.AppHub;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace API.Services
{
    public class ChatService : IChatService
    {
 
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public ChatService(DataContext context, IMapper mapper)
        {
            _dataContext = context;
            _mapper = mapper;
        }

        public async Task<bool> CheckIfUserIsMatch(int user, int matchUser)
        {
            try
            {
                var query = _dataContext.Matches.Where(x => x.State == (int)MatchStateEnum.approved && x.Status == (int)StatusEnum.enable && (x.UserId.Equals(user) && x.MatchedUserId.Equals(matchUser) || x.MatchedUserId.Equals(user) && x.UserId.Equals(matchUser)));

                var _result = await query.FirstOrDefaultAsync();

                if (_result == null) return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Chat?> SendMessage(SendMessageDto data, int userId) {
            try
            {
                var newChat = _mapper.Map<Chat>(data);
                newChat.MessageType = (int)EnumMessageType.text;
                newChat.Sender = userId;

                _dataContext.Add(newChat);
                await _dataContext.SaveChangesAsync();

                return newChat;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ResultPaginate<MessageResultDto>?> GetMessages(int sender, int receiver, int skip = 0, int limit = 50, string sort = "desc") {
            try
            {
                var query = _dataContext.Chats.Where(
                    m => m.Status == (int)StatusEnum.enable 
                    && (
                        (m.Sender == sender && m.Receiver == receiver) || (m.Receiver == sender && m.Sender == receiver)
                    )
                );

                int totalMessage = await query.CountAsync();

                query = sort == "desc" ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt);

                List<Chat>? data = await query.Skip(skip).Take(limit).ToListAsync();

                var result = _mapper.Map<IEnumerable<MessageResultDto>>(data);

                return new ResultPaginate<MessageResultDto>
                {
                    Data = result,
                    Limit = limit,
                    Skip = skip,
                    Total = totalMessage
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}