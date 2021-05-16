using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public class InitializationService : IInitializationService
    {
        private readonly ILogger<InitializationService> _logger;

        private readonly IConfiguration _configuration;

        public InitializationService(ILogger<InitializationService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task VerifyEnvironmentAsync()
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

            //===== Can we Read/Write/Delete in the AppData folder?
            var testFilename = Path.Combine(Constants.APPLICATION_DATA_PATH, "initialization-service.tmp");
            var file = File.Create(testFilename);
            await file.FlushAsync();
            file.Close();
            File.Delete(testFilename);

            //===== Did we find Locast credentials in the built configuration?
            var username = _configuration.GetSection("LOCAST_USERNAME").Value;
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ApplicationException("Could not find LOCAST_USERNAME specified in Settings or Environment Variables.");
            }

            var password = _configuration.GetSection("LOCAST_PASSWORD").Value;
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ApplicationException("Could not find LOCAST_PASSWORD specified in Settings or Environment Variables.");
            }
        }

        public void LogInitializationBanner()
        {
            _logger.LogInformation("=====================================");
            _logger.LogInformation("    Locast Plex Tuner is starting    ");
            _logger.LogInformation("=====================================");
        }
    }
}
