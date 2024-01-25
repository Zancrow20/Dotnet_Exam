using Contracts;
using MediatR;

namespace WebApi.Features.Rating.Queries.GetLastPageNumber;

public class LastPageQuery : IRequest<Result<int, string>>
{
    public int PageSize { get; set; }
}