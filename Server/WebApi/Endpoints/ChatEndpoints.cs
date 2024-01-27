using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.ChatHistory.Queries;

namespace WebApi.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        var routeGroup = app.MapGroup("/chat");

        routeGroup.MapGet("/getHistory", async ([FromServices] IMediator mediator, string gameId) =>
        {
            var query = new GetChatHistoryQuery() { GameId = gameId};
            var res = await mediator.Send(query);
            return res.Match(Results.Ok, Results.BadRequest);
        });
    }
}