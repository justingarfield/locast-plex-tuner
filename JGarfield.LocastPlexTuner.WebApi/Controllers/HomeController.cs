using JGarfield.LocastPlexTuner.Library;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace JGarfield.LocastPlexTuner.WebApi.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly ITunerService _tunerService;

        private readonly IStationsService _stationsService;

        private readonly IM3UService _m3uService;
        
        private readonly IEpg2XmlService _epg2XmlService;
        
        private readonly ApplicationContext _applicationContext;

        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public HomeController(ITunerService tunerService, IStationsService stationsService, IM3UService m3uService, IEpg2XmlService epg2XmlService, ApplicationContext applicationContext)
        {
            _tunerService = tunerService;
            _stationsService = stationsService;
            _m3uService = m3uService;
            _epg2XmlService = epg2XmlService;
            _applicationContext = applicationContext;
        }

        [Route("/")]
        [HttpGet]
        public async Task<IActionResult> GetRmgIdentification()
        {
            var rmgIdentification = await _tunerService.GetRmgIdentification();
            return Ok(rmgIdentification);

            /*return new ContentResult
            {
                Content = rmgIdentification,
                ContentType = "application/xml",
                StatusCode = 200
            };*/
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
        public async Task<IActionResult> GetDiscoverAsync()
        {
            var discoverJson = await _tunerService.GetDiscoverAsync();
            return new JsonResult(discoverJson, jsonSerializerOptions);
        }

        [Route("/lineup_status.json")]
        [HttpGet]
        public async Task<IActionResult> GetLineupStatusAsync()
        {
            var json = await _tunerService.GetLineupStatusAsync();
            return new JsonResult(json, jsonSerializerOptions);
        }

        [Route("/lineup.json")]
        [HttpGet]
        public async Task<IActionResult> GetChannelLineupJsonAsync()
        {
            var channelLineup = await _tunerService.GetChannelLineupAsync();
            return new JsonResult(channelLineup, jsonSerializerOptions);
        }

        [Route("/lineup.xml")]
        [HttpGet]
        public async Task<IActionResult> GetChannelLineupXmlAsync()
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
        public async Task Watch(long stationId)
        {
            /*
            var filePath = @"c:\temp\file.mpg"; // Your path to the audio file.
            var bufferSize = 1024;
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);
            return File(fileStream, "audio/mpeg");
            */
            await _tunerService.DoTuning(stationId);
        }

        [Route("/auto/v{stationId}")]
        [HttpGet]
        public async Task AutoV(long stationId)
        {
            await _tunerService.DoTuning(stationId);
        }

        [Route("/devices/{uuid}/media/{channelNo}")]
        [HttpGet]
        public async Task DeviceTuning(string uuid, string channelNo)
        {
            channelNo = channelNo.Replace("id://", string.Empty).Replace("/", string.Empty);

            decimal.TryParse(channelNo, out decimal channel);

            var stationId = await _stationsService.GetStationIdByChannel(_applicationContext.CurrentDMA.DMA, channel);

            await _tunerService.DoTuning(stationId);
        }

        [Route("/xmltv.xml")]
        [HttpGet]
        public async Task<IActionResult> XmlTv()
        {
            var content = await _epg2XmlService.GetEpgFile(_applicationContext.CurrentDMA.DMA);
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
            var m3uFileContent = await _m3uService.GetChannelsM3U();
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
            var mediaContainerWithChannels = await _tunerService.GetRmgDeviceChannelItemsXml();
            string mediaContainerWithChannelsText;

            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(mediaContainerWithChannels.GetType());
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, mediaContainerWithChannels, emptyNamespaces);
                mediaContainerWithChannelsText = stream.ToString();
            }

            var xDoc = XDocument.Parse(mediaContainerWithChannelsText.ToString());
            foreach (var x in xDoc.Descendants("Channels").Reverse())
            {
                x.AddAfterSelf(x.Nodes());
                x.Remove();
            }
            
            return new ContentResult
            {
                Content = xDoc.ToString(),
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
                        await _stationsService.RefreshDmaStationsAndChannels(_applicationContext.CurrentDMA.DMA);
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
                return BadRequest("Unrecognized scan command.");
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
                    await _stationsService.RefreshDmaStationsAndChannels(_applicationContext.CurrentDMA.DMA);
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
