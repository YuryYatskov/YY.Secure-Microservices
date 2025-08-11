using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer.Configurations;

public static class Config
{
    public const string Admin = "admin";
    public const string Customer = "customer";

    public static WebApplicationBuilder? Builder { get; set; }

    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email(),
        new IdentityResources.Address(),

    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
        new ApiResource("moviesapi", "Movies API"),
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("moviesapi", "Movies API"),
    ];

    public static IEnumerable<Client> Clients =>
    [
        new()
        {
            ClientId = "client_moviesapi",
            ClientName = "Movies API",
            ClientSecrets = { new Secret("client_secrets".Sha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes =
            {
                "moviesapi",
            },
            AllowedCorsOrigins = { Builder?.Configuration["ApiSettings:MoviesAPIAddress"]! }
        },
        new()
        {
            ClientId = "client_blazorui",
            ClientName = "Blazor UI",
            RequireClientSecret = false,
            AllowedGrantTypes = GrantTypes.Code,
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Address,
                IdentityServerConstants.StandardScopes.Email,
                "moviesapi",
            },
            RequireConsent = false,
            RequirePkce = true,
            RedirectUris = { $"{Builder?.Configuration["ApiSettings:WebUIAddress"]!}/authentication/login-callback" },
            PostLogoutRedirectUris = { $"{Builder?.Configuration["ApiSettings:WebUIAddress"]!}/authentication/logout-callback" },
            AllowedCorsOrigins = { Builder?.Configuration["ApiSettings:WebUIAddress"]! },

        }
    ];
}

