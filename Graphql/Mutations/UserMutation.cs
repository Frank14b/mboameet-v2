using API.Graphql.Type;
using API.Interfaces;
using GraphQL;
using GraphQL.Types;

namespace API.Mutation;

public class UserMutation : ObjectGraphType
{
    private IUserService _userService;
    public UserMutation(IUserService userService)
    {
        _userService = userService;

        Field<ListGraphType<UserType>>("users").Resolve(ctx => { return _userService.GetUsers(); });
    }
}