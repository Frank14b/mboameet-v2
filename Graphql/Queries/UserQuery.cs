using API.Graphql.Type;
using API.Interfaces;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace API.Graphql.Query;

public class UserQuery : ObjectGraphType
{
    private readonly IUserService _userService;

    public UserQuery(IUserService userService)
    {
        _userService = userService;

        AddField(new FieldType
        {
            Name = "users",
            Type = typeof(ListGraphType<UserType>),
            // Resolver = _userService.GetUsers();
        });
    }
}