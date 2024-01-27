using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.Game.Commands;
using WebApi.Features.Game.Commands.CreateGame;
using WebApi.Features.Game.Queries.GetAllGames;
using WebApi.Features.Game.Queries.GetGame;
using WebApi.Features.Game.Queries.GetLastPageNumber;

namespace WebApi.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this WebApplication app)
    {
        var routeGroup = app.MapGroup("/game").RequireAuthorization();

        routeGroup.MapPost("/create", async (HttpContext context, [FromServices] IMediator mediator, 
            [FromQuery] int maxRating) =>
        {
            var userName = context.User.Identity!.Name;
            var command = new CreateGameCommand() { MaxRating = maxRating, Owner = userName!};
            var result = await mediator.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        });
        
        routeGroup.MapGet("/all", async ([FromServices] IMediator mediator, 
            [FromQuery] int pageNumber, [FromQuery] int pageSize) =>
        {
            var query = new GetAllGamesQuery() {PageSize = pageSize, PageNumber = pageNumber};
            var result = await mediator.Send(query);
            return result.Match(Results.Ok, Results.BadRequest);
        });
        
        routeGroup.MapGet("/lastPage", async ([FromServices] IMediator mediator, 
            [FromQuery] int pageSize) =>
        {
            var query = new GamesLastPageQuery() {PageSize = pageSize};
            var result = await mediator.Send(query);
            return result.Match(Results.Ok, Results.BadRequest);
        });
        
        routeGroup.MapGet("", async ([FromServices] IMediator mediator, [FromQuery] string gameId) =>
        {
            var query = new GameQuery() {GameId = gameId};
            var result = await mediator.Send(query);
            return result.Match(Results.Ok, Results.BadRequest);
        });
        
    }
}