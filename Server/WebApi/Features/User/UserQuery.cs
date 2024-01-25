using Contracts;
using MediatR;
using WebApi.Features.Shared;

namespace WebApi.Features.User;

public class UserQuery: IRequest<Result<UserDto, string>>
{
    public string Username { get; set; }
}