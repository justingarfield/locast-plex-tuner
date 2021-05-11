using JGarfield.LocastPlexTuner.Library.DTOs.Tuner;
using JGarfield.LocastPlexTuner.Library.Tuner;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using domain = JGarfield.LocastPlexTuner.Library.Domain;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public class TunerService : ITunerService
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

        private const string ffmpeg_path = "";

        private const long bytes_per_read = 1152000;

        private readonly ILogger<TunerService> _logger;

        private readonly IStationsService _stationsService;

        private readonly ILocastService _locastService;

        private bool _hdhr_station_scan = false;

        private readonly IConfiguration _configuration;

        private List<domain.Tuner> _availableTuners;

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion Private Members

        public TunerService(ILogger<TunerService> logger, IConfiguration configuration, IStationsService stationsService, ILocastService locastService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _stationsService = stationsService;
            _locastService = locastService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<string> GetRmgIdentification()
        {
            var tokenizedXml = string.Format(XmlTemplates.xmlRmgIdentification, reporting_friendly_name);
            return Task.FromResult(tokenizedXml);
        }

        public Task<string> GetRmgDeviceDiscoverXml()
        {
            var tokenizedXml = string.Format(XmlTemplates.xmlRmgDeviceDiscover, uuid, reporting_friendly_name, reporting_model, tuner_count, base_url);
            return Task.FromResult(tokenizedXml);
        }

        public Task<string> GetRmgScanStatusXml()
        {
            var tokenizedXml = string.Format(XmlTemplates.xmlRmgScanStatus);
            return Task.FromResult(tokenizedXml);
        }

        public Task<string> GetDeviceXml()
        {
            var tokenizedXml = string.Format(
                XmlTemplates.xmlDiscover,
                reporting_friendly_name,
                reporting_model,
                uuid,
                base_url
            );
            return Task.FromResult(tokenizedXml);
        }

        public Task<string> GetDiscoverJson()
        {
            string tokenizedJson;

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

            tokenizedJson = JsonSerializer.Serialize(discover);

            return Task.FromResult(tokenizedJson);
        }

        public Task<string> GetLineupStatusJsonAsync()
        {
            string tokenizedJson;

            // TODO: Handle hdhr_station_scan
            if (_hdhr_station_scan)
            {
                tokenizedJson = JsonSerializer.Serialize(new LineupStatus());
            }
            else
            {
                tokenizedJson = JsonSerializer.Serialize(new LineupComplete());
                tokenizedJson.Replace("Antenna", tuner_type);
            }

            return Task.FromResult(tokenizedJson);
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

            var idleTuner = await GetIdleTuner();

            if (idleTuner == null)
            {
                // TODO: Need to throw BadRequest here
                throw new ApplicationException("All tuners already in use.");
            }

            idleTuner.ScanStatus = domain.TunerScanStatus.Tuned;
            idleTuner.CurrentChannel = dmaStationsAndChannels[stationId].channel;

            var ffmpegBinary = @"D:\Utils\ffmpeg-4.4-full_build\bin\ffmpeg.exe";

            try
            {
                Console.WriteLine("\nTrying to launch ffmpeg session...");
                var processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = ffmpegBinary;
                processStartInfo.Arguments = $"-i {stationStreamUri} -c:v copy -c:a copy -f mpegts -nostats -hide_banner -loglevel warning pipe:1";

                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;

                var process = Process.Start(processStartInfo);
                
                byte[] videoData = new byte[1152000];
                var bytesRead = await process.StandardOutput.BaseStream.ReadAsync(videoData);

                while (true)
                {
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    else
                    {
                        await _httpContextAccessor.HttpContext.Response.Body.WriteAsync(videoData, 0, bytesRead);
                        
                        bytesRead = await process.StandardOutput.BaseStream.ReadAsync(videoData);
                    }
                }

                process.Close();

                idleTuner.ScanStatus = domain.TunerScanStatus.Idle;
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task StartStationScan()
        {
            var tuners = await GetAllTuners();
            var idleTuners = tuners.Where(_ => _.ScanStatus == domain.TunerScanStatus.Idle);

            foreach (var tuner in idleTuners)
            {
                tuner.ScanStatus = domain.TunerScanStatus.Scan;
            }

            _hdhr_station_scan = true;
        }

        public async Task StopAllScanningForAllTuners()
        {
            var tuners = await GetAllTuners();
            var scanningTuners = tuners.Where(_ => _.ScanStatus == domain.TunerScanStatus.Scan);

            foreach (var tuner in scanningTuners)
            {
                tuner.ScanStatus = domain.TunerScanStatus.Idle;
            }

            _hdhr_station_scan = false;
        }

        public Task<string> GetRmgScanProvidersXml()
        {
            var tokenizedXml = string.Format(XmlTemplates.xmlRmgScanProviders, "Boston");
            return Task.FromResult(tokenizedXml);
        }

        public async Task<string> GetRmgDeviceChannelItemsXml()
        {
            var dmaStationsAndChannels = await _stationsService.GetDmaStationsAndChannels("506");
            
            var sb = new StringBuilder();

            foreach (var dmaStationsAndChannel in dmaStationsAndChannels)
            {
                var tokenizedXml = string.Format(XmlTemplates.xmlRmgDeviceChannelItem, dmaStationsAndChannel.Value.channel, dmaStationsAndChannel.Value.friendlyName);
                sb.Append(tokenizedXml);
            }
            var builtString = sb.ToString();

            return string.Format(XmlTemplates.xmlRmgDeviceChannels, dmaStationsAndChannels.Count, builtString);
        }

        public async Task<string> GetRmgDeviceIdentityXml()
        {
            var allTuners = await GetAllTuners();
            var sb = new StringBuilder();
            string tokenizedXml;
            for (var i = 0; i < allTuners.Count; i++)
            {
                if (allTuners[i].ScanStatus == domain.TunerScanStatus.Idle)
                {
                    tokenizedXml = string.Format(XmlTemplates.xmlRmgTunerIdle, i);
                } 
                else if (allTuners[i].ScanStatus == domain.TunerScanStatus.Scan)
                {
                    tokenizedXml = string.Format(XmlTemplates.xmlRmgTunerScanning, i);
                } 
                else
                {
                    tokenizedXml = string.Format(XmlTemplates.xmlRmgTunerStreaming, i, allTuners[i].CurrentChannel);
                }

                sb.Append(tokenizedXml);
            }

            tokenizedXml = string.Format(XmlTemplates.xmlRmgDeviceIdentity, uuid, reporting_friendly_name, reporting_model, tuner_count, base_url, sb.ToString());
            return tokenizedXml;
        }

        private async Task<domain.Tuner> GetIdleTuner()
        {
            var allTuners = await GetAllTuners();
            return allTuners.First(tuner => tuner.ScanStatus == domain.TunerScanStatus.Idle);
        }

        private async Task<List<domain.Tuner>> GetAllTuners()
        {
            if (_availableTuners == null)
            {
                var tunerCount = 4;

                int.TryParse(_configuration.GetSection("LOCAST_TUNERCOUNT").Value, out tunerCount);

                _availableTuners = new List<domain.Tuner>();
                for (var i = 0; i <= tunerCount; i++)
                {
                    _availableTuners.Add(new domain.Tuner
                    {
                        ScanStatus = domain.TunerScanStatus.Idle
                    });
                }
            }

            return await Task.FromResult(_availableTuners);
        }
    }
}
