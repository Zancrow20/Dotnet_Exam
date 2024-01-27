using MediatR;

namespace WebApi.Features.Game.Commands.LeaveGame;

public class LeaveGameCommand : IRequest
{
    public string Username { get; set; }
    public string GameId { get; set; }
}