using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.WebApi
{
    public class LocastHostedService : IHostedService
    {
        private readonly ILogger _logger;

        private readonly IInitializationService _initializationService;

        public LocastHostedService(
            ILogger<LocastHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IInitializationService initializationService
        )
        {
            _logger = logger;
            _initializationService = initializationService;

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
            
            // TODO: Start timer to call RefreshDmaStationsAndChannels again at some point

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
