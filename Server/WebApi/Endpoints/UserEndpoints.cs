using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.Shared;
using WebApi.Features.User;

namespace WebApi.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var routeGroup = app.MapGroup("/user").RequireAuthorization();

        routeGroup.MapGet("/{username}", async ([FromServices] IMediator mediator, string username) =>
        {
            var query = new UserQuery(){Username = username};
            var result = await mediator.Send(query);
            return result.Match(Results.Ok, Results.BadRequest);
        }).Produces<UserDto>()
            .Produces<string>(400);
        
        routeGroup.MapGet("", async (HttpContext context, [FromServices] IMediator mediator) =>
        {
            var username = context.User.Identity.Name;
            var query = new UserQuery(){Username = username};
            var result = await mediator.Send(query);
            return result.Match(Results.Ok, Results.BadRequest);
        }).Produces<UserDto>()
            .Produces<string>(400);

        //app.MapGet("/", () => "Hello world").RequireAuthorization();
    }
}