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
        private DmaLocation _currentDmaLocation;

        private readonly ILogger<LocastService> _logger;

        private readonly ILocastClient _locastClient;

        private readonly IIpInfoClient _ipInfoClient;

        public DmaService(ILogger<LocastService> logger, ILocastClient locastClient, IIpInfoClient ipInfoClient)
        {
            _logger = logger;
            _locastClient = locastClient;
            _ipInfoClient = ipInfoClient;
        }

        public async Task<DmaLocation> GetDmaLocationAsync(string zipCode = null)
        {
            if (_currentDmaLocation == null)
            {
                // zip_format = re.compile(r'^[0-9]{5}$')
                LocastDmaLocationDto locastDmaLocationDto;

                if (string.IsNullOrWhiteSpace(zipCode))
                {
                    var ipAddress = await _ipInfoClient.GetPublicIpAddressAsync();
                    _logger.LogInformation($"Public IP Address was: {ipAddress}");
                    locastDmaLocationDto = await _locastClient.GetDmaByIpAddressAsync(ipAddress);
                }
                else
                {
                    locastDmaLocationDto = await _locastClient.GetDmaByZipCodeAsync(zipCode);
                }

                _currentDmaLocation = new DmaLocation
                {
                    Active = locastDmaLocationDto.active,
                    DMA = locastDmaLocationDto.DMA,
                    LargeUrl = locastDmaLocationDto.large_url,
                    Latitude = locastDmaLocationDto.latitude,
                    Longitude = locastDmaLocationDto.longitude,
                    Name = locastDmaLocationDto.name,
                    PublicIP = locastDmaLocationDto.publicIP,
                    SmallUrl = locastDmaLocationDto.small_url
                };

                if (locastDmaLocationDto.announcements.Length > 0)
                {
                    foreach (var announcement in locastDmaLocationDto.announcements)
                    {
                        _currentDmaLocation.Announcements.Add(new DmaLocationAnnouncement
                        {
                            Title = announcement.title,
                            MessageHtml = announcement.messageHtml,
                            MessageText = announcement.messageText,
                            MessageType = announcement.messageType
                        });
                    }
                }

                _logger.LogInformation($"Got location as {_currentDmaLocation.Name} - DMA {_currentDmaLocation.DMA} - Lat\\Lon {_currentDmaLocation.Latitude}\\{_currentDmaLocation.Longitude}.");
            }

            return _currentDmaLocation;
        }

    }
}
