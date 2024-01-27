using Contracts;
using MediatR;

namespace WebApi.Features.Rating.Queries.GetRating;

public class RatingQuery : IRequest<Result<RatingDto, string>>
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}