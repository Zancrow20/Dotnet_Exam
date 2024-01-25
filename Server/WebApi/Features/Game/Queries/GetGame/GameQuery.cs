using Contracts;
using MediatR;

namespace WebApi.Features.Game.Queries.GetGame;

public class GameQuery : IRequest<Result<GameDto, string>>
{
    public string GameId { get; set; }
}