using GraphQL.Types;
using API.Entities;

namespace API.Graphql.Type;

public class UserType : ObjectGraphType<AppUser> {
    public UserType () {
        Field(m => m.Id.ToString());
        Field(m => m.UserName);
        Field(m => m.FirstName);
        Field(m => m.LastName);
        Field(m => m.CreatedAt);
        Field(m => m.UpdatedAt);
    }
}