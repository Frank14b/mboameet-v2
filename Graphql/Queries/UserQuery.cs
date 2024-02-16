using API.Graphql.Type;
using API.Interfaces;
using GraphQL;
using GraphQL.Types;

namespace API.Graphql.Query;

public class UserQuery : ObjectGraphType
{
    private readonly IUserService _userService;

    public UserQuery(IUserService userService)
    {
        _userService = userService;

        var Id = new QueryArgument<StringGraphType> { Name = "id", Description = "User object id" };
        var Keyword = new QueryArgument<StringGraphType> { Name = "keyword", Description = "User email, name, ..." };

        Field<ListGraphType<UserType>>("users").Resolve(ctx => { return _userService.GetUsers(); });

        Field<ListGraphType<UserType>>("user").Arguments(new QueryArguments(FindUserDto.Id, FindUserDto.Keyword)).Resolve(ctx => { return _userService.GetUserById(ctx.GetArgument<string>("id")); });
    }
}