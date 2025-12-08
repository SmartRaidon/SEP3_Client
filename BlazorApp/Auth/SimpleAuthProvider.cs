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

    public async Task DummyLogin()
    {
        UserDto userDto = new UserDto
        {
            Id = 999, 
            Username = "tester"
        };
        
        string serialisedData = JsonSerializer.Serialize(userDto);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serialisedData);
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userDto.Username), 
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString())
        }; 
        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth"); 
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        Console.WriteLine("Logged in with dummy data, username: " + userDto.Username);
    }
    
    public async Task DummyLogin2()
    {
        UserDto userDto = new UserDto
        {
            Id = 998, 
            Username = "tester2"
        };
        
        string serialisedData = JsonSerializer.Serialize(userDto);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serialisedData);
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userDto.Username), 
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString())
        }; 
        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth"); 
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        Console.WriteLine("Logged in with dummy data, username: " + userDto.Username);
    }

    public async Task Login(string email, string password)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/users/login",
            new CreateUserDto {Email = email,  Username = "", Password = password });
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
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString())
        };
        
        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", ""); 
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new()))); 
    }

    public async Task Register(string email, string username, string password)
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