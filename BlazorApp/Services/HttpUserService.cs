using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient _httpClient;

    public HttpUserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<UserDto> GetUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await _httpClient.GetAsync($"/api/users/{id}");
        string response = httpResponse.Content.ReadAsStringAsync().Result;
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<UserDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        HttpResponseMessage httpResponse = await _httpClient.GetAsync("api/users");
        string response = httpResponse.Content.ReadAsStringAsync().Result;
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<UserDto>>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
    
    public async Task<List<UserDto>> GetTop10UsersAsync()
    {
        HttpResponseMessage httpResponse = await _httpClient.GetAsync("api/users/top10");
        string response = httpResponse.Content.ReadAsStringAsync().Result;
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        
        var _list = new List<UserDto>();
        _list = JsonSerializer.Deserialize<List<UserDto>>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
        
        Console.WriteLine("Received 10 users: " + _list.Count);
        foreach (var item in _list)
        {
            Console.WriteLine(item);
        }

        return _list;
    }
    
    public async Task UpdateUserAsync(int id, UserDto request)
    {
        HttpResponseMessage httpResponse = await _httpClient.PutAsJsonAsync($"api/users/{id}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await _httpClient.DeleteAsync($"api/users/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }
    
    public async Task ChangePasswordAsync(int id, string newPassword)
    {
        var dto = new ChangePasswordDto
        {
            NewPassword = newPassword
        };

        HttpResponseMessage httpResponse = 
            await _httpClient.PutAsJsonAsync($"/api/users/{id}/password", dto);

        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);
    }

    public async Task ChangeUsernameAsync(int id, string newUsername)
    {
        var dto = new ChangeUsernameDto
        {
            NewUsername = newUsername
        };

        HttpResponseMessage httpResponse = 
            await _httpClient.PutAsJsonAsync($"/api/users/{id}/username", dto);

        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);
    }
}