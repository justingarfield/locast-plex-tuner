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
    public class LocastClient : ILocastClient
    {
        private static readonly Uri LOCAST_API_BASE_URI = new Uri("https://api.locastnet.org/api/");

        private readonly IHttpClientFactory _clientFactory;

        private readonly IConfiguration _configuration;

        private string _loginToken;

        public LocastClient(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<LocastDmaLocationDto> GetLocationByZipCodeAsync(string zipCode)
        {
            using var client = _clientFactory.CreateClient();
            client.BaseAddress = LOCAST_API_BASE_URI;
            
            return await client.GetFromJsonAsync<LocastDmaLocationDto>($"watch/dma/zip/{zipCode}");
        }

        public async Task<LocastDmaLocationDto> GetLocationByIpAddressAsync(string ipAddress)
        {
            using var client = _clientFactory.CreateClient();
            client.BaseAddress = LOCAST_API_BASE_URI;
            client.DefaultRequestHeaders.Add("client_ip", ipAddress);
            
            return await client.GetFromJsonAsync<LocastDmaLocationDto>($"watch/dma/ip");
        }

        public async Task<LocastDmaLocationDto> GetLocationByLatLongAsync(double latitude, double longitude)
        {
            using var client = _clientFactory.CreateClient();
            client.BaseAddress = LOCAST_API_BASE_URI;

            return await client.GetFromJsonAsync<LocastDmaLocationDto>($"watch/dma/{latitude}/{longitude}");
        }

        public async Task<LocastUserDetailsDto> GetUserDetails()
        {
            await PerformLoginIfNeededAsync();

            using var client = _clientFactory.CreateClient();
            client.BaseAddress = LOCAST_API_BASE_URI;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _loginToken);

            return await client.GetFromJsonAsync<LocastUserDetailsDto>($"user/me");
        }

        public async Task<List<LocastEpgStationDto>> GetEpgForDmaAsync(string dma, DateTimeOffset? startTime = null)
        {
            await PerformLoginIfNeededAsync();

            using var client = _clientFactory.CreateClient();
            client.BaseAddress = LOCAST_API_BASE_URI;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _loginToken);

            if (startTime.HasValue)
            {
                var isoStartTime = startTime.Value.DateTime.ToString("yyyy-MM-ddTHH:mm:sszzz");
                return await client.GetFromJsonAsync<List<LocastEpgStationDto>>($"watch/epg/{dma}?startTime={isoStartTime}");
            }
            else
            {
                return await client.GetFromJsonAsync<List<LocastEpgStationDto>>($"watch/epg/{dma}");
            }
        }

        public async Task<LocastStationDto> GetStationAsync(long stationId, double latitude, double longitude)
        {
            await PerformLoginIfNeededAsync();

            using var client = _clientFactory.CreateClient();
            client.BaseAddress = LOCAST_API_BASE_URI;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _loginToken);

            return await client.GetFromJsonAsync<LocastStationDto>($"watch/station/{stationId}/{latitude}/{longitude}");
        }

        public async Task<string> GetM3U8FileAsString(Uri uri)
        {
            await PerformLoginIfNeededAsync();

            using var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _loginToken);

            return await client.GetStringAsync(uri);
        }

        private async Task UserLoginAsync()
        {
            var username = _configuration.GetSection("LOCAST_USERNAME").Value;
            var password = _configuration.GetSection("LOCAST_PASSWORD").Value;

            using var client = _clientFactory.CreateClient();
            client.BaseAddress = LOCAST_API_BASE_URI;

            var response = await client.PostAsJsonAsync<dynamic>($"user/login", new { username = username, password = password });
            var loginResponse = await response.Content.ReadFromJsonAsync<LocastUserLoginResponseDto>();

            _loginToken = loginResponse.token;
        }

        private async Task PerformLoginIfNeededAsync()
        {
            if (string.IsNullOrWhiteSpace(_loginToken))
            {
                await UserLoginAsync();
            }
        }
    }
}
