using ApiContracts;

namespace BlazorApp.Services;

public interface IUserService
{
   // public Task<UserDto> AddUserAsync(CreateUserDto request);
    public Task<UserDto> GetUserAsync(int id); // id or username im not sure for now
    public Task<List<UserDto>> GetUsersAsync();
    public Task<List<UserDto>> GetTop10UsersAsync();
    public Task UpdateUserAsync(int id, UserDto request);
    public Task DeleteUserAsync(int id);
    public Task ChangePasswordAsync(int id, string newPassword);
    public Task ChangeUsernameAsync(int id, string newUsername);
}