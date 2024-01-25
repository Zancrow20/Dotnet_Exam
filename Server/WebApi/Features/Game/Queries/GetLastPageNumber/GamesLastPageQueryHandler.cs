using Contracts;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Rating.Queries.GetLastPageNumber;

namespace WebApi.Features.Game.Queries.GetLastPageNumber;

public class GamesLastPageQueryHandler : IRequestHandler<GamesLastPageQuery, Result<int, string>>
{
    private readonly AppDbContext _dbContext;

    public GamesLastPageQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<int, string>> Handle(GamesLastPageQuery request, CancellationToken cancellationToken)
    {
        var count = await _dbContext.Games.CountAsync(cancellationToken: cancellationToken);
        return (int) Math.Ceiling(count / (double) request.PageSize);
    }
}