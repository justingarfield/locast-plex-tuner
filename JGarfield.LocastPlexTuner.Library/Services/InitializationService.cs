using JGarfield.LocastPlexTuner.Domain.Exceptions;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public class InitializationService : IInitializationService
    {
        private readonly ILogger<InitializationService> _logger;

        private readonly IConfiguration _configuration;

        private readonly ILocastService _locastService;

        private readonly IStationsService _stationsService;

        private readonly IEpg2XmlService _epg2XmlService;

        private readonly IDmaService _dmaService;

        private readonly ApplicationContext _applicationContext;

        public InitializationService(
            ILogger<InitializationService> logger, 
            IConfiguration configuration, 
            ILocastService locastService, 
            IStationsService stationsService, 
            IEpg2XmlService epg2XmlService, 
            ApplicationContext applicationContext,
            IDmaService dmaService)
        {
            _logger = logger;
            _configuration = configuration;
            _locastService = locastService;
            _stationsService = stationsService;
            _epg2XmlService = epg2XmlService;
            _applicationContext = applicationContext;
            _dmaService = dmaService;
        }

        public void LogInitializationBanner()
        {
            _logger.LogInformation("=====================================");
            _logger.LogInformation("    Locast Plex Tuner is starting    ");
            _logger.LogInformation("=====================================");
        }

        public async Task VerifyEnvironmentAsync(CancellationToken cancellationToken)
        {
            //===== Does the AppData folder exist? If not, create it
            if (!Directory.Exists(Constants.APPLICATION_DATA_PATH))
            {
                Directory.CreateDirectory(Constants.APPLICATION_DATA_PATH);
            }

            //===== Does the downloads folder exist? If not, create it
            if (!Directory.Exists(Constants.APPLICATION_DOWNLOADED_FILES_PATH))
            {
                Directory.CreateDirectory(Constants.APPLICATION_DOWNLOADED_FILES_PATH);
            }

            //===== Does the extracted artifacts folder exist? If not, create it
            if (!Directory.Exists(Constants.APPLICATION_EXTRACTED_FILES_PATH))
            {
                Directory.CreateDirectory(Constants.APPLICATION_EXTRACTED_FILES_PATH);
            }

            //===== Does the parsed artifacts folder exist? If not, create it
            if (!Directory.Exists(Constants.APPLICATION_PARSED_FILES_PATH))
            {
                Directory.CreateDirectory(Constants.APPLICATION_PARSED_FILES_PATH);
            }

            //===== Does the cache folder exist? If not, create it
            if (!Directory.Exists(Constants.APPLICATION_CACHE_PATH))
            {
                Directory.CreateDirectory(Constants.APPLICATION_CACHE_PATH);
            }

            //===== Can we Read/Write/Delete files in the AppData folder?
            var testFilename = Path.Combine(Constants.APPLICATION_DATA_PATH, "initialization-service.tmp");
            var file = File.Create(testFilename);
            await file.FlushAsync(cancellationToken);
            file.Close();
            File.Delete(testFilename);

            //===== Did we find Locast credentials in the built configuration?
            var username = _configuration.GetSection("LOCAST_USERNAME").Value;
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new LocastPlexTunerDomainException("Could not find LOCAST_USERNAME specified in User Secrets or Environment Variables.");
            }

            var password = _configuration.GetSection("LOCAST_PASSWORD").Value;
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new LocastPlexTunerDomainException("Could not find LOCAST_PASSWORD specified in User Secrets or Environment Variables.");
            }

            //===== Can we find a valid ffmpeg binary?
            var ffmpegBinaryPath = _configuration.GetSection("FFMPEG_BINARY").Value;
            if (!_applicationContext.RunningInContainer && string.IsNullOrWhiteSpace(ffmpegBinaryPath))
            {
                throw new LocastPlexTunerDomainException("Could not find FFMPEG_BINARY specified in User Secrets or Environment Variables. A local copy of FFMPEG is required for this to work properly.");
            }

            if (!_applicationContext.RunningInContainer && !File.Exists(ffmpegBinaryPath))
            {
                throw new LocastPlexTunerDomainException($"The provided path ({ffmpegBinaryPath}) in the FFMPEG_BINARY configuration value could not be found.");
            }
        }

        public async Task InitializeEnvironmentAsync()
        {
            // Should be safe to get this since Init checks for existence before-hand.
            var ffmpegBinaryPath = _configuration.GetSection("FFMPEG_BINARY").Value;
            _applicationContext.FfmpegBinaryPath = ffmpegBinaryPath;

            // Allow user to override how DMA lookup is performed (checked in this order)
            var dmaOverride = _configuration.GetSection("LOCAST_DMA").Value;
            double.TryParse(_configuration.GetSection("LOCAST_LATITUDE").Value, out double latitudeOverride);
            double.TryParse(_configuration.GetSection("LOCAST_LONGITUDE").Value, out double longitudeOverride);
            var zipCodeOverride = _configuration.GetSection("LOCAST_ZIPCODE").Value;

            var dmaLocation = await _dmaService.GetDmaLocationAsync(zipCodeOverride, latitudeOverride, longitudeOverride, dmaOverride);
            if (dmaLocation == null)
            {
                throw new LocastPlexTunerDomainException("Unable to determine the Designated Market Area (DMA) for your location. Please visit https://www.locast.org/dma to verify your DMA and set it explicitly using the LOCAST_DMA configuration setting.");
            }

            if (!dmaLocation.Active)
            {
                throw new LocastPlexTunerDomainException($"The Designated Market Area (DMA) for your location is either not supported by Locast, or they are reporting it as inactive for some reason. Please visit https://www.locast.org/dma to verify your DMA ({dmaLocation.DMA} [{dmaLocation.Name}]).");
            }

            // TODO: Figure out how to support free accounts too
            if (!await _locastService.IsActivelyDonatingUserAsync())
            {
                throw new LocastPlexTunerDomainException("LocastPlexTuner currently only works for actively donating Locast subscribers. This is due to the fact that we don't currently have the ability to deal with the advertisements that get injected for free subscribers. Please visit https://www.locast.org/donate, login, and give $5/mo to help support the service.");
            }

            _logger.LogInformation("Starting First time Stations refresh...");
            await _stationsService.RefreshDmaStationsAndChannels();

            _logger.LogInformation("Starting First time EPG refresh...");
            await _epg2XmlService.GenerateEpgFile();
        }
    }
}
