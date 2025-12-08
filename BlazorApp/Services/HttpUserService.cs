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
    
    /*public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("api/users/register", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<UserDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }*/

    public async Task<UserDto> GetUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await _httpClient.GetAsync($"users/{id}");
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
        HttpResponseMessage httpResponse = await _httpClient.GetAsync("users");
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

    public async Task UpdateUserAsync(int id, UserDto request)
    {
        HttpResponseMessage httpResponse = await _httpClient.PutAsJsonAsync($"users/{id}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await _httpClient.DeleteAsync($"users/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }
}