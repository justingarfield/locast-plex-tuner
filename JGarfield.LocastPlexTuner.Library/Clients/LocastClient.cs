using JGarfield.LocastPlexTuner.Library.Clients.Contracts;
using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Dma;
using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Epg;
using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Station;
using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.User;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients
{
    /// <summary>
    /// Client used to make calls to the Locast API endpoints.
    /// </summary>
    public class LocastClient : ILocastClient
    {
        #region Private Members

        /// <summary>
        /// The Base URI of the Locast API Root.
        /// </summary>
        private static readonly Uri LOCAST_API_BASE_URI = new Uri("https://api.locastnet.org/api/");

        /// <summary>
        /// A <see cref="HttpClient"/> instance provided via DI.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// An <see cref="IConfiguration"/> instance provided via DI.
        /// </summary>
        private readonly IConfiguration _configuration;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Instantiates a new <see cref="LocastClient"/> using the provided <see cref="HttpClient"/> and <see cref="IConfiguration"/> instances.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for calls to the Locast API.</param>
        /// <param name="configuration">An <see cref="IConfiguration"/> instance used to retrieve application configuration values.</param>
        public LocastClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _httpClient.BaseAddress = LOCAST_API_BASE_URI;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Performs a lookup for a specific Designated Market Area (DMA).
        /// </summary>
        /// <param name="dma">The unique identifier of the DMA to lookup.</param>
        /// <returns>A DTO representing a DMA (if one was found).</returns>
        public async Task<LocastDmaLocationDto> GetDmaAsync(string dma)
        {
            return await _httpClient.GetFromJsonAsync<LocastDmaLocationDto>($"watch/dma/{dma}");
        }

        /// <summary>
        /// Performs a lookup for a Designated Market Area (DMA) by Zip Code.
        /// </summary>
        /// <param name="zipCode">The Zip Code to use when performing the DMA lookup.</param>
        /// <returns>A DTO representing a DMA (if one was found).</returns>
        public async Task<LocastDmaLocationDto> GetDmaByZipCodeAsync(string zipCode)
        {
            return await _httpClient.GetFromJsonAsync<LocastDmaLocationDto>($"watch/dma/zip/{zipCode}");
        }

        /// <summary>
        /// Performs a lookup for a Designated Market Area (DMA) by IP Address.
        /// </summary>
        /// <param name="ipAddress">The IP Address to use when performing the DMA lookup.</param>
        /// <returns>A DTO representing a DMA (if one was found).</returns>
        public async Task<LocastDmaLocationDto> GetDmaByIpAddressAsync(string ipAddress)
        {
            _httpClient.DefaultRequestHeaders.Add("client_ip", ipAddress);
            
            return await _httpClient.GetFromJsonAsync<LocastDmaLocationDto>($"watch/dma/ip");
        }

        /// <summary>
        /// Performs a lookup for a Designated Market Area (DMA) by Longitude and Latitude.
        /// </summary>
        /// <param name="latitude">The Latitude to use when performing the DMA lookup.</param>
        /// <param name="longitude">The Longitude to use when performing the DMA lookup.</param>
        /// <returns>A DTO representing a DMA (if one was found).</returns>
        public async Task<LocastDmaLocationDto> GetDmaByLatLongAsync(double latitude, double longitude)
        {
            return await _httpClient.GetFromJsonAsync<LocastDmaLocationDto>($"watch/dma/{latitude}/{longitude}");
        }

        /// <summary>
        /// Retrieves the User Details for the currently logged-in User (whichever account was provided in the configuration steps).
        /// <br /><br />
        /// Note: This is currently used during startup to ensure that the User is currently donating, to avoid headaches with Locast advertising.
        /// </summary>
        /// <returns>A DTO representing the User Details for the currently logged-in User.</returns>
        public async Task<LocastUserDetailsDto> GetUserDetails()
        {
            await PerformLoginIfNeededAsync();

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.GetFromJsonAsync<LocastUserDetailsDto>($"user/me");
        }

        /// <summary>
        /// Retrieves the Electronic Programming Guide (EPG) channels and listings for a particular Designated Market Area (DMA).
        /// </summary>
        /// <param name="dma">The DMA to retrieve the EPG for.</param>
        /// <param name="startTime">Where in the timeline of the EPG data to start pulling listings.</param>
        /// <param name="hours">How many hours of listings to pull (only used if a startTime is provided).</param>
        /// <returns>EPG channels and listings for a particular Designated Market Area (DMA)</returns>
        public async Task<List<LocastChannelDto>> GetEpgForDmaAsync(string dma, DateTimeOffset? startTime = null, int? hours = 0)
        {
            await PerformLoginIfNeededAsync();

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (startTime.HasValue)
            {
                var isoStartTime = startTime.Value.DateTime.ToString("yyyy-MM-ddTHH:mm:sszzz");

                if (hours.HasValue)
                {
                    return await _httpClient.GetFromJsonAsync<List<LocastChannelDto>>($"watch/epg/{dma}?startTime={isoStartTime}&hours={hours}");
                }
                else
                {
                    return await _httpClient.GetFromJsonAsync<List<LocastChannelDto>>($"watch/epg/{dma}?startTime={isoStartTime}");
                }
            }
            else
            {
                return await _httpClient.GetFromJsonAsync<List<LocastChannelDto>>($"watch/epg/{dma}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public async Task<LocastStationDto> GetStationAsync(long stationId, double latitude, double longitude)
        {
            await PerformLoginIfNeededAsync();

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.GetFromJsonAsync<LocastStationDto>($"watch/station/{stationId}/{latitude}/{longitude}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<string> GetM3U8FileAsString(Uri uri)
        {
            await PerformLoginIfNeededAsync();

            return await _httpClient.GetStringAsync(uri);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Calls the Locast API to acquire and set the Authorization header of the HttpClient if one isn't already set.
        /// <br /><br />
        /// Uses the LOCAST_USERNAME and LOCAST_PASSWORD values provided in the configuration.
        /// </summary>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        // TODO: Marking this as temporary.This should be handled differently with a 401 - Unauthorized handler or something.
        private async Task PerformLoginIfNeededAsync()
        {
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
            {
                var username = _configuration.GetSection("LOCAST_USERNAME").Value;
                var password = _configuration.GetSection("LOCAST_PASSWORD").Value;

                var response = await _httpClient.PostAsJsonAsync<dynamic>($"user/login", new { username = username, password = password });
                var loginResponse = await response.Content.ReadFromJsonAsync<LocastUserLoginResponseDto>();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.token);
            }
        }

        #endregion Private Methods
    }
}
