using Contracts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApi.Features.ChatHistory.Commands;
using WebApi.Services.GameService;

namespace WebApi.Hubs;

[Authorize]
public class GameHub : Hub<IGameHubClient>
{
    private readonly IMediator _mediator;
    private readonly Store _store;
    private readonly IGameService _gameService;
    

    public GameHub(IMediator mediator, Store store, IGameService gameService)
    {
        _mediator = mediator;
        _store = store;
        _gameService = gameService;
    }

    public async Task JoinGame(string gameId)
    {
        var username = Context.User!.Identity!.Name;
        var game = $"{gameId}_game";
        
        //Check user rating
        var canJoin = await _gameService.CheckCanUserJoin(gameId, username);
        if(!canJoin)
        {
            await Clients.Client(Context.ConnectionId).JoinRefused("Рейтинг выше максимального!");
            return;
        }

        if (!_store.GameConnections.ContainsKey(game))
        {
            _store.GameConnections[game] = new HashSet<string>();
        }
        
        if (_store.GameConnections[game].Count == 2)
        {
            await Clients.Client(Context.ConnectionId).JoinRefused("В игре уже 2 игрока!");
            return;
        }
        
        var newPlayer = true;
        _store.GameConnections.AddOrUpdate(game,
            new HashSet<string>() { username },
            (_, value) =>
            {
                newPlayer = value.Add(username);
                return value;
            });

        if (!newPlayer)
            return;
        
        //Todo: command to add player name to game and then check count of players and then change status of game
        await _gameService.AddToGame(username, gameId);
        
        _store.UserGroupsConnections[Context.ConnectionId].GameGroup = game;
        await Groups.AddToGroupAsync(Context.ConnectionId, game);

        if (_store.GameConnections[game].Count == 2)
        {
            await _gameService.ChangeGameStatus(Status.Started, gameId);
            await Clients.Client(Context.ConnectionId).SuccessJoin();
            await Clients.Groups(game).StartGame();
        }

        await Clients.Client(Context.ConnectionId).SuccessJoin();

    }

    public async Task MakeMove(string gameId, Figure figure)
    {
        var username = Context.User!.Identity!.Name;
        if (!_store.UsersMove.TryGetValue(gameId, out var anotherUserMove))
        {
            _store.UsersMove.AddOrUpdate(gameId, new UserMove(username, figure),
                (_, _) => new UserMove(username, figure));
            return;
        }
        
        //Handle move
        var userMove = new UserMove(username,  figure);
        
        //Todo Command
        var gameRes = await _gameService.HandleMove(userMove, anotherUserMove, gameId);
        FinishGameDto finishDto;
        finishDto = gameRes.Winner == null ? new FinishGameDto() {WinnerFigure = figure, Message = gameRes.Message} 
            : new FinishGameDto()
            {
                WinnerName = gameRes.Winner.Username,
                WinnerFigure = gameRes.Winner.Figure,
                LoserName = gameRes.Loser.Username,
                LoserFigure = gameRes.Loser.Figure,
                Message = gameRes.Message
            };
            
        await _gameService.ChangeGameStatus(Status.Finished, gameId);
        await Clients.Group(gameId).FinishGame(finishDto);
        _store.UsersMove.Remove(gameId, out _);
    }
    
    public async Task WatchGame(string gameId)
    {
        //Добавить в зрители
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        _store.UserGroupsConnections.AddOrUpdate(Context.ConnectionId, 
            new UserGroups(){ RoomGroup = gameId },
            (_,_) => new UserGroups(){ RoomGroup = gameId });
    }

    public async Task RestartGame(string gameId)
    {
        
    }

    public async Task SendMessage(string gameId,string message)
    {
        var username = Context.User!.Identity!.Name;

        var sendMessageCommand = new SendMessageCommand()
        {
            Username = username!,
            Message = message,
            GameId = gameId
        };
        
        if(!await _mediator.Send(sendMessageCommand))
            return;
        
        var messageDto = new MessageDto(username!, message);
        await Clients.OthersInGroup(gameId).ReceiveMessage(messageDto);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var gameGroup = _store.UserGroupsConnections[Context.ConnectionId].GameGroup;
        if (gameGroup != null)
        {
            //Todo: command to delete user from game in db
            var username = Context.User?.Identity?.Name!;
            /*var command = new { Username = username, };
            var res = await _mediator.Send(command);*/

            await _gameService.DeleteFromGame(username, gameGroup.Split("_")[0]);
        }

        _store.GameConnections[gameGroup]
            .Remove(Context.ConnectionId);
        _store.UserGroupsConnections.Remove(Context.ConnectionId, out _);
    }
}

public class UserGroups
{
    public string RoomGroup { get; set; }
    public string GameGroup { get; set; }
}