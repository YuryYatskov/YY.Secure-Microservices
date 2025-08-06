using Duende.IdentityServer.Models;

namespace IdentityServer.Configurations;

public static class Config
{
    public const string Admin = "admin";
    public const string Customer = "customer";

    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
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
            }
        },
    ];
}

