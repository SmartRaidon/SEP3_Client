using ApiContracts;

namespace BlazorApp.Services;

public interface IGameService
{
    Task StartAsync();

    Task<GameDTO> CreateGameAsync(int playerId, string playerName);
    Task<GameDTO> JoinGameAsync(string inviteCode, int playerId, string playerName);

    Task SendMoveAsync(int gameId, int playerId, int position);

    Task OnGameUpdated(Func<GameDTO, Task> callback);
    Task OnMoveMade(Func<MoveDTO, Task> callback);
    Task OnGameFinished(Func<GameDTO, Task> callback);

    Task StopAsync();
}
