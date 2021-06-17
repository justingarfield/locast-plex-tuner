using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Epg;
using JGarfield.LocastPlexTuner.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface ILocastService
    {
        Task<bool> IsActivelyDonatingUserAsync();

        Task<List<LocastChannelDto>> GetEpgStationsForDmaAsync(string dma, DateTimeOffset? startTime = null);

        Task<StreamDetails> GetStationStreamUri(long stationId);
    }
}
