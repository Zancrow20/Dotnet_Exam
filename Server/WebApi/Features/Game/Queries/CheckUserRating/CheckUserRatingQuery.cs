using MediatR;

namespace WebApi.Features.Game.Queries.CheckUserRating;

public class CheckUserRatingQuery : IRequest<bool>
{
    public string GameId { get; set; } 
    public string? Username { get; set; }
}