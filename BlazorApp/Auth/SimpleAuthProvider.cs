using System.Net;
using System.Security.Claims;
using System.Text.Json;
using ApiContracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorApp.Auth;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    // private ClaimsPrincipal _principal; // old method, now we store the user in the browser's cache

    public SimpleAuthProvider(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }
    
    public async Task LoginAsync(string email, string password)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/users/login",
            new LoginDto() { Email = email, Password = password });
        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);    
        }
        UserDto userDto = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        
        string serialisedData = JsonSerializer.Serialize(userDto);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serialisedData);

        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userDto.Username),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
            
            // maybe we don't need these two claims
            new Claim(ClaimTypes.Email, userDto.Email),
            new Claim("Points", userDto.Points.ToString())
        };
        
        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", ""); 
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new()))); 
    }

    public async Task RegisterAsync(string email, string username, string password)
    {
        // CHANGE THE URI ACCORDING TO THE LOGIC SERVER'S USERSCONTROLLER!
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/users/register",
            new CreateUserDto { Email = email, Username = username, Password = password });
        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);    
        }
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string userAsJson = "";
        try
        {
            userAsJson = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");
        }
        catch (InvalidOperationException e)
        {
            return new AuthenticationState(new());
        }

        if (string.IsNullOrEmpty(userAsJson))
        {
            return new AuthenticationState(new());
        }
        
        UserDto? userDto = JsonSerializer.Deserialize<UserDto>(userAsJson);
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userDto.Username),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString())
            // here we can add e-mail as well
        };
        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        return new AuthenticationState(principal);
    }
}