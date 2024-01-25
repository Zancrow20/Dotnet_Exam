using Contracts;
using MediatR;

namespace WebApi.Features.Game.Queries.GetAllGames;

public class GetAllGamesQuery : IRequest<Result<GamesDto, string>>
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}