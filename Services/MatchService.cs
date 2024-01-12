using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
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
                var query = _dataContext.Matches.Where(x => x.State == (int)MatchStateEnum.approved && x.Status == (int)StatusEnum.enable && (x.User.Equals(ObjectId.Parse(user)) && x.MatchedUser.Equals(ObjectId.Parse(matchUser)) || x.MatchedUser.Equals(ObjectId.Parse(user)) && x.User.Equals(ObjectId.Parse(matchUser))));

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
                var query = _dataContext.Matches.Where(x => x.State != (int)MatchStateEnum.inititated && x.Status == (int)StatusEnum.enable && (x.User.Equals(ObjectId.Parse(user)) && x.MatchedUser.Equals(ObjectId.Parse(matchUser)) || x.MatchedUser.Equals(ObjectId.Parse(user)) && x.User.Equals(ObjectId.Parse(matchUser))));

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

        public async Task<BooleanReturnDto> CheckIfUserSendMatchRequest(string user, string matchUser, int type = (int)MatchStateEnum.inititated)
        {
            try
            {
                var query = _dataContext.Matches.Where(x => x.Status == (int)StatusEnum.enable && x.User.Equals(ObjectId.Parse(user)) && x.MatchedUser.Equals(ObjectId.Parse(matchUser)));

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
                };
            }
        }

        public async Task<BooleanReturnDto> CheckIfUserReceivedMatchRequest(string user, string matchUser, int type = (int)MatchStateEnum.inititated)
        {
            try
            {
                var query = _dataContext.Matches.Where(x => x.Status == (int)StatusEnum.enable && x.MatchedUser.Equals(ObjectId.Parse(user)) && x.User.Equals(ObjectId.Parse(matchUser)));

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
    }
}