using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public interface IStationsService
    {
        Task GenerateDmaStationsAndChannelsFile(string dma);

        Task<Dictionary<long, Stuff>> GetDmaStationsAndChannels(string dma);

        Task RefreshDmaStationsAndChannels(string dma);

        Task<long> GetStationIdByChannel(string dma, decimal channel);
    }

    public class StationsService : IStationsService
    {
        private readonly IFccService _fccService;

        private readonly ILocastService _locastService;

        public StationsService(IFccService fccService, ILocastService locastService)
        {
            _locastService = locastService;
            _fccService = fccService;
        }

        public async Task<long> GetStationIdByChannel(string dma, decimal channel)
        {
            var stations = await GetDmaStationsAndChannels(dma);
            return stations.FirstOrDefault(_ => _.Value.channel == channel).Key;
        }

        public async Task RefreshDmaStationsAndChannels(string dma)
        {
            await _fccService.GetFccStationsAsync();
            await GenerateDmaStationsAndChannelsFile(dma);
        }

        public async Task GenerateDmaStationsAndChannelsFile(string dma)
        {
            var epgStations = await _locastService.GetEpgStationsForDmaAsync(dma);

            var finalizedEpgStations = new Dictionary<long, Stuff>();

            foreach (var epgStation in epgStations)
            {
                var thing = new Stuff();
                thing.callSign = epgStation.name;

                if (!string.IsNullOrWhiteSpace(epgStation.logo226Url))
                {
                    thing.logoUrl = epgStation.logo226Url;
                }
                else if (!string.IsNullOrWhiteSpace(epgStation.logoUrl))
                {
                    thing.logoUrl = epgStation.logoUrl;
                }

                if (decimal.TryParse(epgStation.callSign.Split()[0], out decimal channel))
                {
                    thing.channel = channel;
                    thing.friendlyName = epgStation.callSign.Split()[1];
                    finalizedEpgStations.Add(epgStation.id, thing);
                    continue;
                }

                // TODO: Finish implementing parser logic. Didn't need for my use-cases
                // DetectCallSign(epgStation.callSign);
                // DetectCallSign(epgStation.name);

                finalizedEpgStations.Add(epgStation.id, thing);
            }

            var filePath = Path.Combine(Constants.APPLICATION_PARSED_FILES_PATH, $"dma{dma}_stations.json");
            using (var fs = File.Create(filePath))
            {
                await JsonSerializer.SerializeAsync<Dictionary<long, Stuff>>(fs, finalizedEpgStations);
                await fs.FlushAsync();
            }
        }

        public async Task<Dictionary<long, Stuff>> GetDmaStationsAndChannels(string dma)
        {
            var filePath = Path.Combine(Constants.APPLICATION_PARSED_FILES_PATH, $"dma{dma}_stations.json");

            Dictionary<long, Stuff> result;
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                result = await JsonSerializer.DeserializeAsync<Dictionary<long, Stuff>>(fs);
            }

            return result;
        }
    }
}
