using API.Graphql.Query;

namespace API.Graphql.Schema;

public class UserSchema : GraphQL.Types.Schema{
    private readonly UserQuery _userQuery;
    public UserSchema (UserQuery userQuery) {
        _userQuery = userQuery;
    }
}