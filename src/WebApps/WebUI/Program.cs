using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebUI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var aaaaa = builder.Configuration["ApiSettings:IdentityAPIAddress"]!;
builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "https://localhost:10000"; //"builder.Configuration["ApiSettings:IdentityAPIAddress"]!;
    options.ProviderOptions.ClientId = "client_blazorui"; // builder.Configuration["ApiSettings:ClientId"]!;

    options.ProviderOptions.DefaultScopes.Clear();
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("email");
    options.ProviderOptions.DefaultScopes.Add("address");
    options.ProviderOptions.DefaultScopes.Add("moviesapi");
    //options.ProviderOptions.DefaultScopes.Add(builder.Configuration["ApiSettings:Scope1"]!);
    //options.ProviderOptions.DefaultScopes.Add(builder.Configuration["ApiSettings:Scope2"]!);

    options.ProviderOptions.ResponseType = "code";
    //options.UserOptions.NameClaim = "sub";
});

await builder.Build().RunAsync();
