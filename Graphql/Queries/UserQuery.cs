using System.Security.Cryptography.X509Certificates;
using API.DTOs;
using API.Entities;
using API.Graphql.Type;
using API.Interfaces;
using AutoMapper;
using GraphQL;
using GraphQL.Types;

namespace API.Graphql.Query;

public class UserQuery : ObjectGraphType
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserQuery(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;

        var Id = new QueryArgument<StringGraphType> { Name = "id", Description = "User object id" };
        var Keyword = new QueryArgument<StringGraphType> { Name = "keyword", Description = "User email, name, ..." };

        Field<ListGraphType<UserType>>("users").ResolveAsync( async ctx => { 
            // List<AppUser> users = await _userService.GetUsers();
            // var data = _mapper.Map<ResultUserDto>(users);
            return await ResultUserList();
        });

        Field<ListGraphType<UserType>>("user").Arguments(new QueryArguments(FindUserDto.Id, FindUserDto.Keyword))
        .Resolve(ctx => { 
            return _userService.GetUserById(ctx.GetArgument<string>("id")); 
            }
        );
        
        async Task<List<ResultUserDto>> ResultUserList() {
            List<AppUser> users = await _userService.GetUsers();
            return _mapper.Map<List<ResultUserDto>>(users);
        }
    }
}
