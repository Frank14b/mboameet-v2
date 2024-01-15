using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace API.Services
{
    public class ChatService : IChatService
    {

        private readonly DataContext _dataContext;

        public ChatService(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<bool> CheckIfUserIsMatch(string user, string matchUser)
        {
            try
            {
                var query = _dataContext.Matches.Where(x => x.State == (int)MatchStateEnum.approved && x.Status == (int)StatusEnum.enable && (x.User.Equals(ObjectId.Parse(user)) && x.MatchedUser.Equals(ObjectId.Parse(matchUser)) || x.MatchedUser.Equals(ObjectId.Parse(user)) && x.User.Equals(ObjectId.Parse(matchUser))));

                var _result = await query.FirstOrDefaultAsync();

                if (_result == null) return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}