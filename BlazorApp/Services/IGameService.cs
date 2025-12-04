using ApiContracts;

namespace BlazorApp.Services;

public interface IGameService
{
    Task StartAsync();

    Task<GameDTO> CreateGameAsync(int playerId);
    Task<GameDTO> JoinGameAsync(string inviteCode, int playerId);

    Task SendMoveAsync(int gameId, int playerId, int position);

    Task OnGameUpdated(Func<GameDTO, Task> callback);
    Task OnMoveMade(Func<MoveDTO, Task> callback);
    Task OnGameFinished(Func<GameDTO, Task> callback);

    Task StopAsync();
}
