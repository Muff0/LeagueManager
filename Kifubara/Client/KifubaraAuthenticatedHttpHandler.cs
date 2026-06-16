using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace Kifubara.Client;

public class KifubaraAuthenticatedHttpHandler(IOptions<KifubaraSettings> settings) : DelegatingHandler
{
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.Value.ApiKey);
            return await base.SendAsync(request, cancellationToken);
        }

}