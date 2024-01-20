using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace API.Services
{
    public class MatchService : IMatchService
    {

        private readonly DataContext _dataContext;

        public MatchService(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<BooleanReturnDto> CheckIfUserIsMatch(string user, string matchUser)
        {
            try
            {
                var query = _dataContext.Matches.Where(x => x.State == (int)MatchStateEnum.approved && x.Status == (int)StatusEnum.enable && (x.UserId.Equals(ObjectId.Parse(user)) && x.MatchedUserId.Equals(ObjectId.Parse(matchUser)) || x.MatchedUserId.Equals(ObjectId.Parse(user)) && x.UserId.Equals(ObjectId.Parse(matchUser))));

                var _result = await query.FirstOrDefaultAsync();

                if (_result == null) return new BooleanReturnDto
                {
                    Status = false
                };

                return new BooleanReturnDto
                {
                    Status = true,
                    Data = _result
                };
            }
            catch (Exception)
            {
                return new BooleanReturnDto
                {
                    Status = false
                };
            }
        }

        public async Task<BooleanReturnDto> CheckIfMatchRequestExist(string user, string matchUser)
        {
            try
            {
                var query = _dataContext.Matches.Where(x => x.Status == (int)StatusEnum.enable && (x.UserId.Equals(ObjectId.Parse(user)) && x.MatchedUserId.Equals(ObjectId.Parse(matchUser)) || x.MatchedUserId.Equals(ObjectId.Parse(user)) && x.UserId.Equals(ObjectId.Parse(matchUser))));

                var _result = await query.CountAsync();

                if (_result == 0) return new BooleanReturnDto
                {
                    Status = false
                };

                return new BooleanReturnDto
                {
                    Status = true,
                    // Data = _result
                };
            }
            catch (Exception)
            {
                return new BooleanReturnDto
                {
                    Status = false
                };
            }
        }

        public async Task<BooleanReturnDto> CheckIfUserSendMatchRequest(string user, string matchUser, int type = (int)MatchStateEnum.inititated)
        {
            try
            {
                var query = _dataContext.Matches.Where(x => x.Status == (int)StatusEnum.enable);

                var _result = await query.FirstOrDefaultAsync();

                if (_result == null) return new BooleanReturnDto
                {
                    Status = false
                };

                if (_result.State != type)
                {
                    return new BooleanReturnDto
                    {
                        Status = false
                    };
                }

                return new BooleanReturnDto
                {
                    Status = true,
                    Data = _result
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BooleanReturnDto
                {
                    Status = false
                };
            }
        }

        public async Task<BooleanReturnDto> CheckIfUserReceivedMatchRequest(string user, string matchUser, int type = (int)MatchStateEnum.inititated)
        {
            try
            {
                var query = _dataContext.Matches.Where(x => x.Status == (int)StatusEnum.enable && x.MatchedUserId.Equals(ObjectId.Parse(user)) && x.UserId.Equals(ObjectId.Parse(matchUser)));

                var _result = await query.FirstOrDefaultAsync();

                if (_result == null) return new BooleanReturnDto
                {
                    Status = false
                };

                if (_result.State != type)
                {
                    return new BooleanReturnDto
                    {
                        Status = false
                    };
                }

                return new BooleanReturnDto
                {
                    Status = true,
                    Data = _result
                };
            }
            catch (Exception)
            {
                return new BooleanReturnDto
                {
                    Status = false
                }; ;
            }
        }

        public async Task<AppMatch?> GetUserMatchSendRequest(string userId)
        {
            try
            {
                var query = _dataContext.Matches.Where(m => m.UserId.ToString() == userId && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated);
                var match = await query.FirstOrDefaultAsync();

                return match;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<AppMatch?> GetUserMatchReceivedRequest(string userId)
        {
            try
            {
                var query = _dataContext.Matches.Where(m => m.MatchedUserId.ToString() == userId && m.Status == (int)StatusEnum.enable && m.State == (int)MatchStateEnum.inititated);
                var match = await query.FirstOrDefaultAsync();

                return match;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}