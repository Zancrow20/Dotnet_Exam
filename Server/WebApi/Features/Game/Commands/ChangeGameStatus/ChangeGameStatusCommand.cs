using Domain.Entities;
using MediatR;

namespace WebApi.Features.Game.Commands.ChangeGameStatus;

public class ChangeGameStatusCommand : IRequest
{
    public Status Status { get; set; }
    public string GameId { get; set; }
}