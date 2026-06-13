using DETechOne.SmartWMS.Web.Authentication;
using DETechOne.SmartWMS.Web.Components;
using DETechOne.SmartWMS.Web.Services.Api;
using DETechOne.SmartWMS.Web.Services.Auth;
using DETechOne.SmartWMS.Web.Services.State;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
    });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthTokenStore, BrowserAuthTokenStore>();
builder.Services.AddScoped<AuthTokenMessageHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, SmartWmsAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISmartWmsApiClient, SmartWmsApiClient>();
builder.Services.AddScoped<IUiStateService, UiStateService>();

var apiBaseUrl = builder.Configuration.GetValue<string>("SmartWmsApi:BaseUrl") ?? "https://localhost:7001/";

builder.Services.AddHttpClient("SmartWmsApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthTokenMessageHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
