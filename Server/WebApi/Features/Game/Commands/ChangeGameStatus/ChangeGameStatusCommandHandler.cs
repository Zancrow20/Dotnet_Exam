using DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Features.Game.Commands.ChangeGameStatus;

public class ChangeGameStatusCommandHandler : IRequestHandler<ChangeGameStatusCommand>
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<Domain.Entities.User> _userManager;
    
    public async Task Handle(ChangeGameStatusCommand request, CancellationToken cancellationToken)
    {
        var game = await _dbContext.Games
            .FirstOrDefaultAsync(g => g.GameId == request.GameId,
                cancellationToken: cancellationToken);
        game.Status = request.Status;
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}