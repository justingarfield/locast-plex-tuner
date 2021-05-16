using JGarfield.LocastPlexTuner.Library.Domain;
using JGarfield.LocastPlexTuner.Library.Services;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.WebApi
{
    public class LocastHostedService : IHostedService
    {
        private readonly ILogger _logger;

        private readonly IInitializationService _initializationService;

        private readonly ILocastService _locastService;

        private readonly IFccService _fccService;

        private readonly IStationsService _stationsService;

        private readonly IConfiguration _configuration;

        private readonly ITunerService _tunerService;

        private readonly IEpg2XmlService _epg2XmlService;

        public LocastHostedService(
            ILogger<LocastHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IInitializationService initializationService,
            ILocastService locastService,
            IFccService fccService,
            IStationsService stationsService,
            IConfiguration configuration,
            ITunerService tunerService,
            IEpg2XmlService epg2XmlService)
        {
            _logger = logger;
            _initializationService = initializationService;
            _locastService = locastService;
            _fccService = fccService;
            _stationsService = stationsService;
            _configuration = configuration;
            _tunerService = tunerService;
            _epg2XmlService = epg2XmlService;

            //appLifetime.ApplicationStarted.Register(OnStarted);
            //appLifetime.ApplicationStopping.Register(OnStopping);
            //appLifetime.ApplicationStopped.Register(OnStopped);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Entered {nameof(StartAsync)}");

            _initializationService.LogInitializationBanner();

            await _initializationService.VerifyEnvironmentAsync();

            var zipCodeOverride = _configuration.GetSection("LOCAST_ZIPCODE").Value;

            var dmaLocation = await _locastService.GetDmaLocationAsync(zipCodeOverride);
            if (dmaLocation == null)
            {
                throw new ApplicationException("Could not determine DMA Location. Aborting...");
            }

            if (!dmaLocation.Active)
            {
                throw new ApplicationException("Locast reports that this DMA\\Market area is not currently active! Aborting...");
            }

            if (!await _locastService.IsActivelyDonatingUserAsync())
            {
                throw new ApplicationException("Sorry, this app only supports Actively Donating Locast members. Please go spend some monies!");
            }

            _logger.LogInformation("Starting First time Stations refresh...");
            await _stationsService.RefreshDmaStationsAndChannels(dmaLocation.DMA);

            // TODO: Start timer to call RefreshDmaStationsAndChannels again at some point

            _logger.LogInformation("Starting First time EPG refresh...");
            await _epg2XmlService.GenerateEpgFile();

            // TODO: Start timer to call EpgProcess again at some point

            _logger.LogInformation("LocastPlexTuner is now online.");

            _logger.LogDebug($"Leaving {nameof(StartAsync)}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("4. StopAsync has been called.");

            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogDebug("2. OnStarted has been called.");
        }

        private void OnStopping()
        {
            _logger.LogDebug("3. OnStopping has been called.");
        }

        private void OnStopped()
        {
            _logger.LogDebug("5. OnStopped has been called.");
        }
    }
}
