using Domain.Entities;

namespace WebApi.Hubs;

public interface IGameHubClient
{
    Task FinishGame(FinishGameDto gameDto);
    Task StartGame();
    Task JoinRefused(string message);
    Task ReceiveMessage(MessageDto messageDto);
    Task AskFigure();
}

public class FinishGameDto
{
    public string WinnerName { get; set; }
    public string LoserName { get; set; }
    public Figure WinnerFigure { get; set; }
    public Figure LoserFigure { get; set; }
    public string Message { get; set; }
}

public record MessageDto(string Username, string Message);