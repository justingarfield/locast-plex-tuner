using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Tuner;
using JGarfield.LocastPlexTuner.Library.Domain;
using JGarfield.LocastPlexTuner.Library.Metrics;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public class TunerService : ITunerService, IMetricsProvider<CollectionOfTunerMetrics>
    {
        #region Private Members

        private readonly ILogger<TunerService> _logger;

        private readonly IStationsService _stationsService;

        private readonly ILocastService _locastService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IHttpFfmpegService _httpFfmpegService;

        private bool _activeStationScan = false;

        private List<Tuner> _tunerInstances;

        private readonly ApplicationContext _applicationContext;

        #endregion Private Members

        public TunerService(
            ILogger<TunerService> logger, 
            IStationsService stationsService, 
            ILocastService locastService, 
            IHttpContextAccessor httpContextAccessor, 
            IHttpFfmpegService httpFfmpegService, 
            ApplicationContext applicationContext
        )
        {
            _logger = logger;
            _stationsService = stationsService;
            _locastService = locastService;
            _httpContextAccessor = httpContextAccessor;
            _httpFfmpegService = httpFfmpegService;
            _applicationContext = applicationContext;
        }

        public Task<string> GetRmgIdentification()
        {
            _logger.LogDebug($"Entering {nameof(GetRmgIdentification)}");

            var tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgIdentification, Constants.reporting_friendly_name);

            _logger.LogDebug($"Returning value of {tokenizedXml}");
            return Task.FromResult(tokenizedXml);
        }

        public Task<string> GetRmgDeviceDiscoverXml()
        {
            var tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgDeviceDiscover, Constants.uuid, Constants.reporting_friendly_name, Constants.reporting_model, Constants.tuner_count, _applicationContext.BaseUri);
            return Task.FromResult(tokenizedXml);
        }

        public Task<string> GetRmgScanStatusXml()
        {
            var tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgScanStatus);
            return Task.FromResult(tokenizedXml);
        }

        public Task<string> GetDeviceXml()
        {
            var tokenizedXml = string.Format(
                TunerXmlTemplates.xmlDiscover,
                Constants.reporting_friendly_name,
                Constants.reporting_model,
                Constants.uuid,
                _applicationContext.BaseUri
            );
            return Task.FromResult(tokenizedXml);
        }

        public async Task<Discover> GetDiscoverAsync()
        {
            var discover = new Discover {
                FriendlyName = Constants.reporting_friendly_name,
                Manufacturer = Constants.reporting_friendly_name,
                ModelNumber = Constants.reporting_model,
                FirmwareName = Constants.reporting_firmware_name,
                TunerCount = Constants.tuner_count,
                FirmwareVersion = Constants.reporting_firmware_ver,
                DeviceID = Constants.uuid,
                DeviceAuth = "LocastPlexTuner",
                BaseURL = $"{_applicationContext.BaseUri}",
                LineupURL = $"{_applicationContext.BaseUri}lineup.json",
            };

            return await Task.FromResult(discover);
        }

        public Task<LineupStatus> GetLineupStatusAsync()
        {
            LineupStatus lineupStatus = null;

            if (_activeStationScan)
            {
                lineupStatus = new LineupStatus
                {
                    ScanInProgress = true, 
                    Progress = 50
                };
            }
            else
            {
                var tunerType = Constants.tuner_type ?? "Antenna";
                lineupStatus = new LineupStatus
                {
                    ScanInProgress = false,
                    ScanPossible = true,
                    Source = tunerType,
                    SourceList = new[] { tunerType }
                };
            }

            return Task.FromResult(lineupStatus);
        }

        public async Task<List<LineupItem>> GetChannelLineupAsync()
        {
            var channelLineup = new List<LineupItem>();

            var dmaStationsAndChannels = await _stationsService.GetDmaStationsAndChannels(_applicationContext.CurrentDMA.DMA);
            foreach (var dmaStationsAndChannel in dmaStationsAndChannels)
            {
                channelLineup.Add(new LineupItem {
                    GuideNumber = $"{dmaStationsAndChannel.Value.channel}",
                    GuideName = dmaStationsAndChannel.Value.friendlyName,
                    URL = $"{_applicationContext.BaseUri}watch/{dmaStationsAndChannel.Key}"
                });
            }

            return channelLineup;
        }
        
        public async Task DoTuning(long stationId)
        {
            _logger.LogDebug($"Entering {nameof(DoTuning)} with a stationId of: {stationId}");

            var stationStreamUri = await _locastService.GetStationStreamUri(stationId);
            _logger.LogTrace($"Using a {nameof(stationStreamUri)} value of: {stationStreamUri}");

            var dmaStationsAndChannels = await _stationsService.GetDmaStationsAndChannels(_applicationContext.CurrentDMA.DMA);
            _logger.LogTrace($"Using {nameof(dmaStationsAndChannels)} value of: {dmaStationsAndChannels}");

            var idleTuner = GetIdleTuner();
            _logger.LogTrace($"Using {nameof(idleTuner)} value of: {idleTuner}");

            if (idleTuner == null)
            {
                _logger.LogWarning($"Could not find an idle tuner when attempting to tune to station: {stationId}");
                _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await _httpContextAccessor.HttpContext.Response.WriteAsync("All tuners are in use.");
                return;
            }

            idleTuner.SetScanStatusToTuned();
            idleTuner.CurrentChannel = dmaStationsAndChannels[stationId].channel;

            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                await _httpFfmpegService.StreamToHttpResponseAsync(stationStreamUri, httpContext);
            }
            finally
            {
                // Place the Tuner back into the available pool by marking it as Idle
                idleTuner.SetScanStatusToIdle();
            }

            _logger.LogDebug($"Leaving {nameof(DoTuning)}");
        }

        public async Task GetStatistics()
        {
            await Task.CompletedTask;
        }

        public async Task StartStationScan()
        {
            var tuners = GetTuners();
            var idleTuners = tuners.Where(_ => _.Status == TunerStatus.Idle);

            foreach (var tuner in idleTuners)
            {
                tuner.SetScanStatusToScanning();
            }

            _activeStationScan = true;

            await Task.CompletedTask;
        }

        public async Task StopAllScanningForAllTuners()
        {
            var tuners = GetTuners();
            var scanningTuners = tuners.Where(_ => _.Status == TunerStatus.Scanning);

            foreach (var tuner in scanningTuners)
            {
                tuner.SetScanStatusToIdle();
            }

            _activeStationScan = false;

            await Task.CompletedTask;
        }

        public Task<string> GetRmgScanProvidersXml()
        {
            var tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgScanProviders, "Boston");
            return Task.FromResult(tokenizedXml);
        }

        public async Task<MediaContainer> GetRmgDeviceChannelItemsXml()
        {
            var dmaStationsAndChannels = await _stationsService.GetDmaStationsAndChannels(_applicationContext.CurrentDMA.DMA);

            List<Channel> channels = new List<Channel>();
            foreach (var dmaStationsAndChannel in dmaStationsAndChannels)
            {
                var channel = new Channel(
                    dmaStationsAndChannel.Value.friendlyName, 
                    dmaStationsAndChannel.Value.channel
                );
                channels.Add(channel);
            }

            return new MediaContainer(channels);
        }

        public async Task<string> GetRmgDeviceIdentityXml()
        {
            var allTuners = GetTuners();
            var sb = new StringBuilder();
            string tokenizedXml;
            for (var i = 0; i < allTuners.Count; i++)
            {
                if (allTuners[i].Status == TunerStatus.Idle)
                {
                    tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgTunerIdle, i);
                } 
                else if (allTuners[i].Status == TunerStatus.Scanning)
                {
                    tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgTunerScanning, i);
                } 
                else
                {
                    tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgTunerStreaming, i, allTuners[i].CurrentChannel);
                }

                sb.Append(tokenizedXml);
            }

            tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgDeviceIdentity, Constants.uuid, Constants.reporting_friendly_name, Constants.reporting_model, Constants.tuner_count, _applicationContext.BaseUri, sb.ToString());
            return await Task.FromResult(tokenizedXml);
        }

        private Tuner GetIdleTuner()
        {
            var allTuners = GetTuners();
            return allTuners.First(tuner => tuner.Status == TunerStatus.Idle);
        }

        private List<Tuner> GetTuners(TunerStatus? scanStatus = null)
        {
            if (_tunerInstances == null)
            {
                var tunerCount = 4;

                // int.TryParse(_configuration.GetSection("LOCAST_TUNERCOUNT").Value, out tunerCount);

                _tunerInstances = new List<Tuner>();
                for (var i = 0; i < tunerCount; i++)
                {
                    _tunerInstances.Add(new Tuner());
                }
            }

            var tuners = scanStatus.HasValue
                    ? _tunerInstances.Where(_ => _.Status == scanStatus).ToList() 
                    : _tunerInstances;

            return tuners;
        }

        public CollectionOfTunerMetrics GetMetrics()
        {
            throw new NotImplementedException();
            /*var metricsCollection = new CollectionOfTunerMetrics();
            foreach (var tuner in GetTuners())
            {
                var tunerMetrics = new TunerMetrics
                {
                    
                };
            }*/
        }
    }
}
