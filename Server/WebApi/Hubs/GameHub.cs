using Contracts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApi.Features.ChatHistory.Commands;
using WebApi.Features.Game.Commands.ChangeGameStatus;
using WebApi.Features.Game.Commands.HandleMoves;
using WebApi.Features.Game.Commands.JoinGame;
using WebApi.Features.Game.Queries.CheckUserRating;

namespace WebApi.Hubs;

[Authorize]
public class GameHub : Hub<IGameHubClient>
{
    private readonly IMediator _mediator;
    private readonly Store _store;
    
    public GameHub(IMediator mediator, Store store)
    {
        _mediator = mediator;
        _store = store;
    }

    public async Task JoinGame(string gameId)
    {
        var username = Context.User!.Identity!.Name;
        var game = $"{gameId}_game";
        
        var checkUserRatingQuery = new CheckUserRatingQuery(){GameId = gameId, Username = username};
        var canJoin = await _mediator.Send(checkUserRatingQuery);
        if(!canJoin)
        {
            await Clients.Client(Context.ConnectionId).JoinRefused("Рейтинг выше максимального!");
            return;
        }

        if (!_store.GameConnections.ContainsKey(game))
        {
            _store.GameConnections[game] = new HashSet<string>();
        }
        
        if (_store.GameConnections.ContainsKey(game) && _store.GameConnections[game].Count == 2)
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
        
        var userJoinCommand = new JoinGameCommand(){GameId = gameId, Username = username};
        await _mediator.Send(userJoinCommand);
        
        _store.UserGroupsConnections[Context.ConnectionId].GameGroup = game;
        await Groups.AddToGroupAsync(Context.ConnectionId, game);

        if (_store.GameConnections[game].Count == 2)
        {
            var changeGameStatusCommand = new ChangeGameStatusCommand(){GameId = gameId, Status = Status.Started};
            await _mediator.Send(changeGameStatusCommand);          
            await Clients.Client(Context.ConnectionId).SuccessJoin();

            await Clients.Groups(game).StartGame();

            await Task.Delay(TimeSpan.FromSeconds(10));
            
            await Clients.Groups(game).AskFigure();
            return;
        }
        await Clients.Client(Context.ConnectionId).SuccessJoin();

    }

    public async Task MakeMove(string gameId, Figure figure)
    {
        var username = Context.User!.Identity!.Name;
        _store.UsersMove.AddOrUpdate(gameId, new List<UserMove>()
            {
                new (username, figure)
            },
            (_, list) =>
            {
                list.Add(new UserMove(username, figure));
                return list;
            });
        
        if(_store.UsersMove[gameId].Count != 2)
            return;
        
        
        var userMove = new UserMove(username,  figure);
        var anotherUserMove = _store.UsersMove[gameId].First(x => x.Username != username);
        var handleMoveCommand = new MovesCommand()
        {
            GameId = gameId,
            UserMove1 = userMove,
            UserMove2 = anotherUserMove
        };
        var gameResult = await _mediator.Send(handleMoveCommand);
        
        FinishGameDto finishDto;
        finishDto = gameResult.Winner == null ? new FinishGameDto() {WinnerFigure = figure, Message = gameResult.Message} 
            : new FinishGameDto()
            {
                WinnerName = gameResult.Winner.Username,
                WinnerFigure = gameResult.Winner.Figure,
                LoserName = gameResult.Loser.Username,
                LoserFigure = gameResult.Loser.Figure,
                Message = gameResult.Message
            };
            
        var changeGameStatusCommand = new ChangeGameStatusCommand(){GameId = gameId, Status = Status.Finished};
        await _mediator.Send(changeGameStatusCommand);
        var game = $"{gameId}_game";
        await Clients.Group(game).FinishGame(finishDto);
        await Clients.Group(gameId).ReceiveMessage(new MessageDto("Server", gameResult.Message));
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
        await Clients.Group(gameId).ReceiveMessage(messageDto);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var gameGroup = _store.UserGroupsConnections[Context.ConnectionId].GameGroup;
        if (gameGroup != null)
        {
            var username = Context.User?.Identity?.Name!;
            var userJoinCommand = new JoinGameCommand()
            {
                GameId = gameGroup.Split("_")[0],
                Username = username
            };
            await _mediator.Send(userJoinCommand);
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