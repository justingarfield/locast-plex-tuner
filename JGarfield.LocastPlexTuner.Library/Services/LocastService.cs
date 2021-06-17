using JGarfield.LocastPlexTuner.Domain;
using JGarfield.LocastPlexTuner.Library.Clients.Contracts;
using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Epg;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using M3USharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public class LocastService : ILocastService
    {
        private readonly ILogger<LocastService> _logger;

        private readonly ILocastClient _locastClient;

        private readonly IDmaService _dmaService;

        public LocastService(ILogger<LocastService> logger, ILocastClient locastClient, IDmaService dmaService)
        {
            _logger = logger;
            _locastClient = locastClient;
            _dmaService = dmaService;
        }

        public async Task<List<LocastChannelDto>> GetEpgStationsForDmaAsync(string dma, DateTimeOffset? startTime = null)
        {
            var epgResponse = await _locastClient.GetEpgForDmaAsync(dma, startTime);
            _logger.LogInformation($"Found {epgResponse.Count} stations for DMA {dma}.");

            return epgResponse;
        }

        public async Task<StreamDetails> GetStationStreamUri(long stationId)
        {
            var dma = await _dmaService.GetDmaLocationAsync();

            var locastStationDto = await _locastClient.GetStationAsync(stationId, dma.Latitude, dma.Longitude);

            var m3u8Data = await _locastClient.GetM3U8FileAsString(new Uri(locastStationDto.streamUrl));

            var m3ufile = M3UReader.Parse(m3u8Data);
            
            if (m3ufile.Streams.Count > 0)
            {
                StreamInfo bestStream = null;
                foreach (var videoStream in m3ufile.Streams)
                { 
                    if (bestStream == null)
                    {
                        bestStream = videoStream;
                    }
                    else if ((videoStream.ResolutionWidth > bestStream.ResolutionWidth)
                      && (videoStream.ResolutionHeight > bestStream.ResolutionHeight))
                    {
                        bestStream = videoStream;
                    }
                    else if ((videoStream.ResolutionWidth == bestStream.ResolutionWidth)
                     && (videoStream.ResolutionHeight == bestStream.ResolutionHeight)
                     && (videoStream.Bandwidth > bestStream.Bandwidth))
                    {
                        bestStream = videoStream;
                    }
                }

                if (bestStream != null)
                {
                    _logger.LogInformation($"{stationId} will use {bestStream.ResolutionWidth} x {bestStream.ResolutionHeight} resolution at {bestStream.Bandwidth}bps");
                    
                    return new StreamDetails {
                        Uri = new Uri(new Uri(locastStationDto.streamUrl), bestStream.Path),
                        BitrateInBytes = bestStream.Bandwidth / 8 // bits-per-second -> bytes-per-second
                    };
                }
            }
            else
            {
                _logger.LogInformation("No variant streams found for this station.  Assuming single stream only.");
                return new StreamDetails {
                    Uri = new Uri(locastStationDto.streamUrl)
                };
            }

            return null;
        }

        public async Task<bool> IsActivelyDonatingUserAsync()
        {
            var locastUserDetailsDto = await _locastClient.GetUserDetails();
            _logger.LogInformation($"User donated: {locastUserDetailsDto.didDonate}");

            if (locastUserDetailsDto.didDonate && locastUserDetailsDto.donationExpire > default(DateTimeOffset).UtcTicks)
            {
                var donateExp = DateTimeOffset.FromUnixTimeMilliseconds(locastUserDetailsDto.donationExpire);
                _logger.LogInformation($"User donationExpire: {donateExp}");
                
                if (DateTimeOffset.UtcNow > donateExp)
                {
                    _logger.LogError("Error!  User's donation ad-free period has expired.");
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private void DetectCallSign(string callSign)
        {
            /*var verified = false;
            var stationType = null;
            var subChannel = null;
            var backIndex = -1;*/

            _logger.LogWarning($"======>   {callSign}");
        }
    }

    public class EpgStationChannel
    {
        public int sid { get; set; }

        public string callSign { get; set; }

        public string logoUrl { get; set; }

        public decimal channel { get; set; }

        public string friendlyName { get; set; }
    }
}
