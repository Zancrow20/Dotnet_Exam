using Domain.Entities;

namespace WebApi.Features.Game;

public class GameDto
{
    public string GameId { get; set; }
    public string Owner { get; set; }
    public DateTime Date { get; set; }
    public Status Status { get; set; }
    public int MaxRating { get; set; }
}