using Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Features.Shared;
using WebApi.Services.Mongo;

namespace WebApi.Features.Rating.Queries.GetRating;

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
        var rating = (await _ratingRepository.GetAllRatingsAsync(cancellationToken))
            .OrderByDescending(r => r.Score)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var users = _userManager.Users.AsEnumerable();

        var usersRating = rating
            .Join(users,
                r => r.UserId,
                u => u.Id,
                (r, u) => new UserDto() {Rating = r.Score, Username = u.UserName});
        return new RatingDto(usersRating);
    }
}