using JGarfield.LocastPlexTuner.Library.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace JGarfield.LocastPlexTuner.WebApi.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly ITunerService _tunerService;

        private readonly IStationsService _stationsService;

        private readonly IChannelsM3UService _channelsM3UService;
        
        private readonly IEpg2XmlService _epg2XmlService;

        public HomeController(ITunerService tunerService, IStationsService stationsService, IChannelsM3UService channelsM3UService, IEpg2XmlService epg2XmlService)
        {
            _tunerService = tunerService;
            _stationsService = stationsService;
            _channelsM3UService = channelsM3UService;
            _epg2XmlService = epg2XmlService;
        }

        [Route("/")]
        [HttpGet]
        public async Task<IActionResult> GetRmgIdentification()
        {
            var rmgIdentification = await _tunerService.GetRmgIdentification();
            return new ContentResult
            {
                Content = rmgIdentification,
                ContentType = "application/xml",
                StatusCode = 200
            };
        }

        [Route("/device.xml")]
        [HttpGet]
        public async Task<IActionResult> GetDeviceXml()
        {
            var xml = await _tunerService.GetDeviceXml();
            return new ContentResult
            {
                Content = xml,
                ContentType = "application/xml",
                StatusCode = 200
            };
        }

        [Route("/discover.json")]
        [HttpGet]
        public async Task<IActionResult> GetDiscoverJson()
        {
            var discoverJson = await _tunerService.GetDiscoverJson();
            return new ContentResult
            {
                Content = JsonSerializer.Serialize(discoverJson),
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        [Route("/lineup_status.json")]
        [HttpGet]
        public async Task<IActionResult> GetLineupStatusJson()
        {
            var json = await _tunerService.GetLineupStatusJsonAsync();
            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        [Route("/lineup.json")]
        [HttpGet]
        public async Task<IActionResult> GetLineupJson()
        {
            var channelLineup = await _tunerService.GetChannelLineupAsync();
            return new ContentResult
            {
                Content = JsonSerializer.Serialize(channelLineup),
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        [Route("/lineup.xml")]
        [HttpGet]
        public async Task<IActionResult> GetLineupXml()
        {
            var channelLineup = await _tunerService.GetChannelLineupAsync();
            string channelLineupText;

            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(channelLineup.GetType());
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            
            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, channelLineup, emptyNamespaces);
                channelLineupText = stream.ToString();
            }

            // Shitty hack for now, because I hate dealing w/ XML
            channelLineupText = channelLineupText.Replace("ArrayOfLineupItem", "Lineup");

            return new ContentResult
            {
                Content = channelLineupText,
                ContentType = "application/xml",
                StatusCode = 200
            };
        }

        [Route("/watch/{stationId}")]
        [HttpGet]
        public async Task<IActionResult> Watch(long stationId)
        {
            await _tunerService.DoTuning(stationId);
            return null;
        }

        [Route("/auto/v{stationId}")]
        [HttpGet]
        public async Task<IActionResult> AutoV(long stationId)
        {
            await _tunerService.DoTuning(stationId);
            return null;
        }

        [Route("/devices/{uuid}/media/{channelNo}")]
        [HttpGet]
        public async Task<IActionResult> DeviceTuning(string uuid, string channelNo)
        {
            channelNo = channelNo.Replace("id://", string.Empty).Replace("/", string.Empty);

            decimal.TryParse(channelNo, out decimal channel);

            var stationId = await _stationsService.GetStationIdByChannel("506", channel);

            await _tunerService.DoTuning(stationId);

            return null;
        }

        [Route("/xmltv.xml")]
        [HttpGet]
        public async Task<IActionResult> XmlTv()
        {
            var content = await _epg2XmlService.GetEpgFile();
            return new ContentResult
            {
                Content = content,
                ContentType = "application/xml",
                StatusCode = 200
            };
        }

        [Route("/channels.m3u")]
        [HttpGet]
        public async Task<IActionResult> ChannelsM3U()
        {
            var m3uFileContent = await _channelsM3UService.GetChannelsM3U();
            return new ContentResult
            {
                Content = m3uFileContent,
                ContentType = "application/vnd.apple.mpegurl",
                StatusCode = 200
            };
        }

        [Route("/devices/{uuid}")]
        [HttpGet]
        public async Task<IActionResult> RmgDeviceIdentity(string uuid)
        {
            var xml = await _tunerService.GetRmgDeviceIdentityXml();
            return new ContentResult
            {
                Content = xml,
                ContentType = "application/xml",
                StatusCode = 200
            };
        }

        [Route("/devices/{uuid}/channels")]
        [HttpGet]
        public async Task<IActionResult> RmgDeviceChannelItems(string uuid)
        {
            var xml = await _tunerService.GetRmgDeviceChannelItemsXml();
            return new ContentResult
            {
                Content = xml,
                ContentType = "application/xml",
                StatusCode = 200
            };
        }

        [Route("/devices/{uuid}/scanners")]
        [HttpGet]
        public async Task<IActionResult> RmgScanProviders(string uuid)
        {
            var xml = await _tunerService.GetRmgScanProvidersXml();
            return new ContentResult
            {
                Content = xml,
                ContentType = "application/xml",
                StatusCode = 200
            };
        }

        [Route("/lineup.post")]
        [HttpPost]
        public async Task<IActionResult> TunerScan(string scan)
        {
            if (scan == "start")
            {
                try
                {
                    await _tunerService.StartStationScan();

                    return Ok();
                }
                finally
                {
                    Response.OnCompleted(async () =>
                    {
                        await _stationsService.RefreshDmaStationsAndChannels("506");
                        await _tunerService.StopAllScanningForAllTuners();
                    });
                }
                
            }
            else if (scan == "abort")
            {
                await _tunerService.StopAllScanningForAllTuners();
            }
            else
            {
                throw new ApplicationException("Bad scan command yo!");
            }

            return null;
        }

        [Route("/devices/discover")]
        [Route("/devices/probe")]
        [HttpPost]
        public async Task<IActionResult> RmgDeviceDiscover()
        {
            var xml = await _tunerService.GetRmgDeviceDiscoverXml();
            return new ContentResult
            {
                Content = xml,
                ContentType = "application/xml",
                StatusCode = 200
            };
        }

        [Route("/devices/{uuid}/scan")]
        [HttpPost]
        public async Task<IActionResult> RmgScanStatus()
        {
            try
            {
                await _tunerService.StartStationScan();

                var xml = await _tunerService.GetRmgScanStatusXml();
                return new ContentResult
                {
                    Content = xml,
                    ContentType = "application/xml",
                    StatusCode = 200
                };
            }
            finally
            {
                Response.OnCompleted(async () =>
                {
                    await _stationsService.RefreshDmaStationsAndChannels("506");
                    await _tunerService.StopAllScanningForAllTuners();
                });
            }

            
        }

        [Route("/devices/{uuid}/scan")]
        [HttpDelete]
        public async Task<IActionResult> StopAllScanningForAllTuners(string uuid)
        {
            await _tunerService.StopAllScanningForAllTuners();
            return null;
        }
    }
}
