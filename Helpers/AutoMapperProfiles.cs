using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, ResultUpdateUserDto>();
            CreateMap<AppUser, ResultAllUserDto>();
            CreateMap<AppUser, ResultloginDto>();
            CreateMap<AppChat, MessageResultDto>();
            CreateMap<SendMessageDto, AppChat>();
            CreateMap<AddMatchDto, AppMatch>();
            CreateMap<AppMatch, MatchesResultDto>();
            CreateMap<CreateUserDto, AppUser>();
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
            // CreateMap<AppUserBusines, UserBusinessResultDto>();
            // CreateMap<UserBusinessDto, AppUserBusines>();
        }
    }
}