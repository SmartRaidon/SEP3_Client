using ApiContracts;

namespace BlazorApp.Services;

public interface IGameService
{
    public Task StartAsync();
    public Task JoinGameAsync(int gameId);
    public Task SendMoveAsync(MoveDto move);
    public Task OnReceiveMoveAsync(Func<MoveDto, Task> callback);
    public Task OnInvalidMoveAsync(Func<string, Task> callback);
    public Task StopAsync();
}