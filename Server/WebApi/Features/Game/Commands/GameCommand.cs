using Contracts;
using MediatR;

namespace WebApi.Features.Game.Commands;

public class GameCommand : IRequest<Result<GameDto, string>>
{
    public string Owner { get; set; }
    public int MaxRating { get; set; }
}