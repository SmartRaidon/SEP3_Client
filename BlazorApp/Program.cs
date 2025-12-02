using BlazorApp.Auth;
using BlazorApp.Components;
using BlazorApp.Services;
using Microsoft.AspNetCore.Components.Authorization;

// setting the developer environment manually
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environments.Development
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001") });
builder.Services.AddScoped<IUserService, HttpUserService>();
builder.Services.AddScoped<IGameService, SignalRGameService>();
builder.Services.AddScoped<AuthenticationStateProvider, SimpleAuthProvider>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();