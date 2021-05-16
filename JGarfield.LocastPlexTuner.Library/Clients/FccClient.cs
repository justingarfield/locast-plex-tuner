using JGarfield.LocastPlexTuner.Library.Clients.Contracts;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients
{
    public class FccClient : IFccClient
    {
        private readonly IHttpClientFactory _clientFactory;

        public FccClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<Stream> GetLmsFacilityDbAsync(Uri uri)
        {
            using var client = _clientFactory.CreateClient();

            var response = await client.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            // TODO: Uhhhh, is this safe? Caller will have to Close stream.
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
