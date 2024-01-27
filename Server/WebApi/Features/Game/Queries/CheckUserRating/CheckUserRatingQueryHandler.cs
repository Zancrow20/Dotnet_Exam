using DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Services.Mongo;

namespace WebApi.Features.Game.Queries.CheckUserRating;

public class CheckUserRatingQueryHandler : IRequestHandler<CheckUserRatingQuery, bool>
{
    private readonly AppDbContext _dbContext;
    private readonly IRatingRepository _ratingRepository;
    private readonly UserManager<Domain.Entities.User> _userManager;

    public CheckUserRatingQueryHandler(AppDbContext dbContext, IRatingRepository ratingRepository, UserManager<Domain.Entities.User> userManager)
    {
        _dbContext = dbContext;
        _ratingRepository = ratingRepository;
        _userManager = userManager;
    }

    public async Task<bool> Handle(CheckUserRatingQuery request, CancellationToken cancellationToken)
    {
        var gameRating = (await _dbContext.Games.FirstOrDefaultAsync(g => g.GameId == request.GameId,
            cancellationToken: cancellationToken))!.MaxRating;
        var userId = (await _userManager.FindByNameAsync(request.Username)).Id;
        var userRating = await _ratingRepository.GetRatingByIdAsync(userId, CancellationToken.None);

        return gameRating > userRating.Score;
    }
}