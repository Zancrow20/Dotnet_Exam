using DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Services.Mongo;

namespace WebApi.Features.Game.Commands.JoinGame;

public class JoinGameCommandHandler : IRequestHandler<JoinGameCommand>
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<Domain.Entities.User> _userManager;

    public JoinGameCommandHandler(AppDbContext dbContext,
        UserManager<Domain.Entities.User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task Handle(JoinGameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        var game = await _dbContext.Games
            .FirstOrDefaultAsync(g => g.GameId == request.GameId, cancellationToken: cancellationToken);
        if (game.PlayerOne == null)
            game.PlayerOne = user.Id;
        else if (game.PlayerTwo == null)
            game.PlayerTwo = user.Id;
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}