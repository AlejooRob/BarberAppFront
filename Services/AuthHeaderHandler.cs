using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using BarberAppFront.Utils;

namespace BarberAppFront.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await SecureStorageHelper.GetTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                if (request.Headers.Contains("Authorization"))
                {
                    request.Headers.Remove("Authorization");
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                System.Diagnostics.Debug.WriteLine($"[DEBUG-AuthHeader] Header final: {request.Headers.Authorization?.ToString()}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}