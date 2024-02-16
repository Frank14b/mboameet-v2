using GraphQL.Types;
using API.Entities;
using API.DTOs;

namespace API.Graphql.Type;

public class UserType : ObjectGraphType<ResultUserDto>
{
    public UserType()
    {
        Field(m => m.Id);
        Field(m => m.UserName);
        Field(m => m.FirstName);
        Field(m => m.LastName);
        Field(m => m.CreatedAt);
        Field(m => m.UpdatedAt);
    }
}

public class FindUserDto
{
    public static QueryArgument<StringGraphType> Id { get; set; } = new QueryArgument<StringGraphType> { Name = "id", Description = "User object id" };

    public static QueryArgument<StringGraphType> Keyword { get; set; } = new QueryArgument<StringGraphType> { Name = "keyword", Description = "User email, name, ..." };
}

public class CreateUserDto : InputObjectGraphType
{

}