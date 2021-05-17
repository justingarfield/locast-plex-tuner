using JGarfield.LocastPlexTuner.Library.Domain.Exceptions;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
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

        private readonly ApplicationContext _applicationContext;

        public InitializationService(ILogger<InitializationService> logger, IConfiguration configuration, ILocastService locastService, IStationsService stationsService, IEpg2XmlService epg2XmlService, ApplicationContext applicationContext)
        {
            _logger = logger;
            _configuration = configuration;
            _locastService = locastService;
            _stationsService = stationsService;
            _epg2XmlService = epg2XmlService;
            _applicationContext = applicationContext;
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
        }

        public async Task InitializeEnvironmentAsync()
        {
            var zipCodeOverride = _configuration.GetSection("LOCAST_ZIPCODE").Value;
            var zipCode = string.IsNullOrWhiteSpace(zipCodeOverride) ? null : zipCodeOverride;

            var dmaLocation = await _locastService.GetDmaLocationAsync(zipCode);
            if (dmaLocation == null)
            {
                throw new LocastPlexTunerDomainException("Unable to determine the Designated Market Area (DMA) for your location. Please visit https://www.locast.org/dma to verify your DMA and set it explicitly using the LOCAST_DMA configuration setting.");
            }

            _applicationContext.CurrentDMA = dmaLocation;

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
            await _stationsService.RefreshDmaStationsAndChannels(dmaLocation.DMA);

            _logger.LogInformation("Starting First time EPG refresh...");
            await _epg2XmlService.GenerateEpgFile(dmaLocation.DMA);
        }
    }
}
