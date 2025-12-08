using ApiContracts;

namespace BlazorApp.Services;

public interface IGameService
{
    Task StartAsync();

    Task<GameDto> CreateGameAsync(int playerId, string playerName);
    Task<GameDto> JoinGameAsync(string inviteCode, int playerId, string playerName);

    Task SendMoveAsync(int gameId, int playerId, int position);

    Task OnGameUpdated(Func<GameDto, Task> callback);
    Task OnMoveMade(Func<MoveDto, Task> callback);
    Task OnGameFinished(Func<GameDto, Task> callback);

    Task StopAsync();
    Task RequestReplayAsync(int gameId, int playerId);
    Task OnReplayStarted(Func<GameDto, Task> callback);
}
