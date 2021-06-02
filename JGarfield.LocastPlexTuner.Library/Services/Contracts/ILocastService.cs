using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Epg;
using JGarfield.LocastPlexTuner.Library.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface ILocastService
    {
        Task<DmaLocation> GetDmaLocationAsync(string zipCode = null);

        Task<bool> IsActivelyDonatingUserAsync();

        Task<List<LocastChannelDto>> GetEpgStationsForDmaAsync(string dma, DateTimeOffset? startTime = null);

        Task<Uri> GetStationStreamUri(long stationId);
    }
}
