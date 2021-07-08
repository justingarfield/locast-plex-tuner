using JGarfield.LocastPlexTuner.Domain;
using JGarfield.LocastPlexTuner.Library.Clients.Contracts;
using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Dma;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    /// <summary>
    /// A service that provides Designated Market Area (DMA) related capabilities.
    /// </summary>
    public class DmaService : IDmaService
    {
        /// <summary>
        /// The DmaLocation 
        /// </summary>
        private DesignatedMarketArea _currentDmaLocation;

        /// <summary>
        /// 
        /// </summary>
        private readonly ILogger<LocastService> _logger;

        /// <summary>
        /// 
        /// </summary>
        private readonly ILocastClient _locastClient;

        /// <summary>
        /// 
        /// </summary>
        private readonly IIpInfoClient _ipInfoClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="locastClient"></param>
        /// <param name="ipInfoClient"></param>
        public DmaService(ILogger<LocastService> logger, ILocastClient locastClient, IIpInfoClient ipInfoClient)
        {
            _logger = logger;
            _locastClient = locastClient;
            _ipInfoClient = ipInfoClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zipCode"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="dma"></param>
        /// <param name="forceLookup"></param>
        /// <returns></returns>
        public async Task<DesignatedMarketArea> GetDmaLocationAsync(string zipCode = null, double latitude = default, double longitude = default, string dma = null, bool forceLookup = false)
        {
            if (_currentDmaLocation == null || forceLookup)
            {
                _logger.LogInformation($"No current DMA set. Performing lookup(s)...");

                LocastDmaLocationDto locastDmaLocationDto = null;

                // Explicit DMA first
                if (locastDmaLocationDto == null && !string.IsNullOrWhiteSpace(dma))
                {
                    _logger.LogInformation($"Attemping DMA lookup using a DMA of {dma}");
                    locastDmaLocationDto = await _locastClient.GetDmaAsync(dma);
                }

                // Next try lat/long if available
                if (locastDmaLocationDto == null && latitude > default(double) && longitude > default(double))
                {
                    _logger.LogInformation($"Attemping DMA lookup using a Latitude of {latitude} and a Longitude of {longitude}.");
                    locastDmaLocationDto = await _locastClient.GetDmaByLatLongAsync(latitude, longitude);
                }

                // Next try zip code if available
                if (locastDmaLocationDto == null && !string.IsNullOrWhiteSpace(zipCode))
                {
                    _logger.LogInformation($"Attemping DMA lookup using a Zip Code of {zipCode}.");
                    locastDmaLocationDto = await _locastClient.GetDmaByZipCodeAsync(zipCode);
                }

                // If all else fails, lookup via IP Address
                if (locastDmaLocationDto == null)
                {
                    var ipAddress = await _ipInfoClient.GetPublicIpAddressAsync();
                    _logger.LogInformation($"Attemping DMA lookup using an IP Address of {ipAddress}.");
                    locastDmaLocationDto = await _locastClient.GetDmaByIpAddressAsync(ipAddress);
                }

                _currentDmaLocation = new DesignatedMarketArea
                {
                    Active = locastDmaLocationDto.active,
                    Id = locastDmaLocationDto.DMA,
                    LargeUrl = locastDmaLocationDto.large_url,
                    Latitude = locastDmaLocationDto.latitude,
                    Longitude = locastDmaLocationDto.longitude,
                    Name = locastDmaLocationDto.name,
                    PublicIP = locastDmaLocationDto.publicIP,
                    SmallUrl = locastDmaLocationDto.small_url
                };

                // Announcements will show up when things like Physical Tuner Maintenance or Outages are occurring for a particular DMA.
                if (locastDmaLocationDto.announcements.Length > 0)
                {
                    foreach (var announcement in locastDmaLocationDto.announcements)
                    {
                        _currentDmaLocation.Announcements.Add(new DmaAnnouncement
                        {
                            Title = announcement.title,
                            MessageHtml = announcement.messageHtml,
                            MessageText = announcement.messageText,
                            MessageType = announcement.messageType
                        });
                    }
                }

                _logger.LogInformation($"Got location as {_currentDmaLocation.Name} - DMA {_currentDmaLocation.Id} - Lat\\Lon {_currentDmaLocation.Latitude}\\{_currentDmaLocation.Longitude}.");
            }

            return _currentDmaLocation;
        }
    }
}
