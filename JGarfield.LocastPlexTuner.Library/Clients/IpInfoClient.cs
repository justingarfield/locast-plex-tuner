using System.Net.Http;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients
{
    public class IpInfoClient : IIpInfoClient
    {
        private readonly IHttpClientFactory _clientFactory;

        public IpInfoClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetPublicIpAddressAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.IPINFO_API_BASE_URI);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("User-Agent", Constants.DEFAULT_USER_AGENT);

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
