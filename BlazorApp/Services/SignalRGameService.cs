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


    public Task StartAsync()
    {
        Console.WriteLine("[Client] Starting SignalR connection...");
        return _hubConnection.StartAsync();
    }

    public Task<GameDTO> CreateGameAsync(int playerId)
    {
        Console.WriteLine($"[Client] CreateGameAsync({playerId})");
        return _hubConnection.InvokeAsync<GameDTO>("CreateGame", playerId);
    }

    public Task<GameDTO> JoinGameAsync(string inviteCode, int playerId)
    {
        Console.WriteLine($"[Client] JoinGameAsync({inviteCode}, {playerId})");
        return _hubConnection.InvokeAsync<GameDTO>("JoinGame", inviteCode, playerId);
    }

    public Task SendMoveAsync(int gameId, int playerId, int position)
    {
        Console.WriteLine($"[Client] SendMoveAsync(game={gameId}, player={playerId}, pos={position})");
        return _hubConnection.SendAsync("MakeMove", gameId, playerId, position);
    }

    public Task OnGameUpdated(Func<GameDTO, Task> callback)
    {
        _hubConnection.On<GameDTO>("GameUpdated", game =>
        {
            Console.WriteLine("[Client] GameUpdated event received");
            return callback(game);
        });
        return Task.CompletedTask;
    }

    public Task OnMoveMade(Func<MoveDTO, Task> callback)
    {
        _hubConnection.On<MoveDTO>("MoveMade", move =>
        {
            Console.WriteLine("[Client] MoveMade event received");
            return callback(move);
        });
        return Task.CompletedTask;
    }

    public Task OnGameFinished(Func<GameDTO, Task> callback)
    {
        _hubConnection.On<GameDTO>("GameFinished", game =>
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
}
