using API.DTOs;
using API.Entities;
using API.Graphql.Type;
using API.Interfaces;
using AutoMapper;
using GraphQL;
using GraphQL.Types;
// using Microsoft.AspNetCore.Authorization;

namespace API.Graphql.Query;

public class UserQuery : ObjectGraphType
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    // [Authorize]
    public UserQuery(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;

        var Id = new QueryArgument<IntGraphType> { Name = "id", Description = "User unique id" };
        var Keyword = new QueryArgument<StringGraphType> { Name = "keyword", Description = "User email, name, ..." };

        Field<ListGraphType<UserType>>("users").ResolveAsync(async ctx =>
        {

            Console.WriteLine(ctx.User);

            if (ctx.User == null)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user list");
            }

            return await ResultUserList();
        });

        Field<UserType>("user").Arguments(new QueryArguments(FindUserDto.Id, FindUserDto.Keyword))
        .ResolveAsync(async ctx =>
        {
            return await ResultUser(ctx.GetArgument<int>("id"));
        });

        async Task<List<ResultUserDto>> ResultUserList()
        {
            List<AppUser> users = await _userService.GetUsers();
            return _mapper.Map<List<ResultUserDto>>(users);
        }

        async Task<ResultUserDto> ResultUser(int id)
        {
            AppUser? user = await _userService.GetUserById(id);
            return _mapper.Map<ResultUserDto>(user);
        }
    }
}
