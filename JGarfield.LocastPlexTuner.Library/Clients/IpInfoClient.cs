using JGarfield.LocastPlexTuner.Library.Clients.Contracts;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients
{
    public class IpInfoClient : IIpInfoClient
    {
        private static readonly Uri IPINFO_API_BASE_URI = new Uri("https://ipinfo.io/ip");

        private readonly IHttpClientFactory _clientFactory;

        public IpInfoClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetPublicIpAddressAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, IPINFO_API_BASE_URI);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("User-Agent", Constants.DEFAULT_HTTPCLIENT_USERAGENT);

            using var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }

    }
}
