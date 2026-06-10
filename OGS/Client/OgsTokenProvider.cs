using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Settings;

namespace OGS.Client;

public interface IOgsTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken ct = default);
}

/// <summary>
/// Obtains an OGS access token via the OAuth2 client-credentials grant and caches it
/// until shortly before it expires. Registered as a singleton so the token is shared
/// across requests.
/// </summary>
public class OgsTokenProvider : IOgsTokenProvider
{
    // OGS uses django-oauth-toolkit; the client-credentials token endpoint lives here.
    private const string TokenEndpoint = "https://online-go.com/oauth2/token/";

    private readonly IHttpClientFactory _httpFactory;
    private readonly OgsSettings _settings;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private string? _accessToken;
    private DateTimeOffset _expiresAt;

    public OgsTokenProvider(IHttpClientFactory httpFactory, IOptions<OgsSettings> settings)
    {
        _httpFactory = httpFactory;
        _settings = settings.Value;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken ct = default)
    {
        if (_accessToken is not null && DateTimeOffset.UtcNow < _expiresAt)
            return _accessToken;

        await _lock.WaitAsync(ct);
        try
        {
            // Re-check after acquiring the lock: another caller may have refreshed it.
            if (_accessToken is not null && DateTimeOffset.UtcNow < _expiresAt)
                return _accessToken;

            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret
            });

            var http = _httpFactory.CreateClient();
            var response = await http.PostAsync(TokenEndpoint, form, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var token = JsonConvert.DeserializeObject<TokenResponse>(json)!;

            _accessToken = token.AccessToken;
            // Refresh a minute early to avoid using a token that expires mid-request.
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(Math.Max(0, token.ExpiresIn - 60));
            return _accessToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    private class TokenResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; } = null!;
        [JsonProperty("token_type")] public string TokenType { get; set; } = null!;
        [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
        [JsonProperty("scope")] public string? Scope { get; set; }
    }
}

/// <summary>
/// Attaches the OGS bearer token to every outgoing request on clients it is wired into.
/// </summary>
public class OgsAuthenticatedHttpHandler : DelegatingHandler
{
    private readonly IOgsTokenProvider _tokenProvider;

    public OgsAuthenticatedHttpHandler(IOgsTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
