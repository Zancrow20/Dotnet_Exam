using DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Features.Game.Commands.LeaveGame;

public class LeaveGameCommandHandler : IRequestHandler<LeaveGameCommand>
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<Domain.Entities.User> _userManager;

    public LeaveGameCommandHandler(AppDbContext dbContext, UserManager<Domain.Entities.User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task Handle(LeaveGameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        var game = await _dbContext.Games
            .FirstOrDefaultAsync(g => g.GameId == request.GameId,
                cancellationToken: cancellationToken);
        if (game.PlayerOne == user.Id)
        {
            game.PlayerOne = game.PlayerTwo;
            game.PlayerTwo = null;
        }
        else if (game.PlayerTwo == user.Id)
            game.PlayerTwo = null;
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}