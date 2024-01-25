using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.Rating;

namespace WebApi.Endpoints;

public static class RatingEndpoints
{
    public static void MapRatingEndpoints(this WebApplication app)
    {
        var routeGroup = app.MapGroup("/rating").RequireAuthorization();

        routeGroup.MapGet("/all", async ([FromServices] IMediator mediator, 
            [FromQuery] int startIndex, [FromQuery] int pageSize) =>
        {
            var query = new RatingQuery() {PageSize = pageSize, StartIndex = startIndex};
            var result = await mediator.Send(query);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .Produces<RatingDto>()
        .Produces<string>(400);
    }
}