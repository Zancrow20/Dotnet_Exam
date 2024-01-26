using Contracts;
using DataAccess;
using Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Services.Mongo;
using WebApi.Services.RabbitMq;

namespace WebApi.Services.GameService;

public interface IGameService
{
    Task<bool> CheckCanUserJoin(string gameId, string? username);

    Task<GameResult> HandleMove(UserMove userMove1, UserMove userMove2, string gameId);

    Task AddToGame(string username, string gameId);
    
    Task ChangeGameStatus(Status status, string gameId);
    
    Task DeleteFromGame(string username, string gameId);
}

public class GameService : IGameService
{
    private readonly AppDbContext _dbContext;
    private readonly IRatingRepository _ratingRepository;
    private readonly UserManager<User> _userManager;
    private readonly IBus _bus;

    public GameService(AppDbContext dbContext, IRatingRepository ratingRepository, UserManager<User> userManager, IBus bus)
    {
        _dbContext = dbContext;
        _ratingRepository = ratingRepository;
        _userManager = userManager;
        _bus = bus;
    }

    public async Task<bool> CheckCanUserJoin(string gameId, string? username)
    {
        var gameRating = (await _dbContext.Games.FirstOrDefaultAsync(g => g.GameId == gameId))!.MaxRating;
        var userId = (await _userManager.FindByNameAsync(username)).Id;
        var userRating = await _ratingRepository.GetRatingByIdAsync(userId, CancellationToken.None);

        return gameRating > userRating.Score;
    }
    
    public async Task<GameResult> HandleMove(UserMove userMove1, UserMove userMove2, string gameId)
    {
        var firstUserMove = userMove1.Figure;
        var secondUserMove = userMove2.Figure;
        var firstUser = await _userManager.FindByNameAsync(userMove1.Username);
        var secondUser = await _userManager.FindByNameAsync(userMove1.Username);

        UserMove winner = null;
        UserMove loser = null;
        var chatMessage = new ChatMessage()
        {
            From = "Server",
            GameId = gameId,
            To = null,
        };
        
        if (firstUserMove == secondUserMove)
        {
            await _bus.Publish(new RabbitMqMessage(firstUser.Id, secondUser.Id, true));
            chatMessage.Message = $"Ничья между игроками {firstUser.UserName} и {secondUser.UserName}";
        }
        else if (firstUserMove == Figure.Scissors && secondUserMove == Figure.Paper ||
                 firstUserMove == Figure.Rock && secondUserMove == Figure.Scissors ||
                 firstUserMove == Figure.Paper && secondUserMove == Figure.Rock)
        {
            await _bus.Publish(new RabbitMqMessage(firstUser.Id, secondUser.Id, false));
            chatMessage.Message = $"Игрок {firstUser.UserName} победил";
            winner = userMove1;
            loser = userMove2;
        }
        else
        {
            await _bus.Publish(new RabbitMqMessage( secondUser.Id, firstUser.Id, false));
            chatMessage.Message = $"Игрок {secondUser.UserName} победил";
            winner = userMove2;
            loser = userMove1;
        }

        _dbContext.ChatMessages.Add(chatMessage);
        await _dbContext.SaveChangesAsync();
        return new GameResult(winner, loser, chatMessage.Message);
    }

    public async Task AddToGame(string username, string gameId)
    {
        var user = await _userManager.FindByNameAsync(username);
        var game = await _dbContext.Games
            .FirstOrDefaultAsync(g => g.GameId == gameId);
        if (game.PlayerOne == null)
            game.PlayerOne = user.Id;
        else if (game.PlayerTwo == null)
            game.PlayerTwo = user.Id;
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ChangeGameStatus(Status status, string gameId)
    {
        var game = await _dbContext.Games
            .FirstOrDefaultAsync(g => g.GameId == gameId);
        game.Status = status;
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteFromGame(string username, string gameId)
    {
        var user = await _userManager.FindByNameAsync(username);
        var game = await _dbContext.Games
            .FirstOrDefaultAsync(g => g.GameId == gameId);
        if (game.PlayerOne == user.Id)
        {
            game.PlayerOne = game.PlayerTwo;
            game.PlayerTwo = null;
        }
        else if (game.PlayerTwo == user.Id)
            game.PlayerTwo = null;
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync();
    }
}