using Contracts;
using MediatR;

namespace WebApi.Features.Rating;

public class RatingQuery : IRequest<Result<RatingDto, string>>
{
    public int PageSize { get; set; }
    public int StartIndex { get; set; }
}