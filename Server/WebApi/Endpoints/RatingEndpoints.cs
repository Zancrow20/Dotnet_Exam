using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.Rating;
using WebApi.Features.Rating.Queries.GetLastPageNumber;
using WebApi.Features.Rating.Queries.GetRating;

namespace WebApi.Endpoints;

public static class RatingEndpoints
{
    public static void MapRatingEndpoints(this WebApplication app)
    {
        var routeGroup = app.MapGroup("/rating").RequireAuthorization();

        routeGroup.MapGet("/all", async ([FromServices] IMediator mediator, 
            [FromQuery] int pageNumber, [FromQuery] int pageSize) =>
        {
            var query = new RatingQuery() {PageSize = pageSize, PageNumber = pageNumber};
            var result = await mediator.Send(query);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .Produces<RatingDto>()
        .Produces<string>(400);
        
        routeGroup.MapGet("/lastPage", async ([FromServices] IMediator mediator, 
            [FromQuery] int pageSize) =>
        {
            var query = new LastPageQuery() {PageSize = pageSize};
            var result = await mediator.Send(query);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .Produces<int>()
        .Produces<string>(400);
    }
}