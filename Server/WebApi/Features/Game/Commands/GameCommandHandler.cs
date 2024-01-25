using Contracts;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Features.Game.Commands;

public class GameCommandHandler : IRequestHandler<GameCommand, Result<GameDto, string>>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly AppDbContext _dbContext;

    public GameCommandHandler(UserManager<Domain.Entities.User> userManager, AppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<Result<GameDto, string>> Handle(GameCommand request, CancellationToken cancellationToken)
    {
        var owner = await _userManager.FindByNameAsync(request.Owner);
        if (owner == null)
            return "Пользователя с таким именем не существует!";

        if (request.MaxRating < 0)
            return "Рейтинг не может быть меньше нуля!";

        var gameId = Guid.NewGuid().ToString();
        var game = new Domain.Entities.Game()
        {
            Date = DateTime.Now,
            GameId = gameId,
            MaxRating = request.MaxRating,
            Owner = owner.Id,
            Status = Status.New
        };

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var gameDto = new GameDto()
        {
            GameId = gameId,
            Date = game.Date,
            MaxRating = request.MaxRating,
            Owner = owner.Id,
            Status = Status.New
        };

        return gameDto;
    }
}