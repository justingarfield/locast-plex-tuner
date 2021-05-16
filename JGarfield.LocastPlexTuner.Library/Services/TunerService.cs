using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Tuner;
using JGarfield.LocastPlexTuner.Library.Domain;
using JGarfield.LocastPlexTuner.Library.Metrics;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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

        private const string reporting_friendly_name = "LocastPlexTuner";

        private const string reporting_model = "lpt";

        private const string uuid = "xasudfds";

        private const string base_url = "localhost:6077";

        private const string reporting_firmware_name = "locastplextuner";

        private const int tuner_count = 4;

        private const string reporting_firmware_ver = "v0.0.1";

        private const string tuner_type = "Antenna";

        private readonly ILogger<TunerService> _logger;

        private readonly IStationsService _stationsService;

        private readonly ILocastService _locastService;

        private readonly IHttpFfmpegService _httpFfmpegService;

        private bool _hdhr_station_scan = false;

        private readonly IConfiguration _configuration;

        private List<Tuner> _tunerPool;

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion Private Members

        public TunerService(ILogger<TunerService> logger, IConfiguration configuration, IStationsService stationsService, ILocastService locastService, IHttpContextAccessor httpContextAccessor, IHttpFfmpegService httpFfmpegService)
        {
            _logger = logger;
            _stationsService = stationsService;
            _locastService = locastService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpFfmpegService = httpFfmpegService;
        }

        public Task<string> GetRmgIdentification()
        {
            var tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgIdentification, reporting_friendly_name);
            return Task.FromResult(tokenizedXml);
        }

        public Task<string> GetRmgDeviceDiscoverXml()
        {
            var tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgDeviceDiscover, uuid, reporting_friendly_name, reporting_model, tuner_count, base_url);
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
                reporting_friendly_name,
                reporting_model,
                uuid,
                base_url
            );
            return Task.FromResult(tokenizedXml);
        }

        public async Task<Discover> GetDiscoverAsync()
        {
            var discover = new Discover {
                FriendlyName = reporting_friendly_name,
                Manufacturer = reporting_friendly_name,
                ModelNumber = reporting_model,
                FirmwareName = reporting_firmware_name,
                TunerCount = tuner_count,
                FirmwareVersion = reporting_firmware_ver,
                DeviceID = uuid,
                DeviceAuth = "LocastPlexTuner",
                BaseURL = $"http://{base_url}",
                LineupURL = $"http://{base_url}/lineup.json",
            };

            return await Task.FromResult(discover);
        }

        public Task<LineupStatus> GetLineupStatusAsync()
        {
            LineupStatus lineupStatus = null;

            if (_hdhr_station_scan)
            {
                lineupStatus = new LineupStatus
                {
                    ScanInProgress = true, 
                    Progress = 50
                };
            }
            else
            {
                var tunerType = tuner_type ?? "Antenna";
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

            var dmaStationsAndChannels = await _stationsService.GetDmaStationsAndChannels("506");
            foreach (var dmaStationsAndChannel in dmaStationsAndChannels)
            {
                channelLineup.Add(new LineupItem {
                    GuideNumber = $"{dmaStationsAndChannel.Value.channel}",
                    GuideName = dmaStationsAndChannel.Value.friendlyName,
                    URL = $"http://{base_url}/watch/{dmaStationsAndChannel.Key}"
                });
            }

            return channelLineup;
        }
        
        public async Task DoTuning(long stationId)
        {
            var stationStreamUri = await _locastService.GetStationStreamUri(stationId);

            var dmaStationsAndChannels = await _stationsService.GetDmaStationsAndChannels("506");

            var idleTuner = GetIdleTuner();

            if (idleTuner == null)
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await _httpContextAccessor.HttpContext.Response.WriteAsync("All tuners are in use.");
                return;
            }

            idleTuner.ScanStatus = TunerStatus.TunedToChannel;
            idleTuner.CurrentChannel = dmaStationsAndChannels[stationId].channel;

            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                await _httpFfmpegService.StreamToHttpResponseAsync(stationStreamUri, httpContext);
            }
            finally
            {
                // Place the Tuner back into the available pool by marking it as Idle
                idleTuner.ScanStatus = TunerStatus.Idle;
            }
        }

        public async Task GetStatistics()
        {
            await Task.CompletedTask;
        }

        public async Task StartStationScan()
        {
            var tuners = GetTuners();
            var idleTuners = tuners.Where(_ => _.ScanStatus == TunerStatus.Idle);

            foreach (var tuner in idleTuners)
            {
                tuner.ScanStatus = TunerStatus.Scanning;
            }

            _hdhr_station_scan = true;

            await Task.CompletedTask;
        }

        public async Task StopAllScanningForAllTuners()
        {
            var tuners = GetTuners();
            var scanningTuners = tuners.Where(_ => _.ScanStatus == TunerStatus.Scanning);

            foreach (var tuner in scanningTuners)
            {
                tuner.ScanStatus = TunerStatus.Idle;
            }

            _hdhr_station_scan = false;

            await Task.CompletedTask;
        }

        public Task<string> GetRmgScanProvidersXml()
        {
            var tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgScanProviders, "Boston");
            return Task.FromResult(tokenizedXml);
        }

        public async Task<MediaContainer> GetRmgDeviceChannelItemsXml()
        {
            var dmaStationsAndChannels = await _stationsService.GetDmaStationsAndChannels("506");

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
                if (allTuners[i].ScanStatus == TunerStatus.Idle)
                {
                    tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgTunerIdle, i);
                } 
                else if (allTuners[i].ScanStatus == TunerStatus.Scanning)
                {
                    tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgTunerScanning, i);
                } 
                else
                {
                    tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgTunerStreaming, i, allTuners[i].CurrentChannel);
                }

                sb.Append(tokenizedXml);
            }

            tokenizedXml = string.Format(TunerXmlTemplates.xmlRmgDeviceIdentity, uuid, reporting_friendly_name, reporting_model, tuner_count, base_url, sb.ToString());
            return await Task.FromResult(tokenizedXml);
        }

        private Tuner GetIdleTuner()
        {
            var allTuners = GetTuners();
            return allTuners.First(tuner => tuner.ScanStatus == TunerStatus.Idle);
        }

        private List<Tuner> GetTuners(TunerStatus? scanStatus = null)
        {
            if (_tunerPool == null)
            {
                var tunerCount = 4;

                // int.TryParse(_configuration.GetSection("LOCAST_TUNERCOUNT").Value, out tunerCount);

                _tunerPool = new List<Tuner>();
                for (var i = 0; i < tunerCount; i++)
                {
                    _tunerPool.Add(new Tuner
                    {
                        ScanStatus = TunerStatus.Idle
                    });
                }
            }

            var tuners = scanStatus.HasValue
                    ? _tunerPool.Where(_ => _.ScanStatus == scanStatus).ToList() 
                    : _tunerPool;

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
