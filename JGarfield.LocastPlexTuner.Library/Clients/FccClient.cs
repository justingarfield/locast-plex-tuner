using JGarfield.LocastPlexTuner.Library.Clients.Contracts;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients
{
    /// <summary>
    /// Defines a contract for a class that implements capabilities for accessing FCC Licensing and Management System (LMS) Facilities Database related information.
    /// <br /><br />
    /// For more information, see: <see href="https://enterpriseefiling.fcc.gov/dataentry/public/tv/lmsDatabase.html" />
    /// </summary>
    public class FccClient : IFccClient
    {
        #region Private Members

        /// <summary>
        /// The <see cref="IHttpClientFactory"/> implementation used to create new <see cref="HttpClient"/> instances.
        /// </summary>
        private readonly IHttpClientFactory _clientFactory;

        #endregion Private Members

        #region Constructor

        /// <summary>
        /// Instantiates a new <see cref="FccClient"/> using the provided <see cref="IHttpClientFactory"/> implementation.
        /// </summary>
        /// <param name="clientFactory">The <see cref="IHttpClientFactory"/> implementation used to create new <see cref="HttpClient"/> instances.</param>
        public FccClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<Stream> GetLmsFacilityDbAsync(Uri uri)
        {
            using var client = _clientFactory.CreateClient();

            var response = await client.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            // TODO: Uhhhh, is this safe? Caller will have to Close stream.
            return await response.Content.ReadAsStreamAsync();
        }

        #endregion Public Methods
    }
}
