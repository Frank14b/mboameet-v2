using Api.DTOs;
using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, ResultUpdateUserDto>();
            CreateMap<User, ResultUserDto>();
            CreateMap<User, ResultloginDto>();
            CreateMap<Chat, MessageResultDto>();
            CreateMap<SendMessageDto, Chat>();
            CreateMap<Match, MatchesResultDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<Feed, FeedResultDto>();
            CreateMap<User, ResultUserFeedDto>();
            CreateMap<FeedFile, ResultFeedFileDto>();
            CreateMap<FeedComment, ResultFeedCommentDto>();
            CreateMap<FeedLike, ResultFeedLike>();
        }
    }
}