using Contracts;
using MediatR;

namespace WebApi.Features.Game.Queries.GetLastPageNumber;

public class GamesLastPageQuery : IRequest<Result<int, string>>
{
    public int PageSize { get; set; }
}