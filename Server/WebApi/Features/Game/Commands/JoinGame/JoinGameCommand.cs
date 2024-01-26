using Contracts;
using MediatR;

namespace WebApi.Features.Game.Commands.JoinGame;

public class JoinGameCommand : IRequest
{
    public string Username { get; set; }
    public string GameId { get; set; }
}