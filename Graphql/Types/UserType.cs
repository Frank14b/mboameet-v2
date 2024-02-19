using GraphQL.Types;
using API.DTOs;

namespace API.Graphql.Type;

public class UserType : ObjectGraphType<ResultUserDto>
{
    public UserType()
    {
        Field(m => m.Id);
        Field(m => m.UserName);
        Field(m => m.FirstName, nullable: true);
        Field(m => m.Email, nullable: true);
        Field(m => m.Status);
        Field(m => m.LastName, nullable: true);
        Field(m => m.CreatedAt);
        Field(m => m.UpdatedAt);
    }
}

public class FindUserDto
{
    public static QueryArgument<StringGraphType> Id { get; set; } = new QueryArgument<StringGraphType> { Name = "id", Description = "User object id" };

    public static QueryArgument<StringGraphType> Keyword { get; set; } = new QueryArgument<StringGraphType> { Name = "keyword", Description = "User email, name, ..." };
}

public class CreateUserType : InputObjectGraphType
{
    public CreateUserType() {
        Field<IntGraphType>("id");
        Field<StringGraphType>("username");
        Field<StringGraphType>("firstname");
        Field<StringGraphType>("lastname");
        Field<StringGraphType>("email");
    }
}