using API.DTOs;
using API.Graphql.Type;
using API.Interfaces;
using GraphQL;
using GraphQL.Types;

namespace API.Mutation;

public class UserMutation : ObjectGraphType
{
    private readonly IUserService _userService;
    public UserMutation(IUserService userService)
    {
        _userService = userService;

        Field<UserType>("createUser").Arguments(new QueryArguments(new QueryArgument<CreateUserType> { Name = "user" }))
        .ResolveAsync(async ctx => { 
            return await _userService.CreateUserAccount(ctx.GetArgument<RegisterDto>("user")); 
            }
        );
    }
}