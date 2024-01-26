using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        var routeGroup = app.MapGroup("/chat");

        routeGroup.MapGet("/getHistory", async ([FromServices] IMediator mediator, string gameId) =>
        {
            
        });
    }
}