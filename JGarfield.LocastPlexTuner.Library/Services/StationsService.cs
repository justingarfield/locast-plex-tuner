using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public class StationsService : IStationsService
    {
        private Dictionary<long, EpgStationChannel> _cachedEpgStationChannels;

        private readonly ILocastService _locastService;

        private StationsService()
        {
            _cachedEpgStationChannels = new Dictionary<long, EpgStationChannel>();
        }

        public StationsService(ILocastService locastService) : this()
        {
            _locastService = locastService;
        }

        public async Task<long> GetStationIdByChannel(string dma, decimal channel)
        {
            var stations = await GetDmaStationsAndChannels(dma);
            return stations.FirstOrDefault(_ => _.Value.channel == channel).Key;
        }

        public async Task RefreshDmaStationsAndChannels(string dma)
        {
            // await RefreshStationCache();
            await GenerateDmaStationsAndChannelsFile(dma);
        }

        public async Task GenerateDmaStationsAndChannelsFile(string dma)
        {
            
            var epgStations = await _locastService.GetEpgStationsForDmaAsync(dma);

            foreach (var epgStation in epgStations)
            {
                var epgStationChannel = new EpgStationChannel();
                epgStationChannel.callSign = epgStation.name;

                if (!string.IsNullOrWhiteSpace(epgStation.logo226Url))
                {
                    epgStationChannel.logoUrl = epgStation.logo226Url;
                }
                else if (!string.IsNullOrWhiteSpace(epgStation.logoUrl))
                {
                    epgStationChannel.logoUrl = epgStation.logoUrl;
                }

                if (decimal.TryParse(epgStation.callSign.Split()[0], out decimal channel))
                {
                    epgStationChannel.channel = channel;
                    epgStationChannel.friendlyName = epgStation.callSign.Split()[1];
                    _cachedEpgStationChannels.Add(epgStation.id, epgStationChannel);
                    continue;
                }

                _cachedEpgStationChannels.Add(epgStation.id, epgStationChannel);
            }

            var filePath = Path.Combine(Constants.APPLICATION_PARSED_FILES_PATH, $"dma{dma}_stations.json");
            using (var fs = File.Create(filePath))
            {
                await JsonSerializer.SerializeAsync(fs, _cachedEpgStationChannels);
                await fs.FlushAsync();
            }
        }

        public async Task<Dictionary<long, EpgStationChannel>> GetDmaStationsAndChannels(string dma)
        {
            var filePath = Path.Combine(Constants.APPLICATION_PARSED_FILES_PATH, $"dma{dma}_stations.json");

            Dictionary<long, EpgStationChannel> result;
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                result = await JsonSerializer.DeserializeAsync<Dictionary<long, EpgStationChannel>>(fs);
            }

            return result;
        }
    }
}
