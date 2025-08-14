using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;
using WebUI;
using WebUI.Handlers;
using WebUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTransient<AuthenticationDelegatingHandler>();

builder.Services.AddRefitClient<IMovieService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("https://localhost:11000"); // builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services.AddHttpClient("IDPClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:10000/"); // builder.Configuration["ApiSettings:ISAddress"]!);
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add("Accept", "application/json"); //  HeaderNames.Accept
});

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "https://localhost:10000"; // builder.Configuration["ApiSettings:ISAddress"]!);
    options.ProviderOptions.ClientId = "client_blazorui";

    options.ProviderOptions.DefaultScopes.Clear();
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("email");
    options.ProviderOptions.DefaultScopes.Add("address");
    options.ProviderOptions.DefaultScopes.Add("moviesapi");

    options.ProviderOptions.ResponseType = "code";
});

builder.Services.AddMemoryCache();

await builder.Build().RunAsync();
