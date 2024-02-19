using API.DTOs;
using API.Entities;
using API.Graphql.Type;
using API.Interfaces;
using AutoMapper;
using GraphQL;
using GraphQL.Types;

namespace API.Mutation;

public class UserMutation : ObjectGraphType
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserMutation(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;

        Field<UserType>("createUser").Arguments(new QueryArguments(new QueryArgument<CreateUserType> { Name = "user" }))
            .ResolveAsync(async ctx =>
            {
                return await SignUp(ctx.GetArgument<RegisterDto>("user"));
            }
        );

        async Task<ResultUserDto> SignUp(RegisterDto data)
        {
            AppUser? user = await _userService.CreateUserAccount(data);
            return _mapper.Map<ResultUserDto>(user);
        }
    }
}