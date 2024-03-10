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
            // CreateMap<AddMatchDto, Match>();
            CreateMap<Match, MatchesResultDto>();
            CreateMap<CreateUserDto, User>();
            // CreateMap<AppRole, RoleResultDtos>();
            // CreateMap<DeleteRolesDto, AppRole>();
            // CreateMap<DeleteAccessDto, AppAcces>();
            // CreateMap<CreateBusinessDto, AppBusiness>();
            // CreateMap<AppBusiness, BusinessResultDto>();
            // CreateMap<AppBusiness, BusinessResultListDto>();
            // CreateMap<UpdateBusinessDto, AppBusiness>();
            // CreateMap<RoleaccessPostDto, AppRoleAcces>();
            // CreateMap<AppRoleAcces, RoleaccessResultDto>();
            // CreateMap<AppPropertyType, PropertyTResultListDto>();
            // CreateMap<PropertyTCreateDto, AppPropertyType>();
            // CreateMap<AppPropertyMeta, PropertyMTResultDto>();
            // CreateMap<PropertyMTCreateDto, AppPropertyMeta>();
            // CreateMap<AppProperty, PropertiesResultListDto>();
            // CreateMap<PropertyTUpdateDto, AppPropertyType>();
            // CreateMap<PropertiesCreateDto, AppProperty>();
            // CreateMap<UserBusines, UserBusinessResultDto>();
            // CreateMap<UserBusinessDto, UserBusines>();
        }
    }
}