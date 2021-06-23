using System.Collections.Generic;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface IStationsService
    {
        Task GenerateDmaStationsAndChannelsFile();

        Task<Dictionary<long, EpgStationChannel>> GetDmaStationsAndChannels();

        Task RefreshDmaStationsAndChannels();

        Task<long> GetStationIdByChannel(decimal channel);
    }
}
