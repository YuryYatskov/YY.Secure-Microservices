using Duende.IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;

namespace WebUI.Handlers;

public class AuthenticationDelegatingHandler(
    IHttpClientFactory _httpClientFactory,
    IMemoryCache _cache) : DelegatingHandler
{
    private const string _token = "Token";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string token = await AuthorizationAsync(cancellationToken);
        
        request.SetBearerToken(token);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> AuthorizationAsync(CancellationToken cancellationToken)
    {
        string token = RetrieveCachedToken();
        if (string.IsNullOrWhiteSpace(token))
            token = await GetToken(cancellationToken);

        return token;
    }

    private async Task<string> GetToken(CancellationToken cancellationToken)
    {
        var apiClientCredentials = new ClientCredentialsTokenRequest
        {
            Address = "https://localhost:10000/connect/token", // TODO: ClientCredentialsTokenRequest
            ClientId = "client_moviesapi",
            ClientSecret = "client_secrets",
            Scope = "moviesapi"
        };

        var httpClient = _httpClientFactory.CreateClient("IDPClient");

        TokenResponse tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(apiClientCredentials, cancellationToken: cancellationToken);
        if (tokenResponse.IsError)
        {
            throw new HttpRequestException("Something went wrong while requesting the access token");
        }

        return SetCacheToken(tokenResponse);
    }

    private string RetrieveCachedToken() => _cache.Get<string>(_token)!;

    private string SetCacheToken(TokenResponse tokenResponse)
    { 
        var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenResponse.ExpiresIn));
        _cache.Set(_token, tokenResponse.AccessToken, options);
        return _cache.Get<string>(_token)!;
    }
}
