using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorGame.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using BlazorGame.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "http://localhost:8080/realms/BlazorGame";
    options.ProviderOptions.ClientId = "blazorgame-client";
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
});

builder.Services.AddSingleton<AuthService>();

await builder.Build().RunAsync();