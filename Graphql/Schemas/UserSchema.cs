using API.Graphql.Query;
using API.Mutation;

namespace API.Graphql.Schema;

public class UserSchema : GraphQL.Types.Schema
{
    public UserSchema(UserQuery userQuery, UserMutation userMutation)
    {
        Query = userQuery;
        Mutation = userMutation;
    }
}