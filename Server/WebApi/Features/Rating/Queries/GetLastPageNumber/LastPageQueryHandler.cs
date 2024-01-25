using Contracts;
using MediatR;
using WebApi.Services.Mongo;

namespace WebApi.Features.Rating.Queries.GetLastPageNumber;

public class LastPageQueryHandler : IRequestHandler<LastPageQuery, Result<int, string>>
{
    private readonly IRatingRepository _ratingRepository;

    public LastPageQueryHandler(IRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }

    public async Task<Result<int, string>> Handle(LastPageQuery request, CancellationToken cancellationToken)
    {
        var count = (await _ratingRepository.GetAllRatingsAsync(cancellationToken)).Count();
        return (int)Math.Ceiling(count / (double)request.PageSize);
    }
}