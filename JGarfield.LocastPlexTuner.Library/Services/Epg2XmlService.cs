using JGarfield.LocastPlexTuner.Library.DTOs.Locast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public interface IEpg2XmlService
    {
        Task WriteDummyXmlIfNotExists();

        Task GenerateEpgFile();
    }

    public class Epg2XmlService : IEpg2XmlService
    {
        private const string DMA = "506";

        private readonly IStationsService _stationsService;

        private readonly ILocastService _locastService;

        public Epg2XmlService(IStationsService stationsService, ILocastService locastService)
        {
            _stationsService = stationsService;
            _locastService = locastService;
        }

        public async Task WriteDummyXmlIfNotExists()
        {
            var xmlDoc = CreateNewRootXmlDocument();

            var tvElement = CreateNewTvXmlElement(xmlDoc);
            xmlDoc.AppendChild(tvElement);

            var filename = Path.Combine(Constants.APPLICATION_CACHE_PATH, $"{DMA}_epg.xml");
            xmlDoc.Save(filename);
        }

        public async Task GenerateEpgFile()
        {
            var dmaChannels = await _stationsService.GetDmaStationsAndChannels(DMA);

            var todaysDate = DateTimeOffset.UtcNow;
            var datesToPull = new List<DateTimeOffset> { todaysDate, todaysDate.AddDays(1), todaysDate.AddDays(2) };

            await RemoveStaleCache(todaysDate);

            var xmlDoc = CreateNewRootXmlDocument();

            var tvElement = CreateNewTvXmlElement(xmlDoc);
            xmlDoc.AppendChild(tvElement);

            var channelsDone = false;
            foreach (var dateToPull in datesToPull)
            {
                var epgStations = await GetCached(dateToPull);

                if (!channelsDone)
                {
                    foreach (var epgStation in epgStations)
                    {
                        var sid = epgStation.id;

                        if (dmaChannels.ContainsKey(sid))
                        {
                            var channelNumber = dmaChannels[sid].channel;
                            var channelRealName = dmaChannels[sid].friendlyName;
                            var channelCallSign = dmaChannels[sid].callSign;

                            var channelLogo = string.Empty;
                            if (!string.IsNullOrWhiteSpace(epgStation.logo226Url))
                            {
                                channelLogo = epgStation.logo226Url;
                            }
                            else if (!string.IsNullOrWhiteSpace(epgStation.logoUrl))
                            {
                                channelLogo = epgStation.logoUrl;
                            }

                            var channelElement = CreateNewChannelXmlElement(xmlDoc, sid, channelNumber, channelCallSign, channelRealName, channelLogo);
                            xmlDoc.AppendChild(channelElement);
                        }
                    }

                    channelsDone = true;
                }

                foreach (var epgStation in epgStations)
                {
                    var sid = epgStation.id;

                    if (dmaChannels.ContainsKey(sid))
                    {
                        var channelNumber = dmaChannels[sid].channel;
                        var channelRealName = dmaChannels[sid].friendlyName;
                        var channelCallSign = dmaChannels[sid].callSign;

                        var channelLogo = string.Empty;
                        if (!string.IsNullOrWhiteSpace(epgStation.logo226Url))
                        {
                            channelLogo = epgStation.logo226Url;
                        }
                        else if (!string.IsNullOrWhiteSpace(epgStation.logoUrl))
                        {
                            channelLogo = epgStation.logoUrl;
                        }

                        foreach (var listing in epgStation.listings) {

                        }
                    }

                }
            }

            var filename = Path.Combine(Constants.APPLICATION_CACHE_PATH, $"{DMA}_epg.xml");
            xmlDoc.Save(filename);
        }

        private async Task<List<LocastEpgStationDto>> GetCached(DateTimeOffset cacheFileDate)
        {
            var dateString = cacheFileDate.ToString("yyyy-mm-dd");
            var cachePath = Path.Combine(Constants.APPLICATION_CACHE_PATH, "{dateString}.json");
            
            if (File.Exists(cachePath))
            {
                using (var fs = File.Open(cachePath, FileMode.Open, FileAccess.Read))
                {
                    var locastEpgStations = await JsonSerializer.DeserializeAsync<List<LocastEpgStationDto>>(fs);
                    return locastEpgStations;
                }
            }
            else
            {
                var locastEpgStations = await _locastService.GetEpgStationsForDmaAsync(DMA, cacheFileDate);

                using (var fs = File.Create(cachePath))
                {
                    await JsonSerializer.SerializeAsync(fs, locastEpgStations);
                    await fs.FlushAsync();
                }

                return locastEpgStations;
            }
        }

        private XmlDocument CreateNewRootXmlDocument()
        {
            var xmlDocument = new XmlDocument();

            var xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            var rootElement = xmlDocument.DocumentElement;
            xmlDocument.InsertBefore(xmlDeclaration, rootElement);

            return xmlDocument;
        }

        private XmlElement CreateNewTvXmlElement(XmlDocument xmlDoc)
        {
            XmlElement tvElement = xmlDoc.CreateElement(string.Empty, "tv", string.Empty);
            tvElement.SetAttribute("source-info-url", "https://www.locast.org");
            tvElement.SetAttribute("source-info-name", "locast.org");
            tvElement.SetAttribute("generator-info-name", "locastepg");
            tvElement.SetAttribute("generator-info-url", "github.com/justingarfield/LocastPlexTuner");
            return tvElement;
        }

        private XmlElement CreateNewChannelXmlElement(XmlDocument xmlDoc, long stationId, decimal channelNumber, string channelCallsign, string channelRealName, string channelLogo)
        {
            XmlElement channelElement = xmlDoc.CreateElement(string.Empty, "channel", string.Empty);

            var displayNameElement = xmlDoc.CreateElement(string.Empty, "display-name", string.Empty);
            displayNameElement.InnerText = $"{channelNumber} {channelCallsign}";
            channelElement.AppendChild(displayNameElement);

            displayNameElement = xmlDoc.CreateElement(string.Empty, "display-name", string.Empty);
            displayNameElement.InnerText = $"{channelNumber} {channelCallsign} {stationId}";
            channelElement.AppendChild(displayNameElement);

            displayNameElement = xmlDoc.CreateElement(string.Empty, "display-name", string.Empty);
            displayNameElement.InnerText = $"{channelNumber}";
            channelElement.AppendChild(displayNameElement);

            displayNameElement = xmlDoc.CreateElement(string.Empty, "display-name", string.Empty);
            displayNameElement.InnerText = $"{channelNumber} {channelCallsign} fcc";
            channelElement.AppendChild(displayNameElement);

            displayNameElement = xmlDoc.CreateElement(string.Empty, "display-name", string.Empty);
            displayNameElement.InnerText = $"{channelCallsign}";
            channelElement.AppendChild(displayNameElement);

            displayNameElement = xmlDoc.CreateElement(string.Empty, "display-name", string.Empty);
            displayNameElement.InnerText = $"{channelRealName}";
            channelElement.AppendChild(displayNameElement);

            if (!string.IsNullOrWhiteSpace(channelLogo))
            {
                displayNameElement = xmlDoc.CreateElement(string.Empty, "icon", string.Empty);
                displayNameElement.SetAttribute("src", channelLogo);
                channelElement.AppendChild(displayNameElement);
            }

            return channelElement;
        }

        private async Task RemoveStaleCache(DateTimeOffset todaysDate)
        {
            var existingCacheFiles = Directory.GetFiles(Constants.APPLICATION_CACHE_PATH);

            foreach (var existingCacheFile in existingCacheFiles)
            {
                var filename = Path.GetFileName(existingCacheFile);
                filename = filename.Replace(".json", "");
                DateTimeOffset.TryParse(filename, out DateTimeOffset cacheDate);
                
                if (cacheDate >= DateTimeOffset.UtcNow)
                {
                    continue;
                }
                else
                {
                    File.Delete(existingCacheFile);
                }

            }
            

            await Task.CompletedTask;
        }
    }
}
