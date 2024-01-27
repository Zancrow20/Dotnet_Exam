using Contracts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApi.Features.ChatHistory.Commands;
using WebApi.Features.Game.Commands.ChangeGameStatus;
using WebApi.Features.Game.Commands.HandleMoves;
using WebApi.Features.Game.Commands.JoinGame;
using WebApi.Features.Game.Commands.LeaveGame;
using WebApi.Features.Game.Queries.CheckUserRating;
using WebApi.Features.Game.Queries.GetGame;

namespace WebApi.Hubs;

[Authorize]
public class GameHub : Hub<IGameHubClient>
{
    private readonly IMediator _mediator;
    private readonly Store _store;
    private static readonly object _locker = new();
    private readonly ILogger<GameHub> _logger;
    
    public GameHub(IMediator mediator, Store store, ILogger<GameHub> logger)
    {
        _mediator = mediator;
        _store = store;
        _logger = logger;
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
        
        var query = new GameQuery() {GameId = gameId};
        var gameRes = await _mediator.Send(query);
        var gameStatus = gameRes.Match(g => g.Status, _ => Status.Started);

        if(gameStatus == Status.Finished)
        {
            await Clients.Client(Context.ConnectionId).JoinRefused("Игра уже завершена!");
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

            await Clients.Group(game).StartGame();

            await Task.Delay(TimeSpan.FromSeconds(10));
            
            await Clients.Group(game).AskFigure();
            return;
        }
        await Clients.Client(Context.ConnectionId).SuccessJoin();

    }

    public async Task MakeMove(string gameId, Figure figure)
    {
        var username = Context.User!.Identity!.Name;
        
        lock(_locker)
            _store.UsersMove.AddOrUpdate(gameId, new HashSet<UserMove>()
                {
                    new (username, figure)
                },
                (_, set) =>
                {
                    set.Add(new UserMove(username, figure));
                    return set;
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
        
        _logger.LogInformation("Message: {GameMessage}", gameResult.Message);
        
        FinishGameDto finishDto = new()
        {
            WinnerName = gameResult.Winner.Username,
            WinnerFigure = gameResult.Winner.Figure,
            LoserName = gameResult.Loser.Username,
            LoserFigure = gameResult.Loser.Figure,
            Message = gameResult.Message,
            IsDraw = gameResult.IsDraw
        };
            
        var changeGameStatusCommand = new ChangeGameStatusCommand(){GameId = gameId, Status = Status.Finished};
        await _mediator.Send(changeGameStatusCommand);
        var game = $"{gameId}_game";
        await Clients.Group(gameId).ReceiveMessage(new MessageDto("Server", gameResult.Message));
        await Clients.Group(game).FinishGame(finishDto);
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
        var query = new GameQuery() {GameId = gameId};
        var gameRes = await _mediator.Send(query);
        var gameStatus = gameRes.Match(g => g.Status, _ => Status.Started);

        var game = $"{gameId}_game";
        if(gameStatus == Status.New || _store.GameConnections[game].Count() != 2)
            return;
        
        var changeGameStatusCommand = new ChangeGameStatusCommand(){GameId = gameId, Status = Status.Started};
        await _mediator.Send(changeGameStatusCommand);
        
        await Clients.Client(Context.ConnectionId).StartGame();

        await Task.Delay(TimeSpan.FromSeconds(10));
            
        await Clients.Client(Context.ConnectionId).AskFigure();
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
            var gameId = gameGroup.Split("_")[0];
            var userLeaveCommand = new LeaveGameCommand()
            {
                GameId = gameId,
                Username = username
            };
            await _mediator.Send(userLeaveCommand);
            
            var changeGameStatusCommand = new ChangeGameStatusCommand(){GameId = gameId, Status = Status.New};
            await _mediator.Send(changeGameStatusCommand);
            _store.GameConnections[gameGroup].Remove(username);
        }

        _store.UserGroupsConnections.Remove(Context.ConnectionId, out _);
    }
}

public class UserGroups
{
    public string RoomGroup { get; set; }
    public string GameGroup { get; set; }
}