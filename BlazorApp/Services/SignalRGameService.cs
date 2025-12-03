using ApiContracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorApp.Services;

public class SignalRGameService : IGameService
{
    private readonly HubConnection _hubConnection;

    public SignalRGameService(NavigationManager navigationManager)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7174/gameHub")  // SignalR endpoint on the server
            .Build();
    }

    public async Task StartAsync()
    {
        await _hubConnection.StartAsync();
        // await Task.CompletedTask; // FOR TESTING - REMOVE ME
    }

    public async Task JoinGameAsync(int gameId)
    {
       await _hubConnection.SendAsync("JoinGameRoom", gameId);
       // await Task.CompletedTask; // FOR TESTING - REMOVE ME
    }

    public async Task SendMoveAsync(MoveDto move)
    {
        await _hubConnection.SendAsync("SendMoveAsync", move);
        // await Task.CompletedTask; // FOR TESTING - REMOVE ME
    }

    public Task OnReceiveMoveAsync(Func<MoveDto, Task> callback)
    {
        _hubConnection.On("ReceiveMove", callback);
        return Task.CompletedTask;
        // Simulate receiving a move (you can trigger the callback with a dummy MoveDTO)
        /*
        await callback(new MoveDto
        {
            GameId = 1,
            PlayerType = "X",
            CellIndex = 0
        });*/
    }

    public Task OnInvalidMoveAsync(Func<string, Task> callback)
    {
        _hubConnection.On("InvalidMove", callback);
        return Task.CompletedTask;
        
        // Simulate an invalid move response
        // await callback("Invalid move! Try again.");
    }

    public async Task StopAsync()
    {
        await _hubConnection.StopAsync();
    }
}