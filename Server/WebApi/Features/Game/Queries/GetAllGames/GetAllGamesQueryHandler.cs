using Contracts;
using DataAccess;
using Domain.Entities;
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
            .Select(g => new
            {
                Game = g,
                SortKey = (g.Status == Status.New ? 1 : 0) +
                          (g.Status == Status.Started ? 2 : 0) + 
                          (g.Status == Status.Finished ? 3 : 0)
                    
            })
            .OrderBy(g => g.SortKey)
            .ThenByDescending(g => g.Game.Date)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(g => new GameDto()
            {
                GameId = g.Game.GameId,
                Owner = g.Game.Owner,
                OwnerName = g.Game.OwnerName,
                Date = g.Game.Date,
                MaxRating = g.Game.MaxRating,
                Status = g.Game.Status
            })
            .ToListAsync(cancellationToken: cancellationToken);
        return new GamesDto() {Games = games};
    }
}