using API.Graphql.Query;

namespace API.Graphql.Schema;

public class UserSchema : GraphQL.Types.Schema
{
    public UserSchema(UserQuery userQuery)
    {
        Query = userQuery;
    }
}