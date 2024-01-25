using Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Shared;
using WebApi.Services.Mongo;

namespace WebApi.Features.Rating;

public class RatingQueryHandler : IRequestHandler<RatingQuery, Result<RatingDto, string>>
{
    private readonly IRatingRepository _ratingRepository;
    private readonly UserManager<Domain.Entities.User> _userManager;

    public RatingQueryHandler(IRatingRepository ratingRepository, UserManager<Domain.Entities.User> userManager)
    {
        _ratingRepository = ratingRepository;
        _userManager = userManager;
    }

    public async Task<Result<RatingDto, string>> Handle(RatingQuery request, CancellationToken cancellationToken)
    {
        var users = _userManager.Users
            .Select(u => new {Username = u.UserName, UserId = u.Id});
        
        var rating = await _ratingRepository.GetAllRatingsAsync(cancellationToken);

        var usersRating = rating
            .Join(users,
                r => r.UserId,
                u => u.UserId,
                (r, u) => new UserDto() {Rating = r.Score, Username = u.Username});
        return new RatingDto(usersRating);
    }
}