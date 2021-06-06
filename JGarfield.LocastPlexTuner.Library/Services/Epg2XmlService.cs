using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Epg;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public class Epg2XmlService : IEpg2XmlService
    {
        /// <summary>
        /// 
        /// </summary>
        private const int RELEASE_YEAR_CUTOFF = 1800;

        private readonly IStationsService _stationsService;

        private readonly ILocastService _locastService;
        
        public Epg2XmlService(IStationsService stationsService, ILocastService locastService)
        {
            _stationsService = stationsService;
            _locastService = locastService;
        }

        public async Task WriteDummyXmlIfNotExists(string dma)
        {
            var xmlDoc = CreateNewRootXmlDocument();

            var tvElement = CreateNewTvXmlElement(xmlDoc);
            xmlDoc.AppendChild(tvElement);

            var filename = Path.Combine(Constants.APPLICATION_CACHE_PATH, $"{dma}_epg.xml");
            xmlDoc.Save(filename);

            await Task.CompletedTask;
        }

        public async Task<string> GetEpgFile(string dma)
        {
            var filename = Path.Combine(Constants.APPLICATION_CACHE_PATH, $"{dma}_epg.xml");
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);
            
            return await Task.FromResult(xmlDoc.OuterXml);
        }

        public async Task GenerateEpgFile(string dma)
        {
            var dmaChannels = await _stationsService.GetDmaStationsAndChannels(dma);

            var todaysDate = DateTimeOffset.UtcNow;
            var datesToPull = new List<DateTimeOffset> { todaysDate, todaysDate.AddDays(1), todaysDate.AddDays(2) };

            await RemoveStaleCache(todaysDate);

            var xmlDoc = CreateNewRootXmlDocument();

            var tvElement = CreateNewTvXmlElement(xmlDoc);
            xmlDoc.AppendChild(tvElement);

            var channelsDone = false;
            foreach (var dateToPull in datesToPull)
            {
                var epgStations = await GetCached(dateToPull, dma);

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
                            tvElement.AppendChild(channelElement);
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
                            var startTime = DateTimeOffset.FromUnixTimeSeconds(listing.startTime / 1000);
                            int durationInSeconds = listing.duration;
                            var endTime = startTime.AddSeconds(durationInSeconds);
                            
                            var programmeElement = xmlDoc.CreateElement("programme");
                            programmeElement.SetAttribute("start", startTime.ToString("yyyyMMddHHmmss +0000"));
                            programmeElement.SetAttribute("stop", endTime.ToString("yyyyMMddHHmmss +0000"));
                            programmeElement.SetAttribute("channel", $"{sid}");

                            if (!string.IsNullOrWhiteSpace(listing.title))
                            {
                                var titleElement = xmlDoc.CreateElement("title");
                                titleElement.SetAttribute("lang", "en");
                                titleElement.InnerText = listing.title;
                                programmeElement.AppendChild(titleElement);
                            }

                            var listingGenres = new string[0];
                            if (!string.IsNullOrWhiteSpace(listing.genres))
                            {
                                listingGenres = listing.genres.Split(',');
                            }

                            if (listingGenres.Any(_ => _.Equals("movies", StringComparison.InvariantCultureIgnoreCase)) 
                                && listing.releaseYear > RELEASE_YEAR_CUTOFF)
                            {
                                var subTitleElement = xmlDoc.CreateElement("sub-title");
                                subTitleElement.SetAttribute("lang", "en");
                                subTitleElement.InnerText = $"Movie: {listing.releaseYear}";
                                programmeElement.AppendChild(subTitleElement);
                            }
                            else if (!string.IsNullOrWhiteSpace(listing.episodeTitle))
                            {
                                var subTitleElement = xmlDoc.CreateElement("sub-title");
                                subTitleElement.SetAttribute("lang", "en");
                                subTitleElement.InnerText = $"{listing.episodeTitle}";
                                programmeElement.AppendChild(subTitleElement);
                            }

                            if (string.IsNullOrWhiteSpace(listing.description))
                            {
                                listing.description = "Unavailable";
                            }

                            var descElement = xmlDoc.CreateElement("desc");
                            descElement.SetAttribute("lang", "en");
                            descElement.InnerText = $"{listing.description}";
                            programmeElement.AppendChild(descElement);

                            var lengthElement = xmlDoc.CreateElement("length");
                            lengthElement.SetAttribute("units", "minutes");
                            lengthElement.InnerText = $"{listing.duration}";
                            programmeElement.AppendChild(lengthElement);

                            foreach (var genre in listingGenres)
                            {
                                var categoryElement = xmlDoc.CreateElement("category");
                                descElement.SetAttribute("lang", "en");
                                categoryElement.InnerText = $"{genre.Trim()}";
                                programmeElement.AppendChild(categoryElement);

                                var genreElement = xmlDoc.CreateElement("genre");
                                descElement.SetAttribute("lang", "en");
                                genreElement.InnerText = $"{genre.Trim()}";
                                programmeElement.AppendChild(genreElement);
                            }
                            
                            if (!string.IsNullOrWhiteSpace(listing.preferredImage))
                            {
                                var iconElement = xmlDoc.CreateElement("icon");
                                iconElement.SetAttribute("src", $"{listing.preferredImage}");
                                programmeElement.AppendChild(iconElement);
                            }

                            if (string.IsNullOrWhiteSpace(listing.rating))
                            {
                                listing.rating = "N/A";
                            }

                            var ratingElement = xmlDoc.CreateElement("rating");
                            var valueElement = xmlDoc.CreateElement("value");
                            valueElement.InnerText = $"{listing.rating}";
                            ratingElement.AppendChild(valueElement);
                            programmeElement.AppendChild(ratingElement);

                            // TODO: Could 'Specials' be 0?
                            if (listing.seasonNumber > 0 && listing.episodeNumber > 0)
                            {
                                var paddedSeasonNumber = listing.seasonNumber.ToString("00");
                                var paddedEpisodeNumber = listing.seasonNumber.ToString("00");

                                var episodeNumElement = xmlDoc.CreateElement("episode-num");
                                episodeNumElement.SetAttribute("system", "common");
                                episodeNumElement.InnerText = $"S{paddedSeasonNumber}E{paddedEpisodeNumber}";
                                programmeElement.AppendChild(episodeNumElement);

                                episodeNumElement = xmlDoc.CreateElement("episode-num");
                                episodeNumElement.SetAttribute("system", "xmltv_ns");
                                episodeNumElement.InnerText = $"{listing.seasonNumber - 1}.{listing.episodeNumber - 1}.0";
                                programmeElement.AppendChild(episodeNumElement);

                                episodeNumElement = xmlDoc.CreateElement("episode-num");
                                episodeNumElement.SetAttribute("system", "SxxExx");
                                episodeNumElement.InnerText = $"S{paddedSeasonNumber}E{paddedEpisodeNumber}";
                                programmeElement.AppendChild(episodeNumElement);
                            }

                            if (listing.isNew)
                            {
                                var newElement = xmlDoc.CreateElement("new");
                                programmeElement.AppendChild(newElement);
                            }

                            tvElement.AppendChild(programmeElement);
                        }
                    }
                }
            }

            var filename = Path.Combine(Constants.APPLICATION_CACHE_PATH, $"{dma}_epg.xml");
            xmlDoc.Save(filename);
        }

        private async Task<List<LocastChannelDto>> GetCached(DateTimeOffset cacheFileDate, string dma)
        {
            var dateString = cacheFileDate.ToString("yyyy-MM-dd");
            var cachePath = Path.Combine(Constants.APPLICATION_CACHE_PATH, $"{dateString}.json");
            
            if (File.Exists(cachePath))
            {
                using (var fs = File.Open(cachePath, FileMode.Open, FileAccess.Read))
                {
                    var locastEpgStations = await JsonSerializer.DeserializeAsync<List<LocastChannelDto>>(fs);
                    return locastEpgStations;
                }
            }
            else
            {
                var locastEpgStations = await _locastService.GetEpgStationsForDmaAsync(dma, cacheFileDate);

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
            channelElement.SetAttribute("id", $"{stationId}");

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
