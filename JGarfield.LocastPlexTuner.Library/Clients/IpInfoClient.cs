using JGarfield.LocastPlexTuner.Library.Clients.Contracts;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients
{
    /// <summary>
    /// 
    /// </summary>
    public class IpInfoClient : IIpInfoClient
    {
        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        private static readonly Uri IPINFO_API_BASE_URI = new Uri("https://ipinfo.io/ip");

        /// <summary>
        /// 
        /// </summary>
        private readonly HttpClient _httpClient;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        public IpInfoClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPublicIpAddressAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, IPINFO_API_BASE_URI);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("User-Agent", Constants.DEFAULT_HTTPCLIENT_USERAGENT);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }

        #endregion Public Methods
    }
}
