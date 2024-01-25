using Contracts;
using MediatR;

namespace WebApi.Features.Game.Queries.GetAllGames;

public class GetAllGamesQuery : IRequest<Result<GamesDto, string>>
{
    
}