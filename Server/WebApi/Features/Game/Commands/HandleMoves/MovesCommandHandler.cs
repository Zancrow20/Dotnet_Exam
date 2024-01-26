using Contracts;
using DataAccess;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Services.Mongo;
using WebApi.Services.RabbitMq;

namespace WebApi.Features.Game.Commands.HandleMoves;

public class MovesCommandHandler : IRequestHandler<MovesCommand, GameResult>
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IBus _bus;

    public MovesCommandHandler(UserManager<Domain.Entities.User> userManager,
        IBus bus,
        AppDbContext dbContext)
    {
        _userManager = userManager;
        _bus = bus;
        _dbContext = dbContext;
    }

    public async Task<GameResult> Handle(MovesCommand request, CancellationToken cancellationToken)
    {
        var firstUserMove = request.UserMove1.Figure;
        var secondUserMove = request.UserMove2.Figure;
        var firstUser = await _userManager.FindByNameAsync(request.UserMove1.Username);
        var secondUser = await _userManager.FindByNameAsync(request.UserMove2.Username);

        UserMove winner = null;
        UserMove loser = null;
        var chatMessage = new ChatMessage()
        {
            From = "Server",
            GameId = request.GameId,
            To = null,
        };
        
        if (firstUserMove == secondUserMove)
        {
            await _bus.Publish(new RabbitMqMessage(firstUser.Id, secondUser.Id, true),
                cancellationToken);
            chatMessage.Message = $"Ничья между игроками {firstUser.UserName}:{firstUserMove.ToString()} " +
                                  $"и {secondUser.UserName}:{secondUserMove.ToString()}";
        }
        else if (firstUserMove == Figure.Scissors && secondUserMove == Figure.Paper ||
                 firstUserMove == Figure.Rock && secondUserMove == Figure.Scissors ||
                 firstUserMove == Figure.Paper && secondUserMove == Figure.Rock)
        {
            await _bus.Publish(new RabbitMqMessage(firstUser.Id, secondUser.Id, false),
                cancellationToken);
            chatMessage.Message = $"Игрок {firstUser.UserName}:{firstUserMove.ToString()} " +
                                  $"победил игрока {secondUser.UserName}:{secondUserMove.ToString()}";
            winner = request.UserMove1;
            loser = request.UserMove2;
        }
        else
        {
            await _bus.Publish(new RabbitMqMessage( secondUser.Id, firstUser.Id, false),
                cancellationToken);
            chatMessage.Message = $"Игрок {secondUser.UserName}:{secondUserMove.ToString()} " +
                                  $"победил игрока{firstUser.UserName}:{firstUserMove.ToString()}";
            winner = request.UserMove2;
            loser = request.UserMove1;
        }

        _dbContext.ChatMessages.Add(chatMessage);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new GameResult(winner, loser, chatMessage.Message);
    }
}