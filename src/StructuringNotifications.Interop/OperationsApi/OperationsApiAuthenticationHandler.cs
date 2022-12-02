using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace StructuringNotifications.Interop.OperationsApi;

internal class OperationsApiAuthenticationHandler : DelegatingHandler
{
    private readonly IOperationsApiConfiguration _configDto;
    private string? _token;

    public OperationsApiAuthenticationHandler(IOperationsApiConfiguration configDto)
    {
        _configDto = configDto;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _token ??= await GetToken().ConfigureAwait(false);

        request.Headers.Add("Authorization", token);
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            token = _token = await GetToken().ConfigureAwait(false);

            request.Headers.Remove("Authorization");
            request.Headers.Add("Authorization", token);
            response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }

    private async Task<string> GetToken()
    {
        var clientCredential =
            new ClientCredential(_configDto.OperationsApiAdClientId, _configDto.OperationsApiAdClientSecret);
        var context =
            new AuthenticationContext($"{_configDto.OperationsApiAdInstance}/{_configDto.OperationsApiAdTenantId}",
                false);
        #pragma warning disable CS0618
        var token = await context.AcquireTokenAsync(_configDto.OperationsApiAdClientId, clientCredential);
        #pragma warning restore CS0618
        return token.CreateAuthorizationHeader();
    }
}