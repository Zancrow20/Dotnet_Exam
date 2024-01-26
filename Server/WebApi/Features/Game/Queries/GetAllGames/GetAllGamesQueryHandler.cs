using Contracts;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Features.Game.Queries.GetAllGames;

public class GetAllGamesQueryHandler : IRequestHandler<GetAllGamesQuery, Result<GamesDto,string>>
{
    private readonly AppDbContext _dbContext;

    public GetAllGamesQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Result<GamesDto, string>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
    {
        var games = await _dbContext.Games
            .OrderBy(g => g.Status)
            .ThenByDescending(g => g.Date)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(game => new GameDto()
            {
                GameId = game.GameId,
                Owner = game.Owner,
                OwnerName = game.OwnerName,
                Date = game.Date,
                MaxRating = game.MaxRating,
                Status = game.Status
            }).ToListAsync(cancellationToken: cancellationToken);
        return new GamesDto() {Games = games};
    }
}