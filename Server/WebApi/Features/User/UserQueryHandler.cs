using Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Features.Shared;
using WebApi.Services.Mongo;

namespace WebApi.Features.User;

public class UserQueryHandler : IRequestHandler<UserQuery, Result<UserDto, string>>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IRatingRepository _ratingRepository;

    public UserQueryHandler(UserManager<Domain.Entities.User> userManager, IRatingRepository ratingRepository)
    {
        _userManager = userManager;
        _ratingRepository = ratingRepository;
    }

    public async Task<Result<UserDto, string>> Handle(UserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return "Пользователя с таким именем не существует!";
        }

        var rating = await _ratingRepository.GetRatingByIdAsync(user.Id, cancellationToken);
        var userDto = new UserDto() {Username = user.UserName, Rating = rating!.Score};
        return userDto;
    }
}