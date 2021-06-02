using JGarfield.LocastPlexTuner.Library.Clients.Contracts;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TimeZoneConverter;

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

        private static readonly Uri FCC_LMS_BASE_URI = new Uri("https://enterpriseefiling.fcc.gov/dataentry/api/");

        private const string FCC_LMS_FACILITY_DB_DATEFORMAT = "MM-dd-yyyy";

        private const string FCC_LMS_FACILITY_DB_FILENAME = "facility.zip";

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
        /// <param name="relativeUri"></param>
        /// <returns></returns>
        public async Task FetchLmsFacilityDbAsync(FileStream fileStream)
        {
            using var client = _clientFactory.CreateClient();

            var fccLmsDatabaseUri = GetLmsFacilityDbUri();

            var response = await client.GetAsync(fccLmsDatabaseUri);

            response.EnsureSuccessStatusCode();

            await response.Content.CopyToAsync(fileStream);
        }

        #endregion Public Methods

        /// <summary>
        /// Generates a new <see cref="Uri"/> representing the absolute location to the most current FCC LMS Database file.
        /// </summary>
        /// <returns>A new <see cref="Uri"/> representing the absolute location to the most current FCC LMS Database file.</returns>
        private Uri GetLmsFacilityDbUri()
        {
            // FCC operates with file modified timestamps and maintenance windows based in EST
            var estTimeZone = TZConvert.GetTimeZoneInfo("Eastern Standard Time"); // Need to use TZConvert here since Linux distro Timezones differ using IANA
            var estDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, estTimeZone);
            var fccDateTimeFormat = estDateTime.ToString(FCC_LMS_FACILITY_DB_DATEFORMAT);

            // The FCC LMS Facilities URI conforms to the following pattern: https://enterpriseefiling.fcc.gov/dataentry/api/download/dbfile/05-09-2021/facility.zip
            return new Uri(FCC_LMS_BASE_URI, $"download/dbfile/{fccDateTimeFormat}/{FCC_LMS_FACILITY_DB_FILENAME}");
        }
    }
}
