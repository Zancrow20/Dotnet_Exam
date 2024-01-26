using Contracts;
using MediatR;

namespace WebApi.Features.Game.Commands.CreateGame;

public class CreateGameCommand : IRequest<Result<GameDto, string>>
{
    public string Owner { get; set; }
    public int MaxRating { get; set; }
}