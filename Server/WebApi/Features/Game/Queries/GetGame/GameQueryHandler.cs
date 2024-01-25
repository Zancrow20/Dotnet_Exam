using Contracts;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Features.Game.Queries.GetGame;

public class GameQueryHandler : IRequestHandler<GameQuery, Result<GameDto, string>>
{
    private readonly AppDbContext _dbContext;

    public GameQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GameDto, string>> Handle(GameQuery request, CancellationToken cancellationToken)
    {
        var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameId == request.GameId,
            cancellationToken: cancellationToken);
        if (game == null)
            return "Игры с таким id не существует!";
        return new GameDto()
        {
            GameId = game.GameId,
            Owner = game.Owner,
            Date = game.Date,
            MaxRating = game.MaxRating,
            Status = game.Status
        };
    }
}