using ApiContracts;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorApp.Services;

public class SignalRGameService : IGameService
{
    private readonly HubConnection _hubConnection;

    public SignalRGameService()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7174/gamehub") // LOGIC SERVER HUB HTTPS
            .WithAutomaticReconnect()
            .Build();
    }


    public async Task StartAsync()
    {
        if (_hubConnection == null)
            throw new InvalidOperationException("HubConnection is not initialized.");

        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            Console.WriteLine("[Client] Starting SignalR connection...");
            await _hubConnection.StartAsync();
            Console.WriteLine("[Client] SignalR connected. State=" + _hubConnection.State);
        }
        else
        {
            Console.WriteLine("[Client] StartAsync called but connection is in state: " + _hubConnection.State);

        }
    }
    public Task<GameDto> CreateGameAsync(int playerId, string playerName)
    {
        Console.WriteLine($"[Client] CreateGameAsync({playerId},  {playerName})");
        return _hubConnection.InvokeAsync<GameDto>("CreateGame", playerId, playerName);
    }

    public Task<GameDto> JoinGameAsync(string inviteCode, int playerId, string playerName)
    {
        Console.WriteLine($"[Client] JoinGameAsync({inviteCode}, {playerId}, {playerName})");
        return _hubConnection.InvokeAsync<GameDto>("JoinGame", inviteCode, playerId, playerName);
    }

    public Task SendMoveAsync(int gameId, int playerId, int position)
    {
        Console.WriteLine($"[Client] SendMoveAsync(game={gameId}, player={playerId}, pos={position})");
        return _hubConnection.SendAsync("MakeMove", gameId, playerId, position);
    }

    public Task OnGameUpdated(Func<GameDto, Task> callback)
    {
        _hubConnection.On<GameDto>("GameUpdated", game =>
        {
            Console.WriteLine("[Client] GameUpdated event received");
            return callback(game);
        });
        return Task.CompletedTask;
    }

    public Task OnMoveMade(Func<MoveDto, Task> callback)
    {
        _hubConnection.On<MoveDto>("MoveMade", move =>
        {
            Console.WriteLine("[Client] MoveMade event received");
            return callback(move);
        });
        return Task.CompletedTask;
    }

    public Task OnGameFinished(Func<GameDto, Task> callback)
    {
        _hubConnection.On<GameDto>("GameFinished", game =>
        {
            Console.WriteLine("[Client] GameFinished event received");
            return callback(game);
        });
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        Console.WriteLine("[Client] Stopping SignalR connection...");
        return _hubConnection.StopAsync();
    }

    public Task RequestReplayAsync(int gameId, int playerId)
    {
        Console.WriteLine($"[Client] RequestReplayAsync(game={gameId}, player={playerId})");
        return _hubConnection.SendAsync("RequestReplay", gameId, playerId);
    }
    
    public Task OnReplayStarted(Func<GameDto, Task> callback)
    {
        _hubConnection.On<GameDto>("ReplayStarted", game =>
        {
            Console.WriteLine("[Client] ReplayStarted event received");
            return callback(game);
        });
        return Task.CompletedTask;
    }
    
    public async Task<GameDto> CheckTimeoutAsync(int gameId)
    {
        return await _hubConnection.InvokeAsync<GameDto>("CheckTimeout", gameId);
    }

}
