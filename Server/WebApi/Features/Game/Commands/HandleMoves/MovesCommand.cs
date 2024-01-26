using Contracts;
using MediatR;

namespace WebApi.Features.Game.Commands.HandleMoves;

public class MovesCommand : IRequest<GameResult>
{
    public UserMove UserMove1 { get; set; }
    public UserMove UserMove2 { get; set; }
    public string GameId { get; set; }
}