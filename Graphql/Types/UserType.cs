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
    public static QueryArgument<IntGraphType> Id { get; set; } = new QueryArgument<IntGraphType> { Name = "id", Description = "User object id" };

    public static QueryArgument<StringGraphType> Keyword { get; set; } = new QueryArgument<StringGraphType> { Name = "keyword", Description = "User email, name, ..." };
}

public class CreateUserType : InputObjectGraphType
{
    public CreateUserType()
    {
        Field<StringGraphType>("userName");
        Field<StringGraphType>("firstName");
        Field<StringGraphType>("lastName");
        Field<StringGraphType>("email");
        Field<StringGraphType>("password");
    }
}