using JGarfield.LocastPlexTuner.Library;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.WebApi
{
    public class LocastHostedService : IHostedService
    {
        /// <summary>
        /// Chances of Stations and Channels getting added/removed is pretty rare due to contractual 
        /// obligations and what-not, so setting this to 24-hours.
        /// </summary>
        private const int DEFAULT_DMA_STATIONS_AND_CHANNELS_REFRESH_RATE_IN_HOURS = 24;

        /// <summary>
        /// I would think that the chances of EPGs refreshing info would be pretty minimal since 
        /// airings are usually pre-determined well in advance, but just in-case, let's set this
        /// to 4-hours, which seems more than fair.
        /// </summary>
        private const int DEFAULT_EPG_REFRESH_RATE_IN_HOURS = 4;

        private readonly ILogger _logger;

        private readonly IInitializationService _initializationService;

        private readonly IStationsService _stationsService;

        private readonly IEpg2XmlService _epg2XmlService;

        private readonly IDmaService _dmaService;

        private readonly ITunerService _tunerService;

        private readonly ApplicationContext _applicationContext;

        private DateTimeOffset _lastEpgRefresh;

        private DateTimeOffset _lastDmaStationsAndChannelsRefresh;

        private bool _shuttingDown = false;

        public LocastHostedService(
            ILogger<LocastHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IInitializationService initializationService,
            IStationsService stationsService,
            IEpg2XmlService epg2XmlService,
            IDmaService dmaService,
            ApplicationContext applicationContext,
            ITunerService tunerService
        )
        {
            _logger = logger;
            _initializationService = initializationService;
            _stationsService = stationsService;
            _epg2XmlService = epg2XmlService;
            _dmaService = dmaService;
            _applicationContext = applicationContext;
            _tunerService = tunerService;

            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Entered {nameof(StartAsync)}");

            _initializationService.LogInitializationBanner();

            await _initializationService.VerifyEnvironmentAsync(cancellationToken);

            await _initializationService.InitializeEnvironmentAsync();
            
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
            
            while (!_shuttingDown)
            {
                if (DateTimeOffset.UtcNow.Subtract(_lastEpgRefresh).TotalHours > DEFAULT_EPG_REFRESH_RATE_IN_HOURS)
                {
                    _logger.LogInformation("Refreshing Electronic Programming Guide (EPG)...");

                    //await _epg2XmlService.GenerateEpgFile(_applicationContext.CurrentDMA.DMA);
                    _lastEpgRefresh = DateTimeOffset.UtcNow;

                    _logger.LogInformation("Electronic Programming Guide (EPG) has been refreshed.");
                }

                if (DateTimeOffset.UtcNow.Subtract(_lastDmaStationsAndChannelsRefresh).TotalHours > DEFAULT_DMA_STATIONS_AND_CHANNELS_REFRESH_RATE_IN_HOURS)
                {
                    _logger.LogInformation("Refreshing DMA Stations and Channels...");

                    //await _stationsService.RefreshDmaStationsAndChannels(_applicationContext.CurrentDMA.DMA);
                    _lastDmaStationsAndChannelsRefresh = DateTimeOffset.UtcNow;

                    _logger.LogInformation("DMA Stations and Channels have been refreshed.");
                }

                // await Task.Delay(TimeSpan.FromSeconds(30));
            }

            _logger.LogDebug($"Leaving {nameof(OnStarted)}");
        }

        private void OnStopping()
        {
            _logger.LogDebug("3. OnStopping has been called.");

            _shuttingDown = true;
            
            _tunerService.StopAllActiveStreams();

            _logger.LogDebug($"Leaving {nameof(OnStopping)}");
        }

        private void OnStopped()
        {
            _logger.LogDebug("5. OnStopped has been called.");

            _logger.LogDebug($"Leaving {nameof(OnStopped)}");
        }
    }
}
