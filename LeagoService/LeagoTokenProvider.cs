using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Duende.IdentityModel.OidcClient;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace LeagoService
{
    public class LeagoAuthenticatedHttpHandler : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;

        public LeagoAuthenticatedHttpHandler(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenProvider.GetAccessTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
    public class LeagoTokenProvider : ITokenProvider
    {

        private readonly OidcClient _oidcClient;
        private readonly IOptions<LeagoSettings> _leagoOptions;
        private string? _accessToken;
        private DateTimeOffset _expiresAt;
        private OidcClientOptions _clientOptions;
        private readonly string _authority = "https://id.leago.gg/";

        public LeagoTokenProvider(IOptions<LeagoSettings> leagoOptions)
        {
            _leagoOptions = leagoOptions;

            var browser = new SystemBrowser(63136);
            string redirectUri = string.Format($"http://127.0.0.1:{browser.Port}");

            _clientOptions = new OidcClientOptions
            {
                Authority = _authority,
                ClientId = "leago.public",
                // ClientId = "interactive.public",
                RedirectUri = redirectUri,
                // Scope = "openid profile api",
                Scope = "openid profile Leago.WebAPI",
                FilterClaims = false,
                Browser = browser,
            };

            _oidcClient = new OidcClient(_clientOptions);
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiresAt)
            {
                return _accessToken!;
            }

            var loginResult = await _oidcClient.LoginAsync();

            _accessToken = loginResult.AccessToken;
            _expiresAt = loginResult.AccessTokenExpiration;

            return _accessToken!;
        }
    }

    public interface ITokenProvider
    {
        Task<string> GetAccessTokenAsync();
    }
}
