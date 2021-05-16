using System.Collections.Generic;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface IStationsService
    {
        Task GenerateDmaStationsAndChannelsFile(string dma);

        Task<Dictionary<long, Stuff>> GetDmaStationsAndChannels(string dma);

        Task RefreshDmaStationsAndChannels(string dma);

        Task<long> GetStationIdByChannel(string dma, decimal channel);
    }
}
