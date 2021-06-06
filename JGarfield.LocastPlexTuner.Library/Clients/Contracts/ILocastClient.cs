using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Dma;
using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Epg;
using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Station;
using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients.Contracts
{
    public interface ILocastClient
    {
        Task<LocastDmaLocationDto> GetDmaByZipCodeAsync(string zipCode);

        Task<LocastDmaLocationDto> GetDmaByIpAddressAsync(string ipAddress);

        Task<LocastDmaLocationDto> GetDmaByLatLongAsync(double latitude, double longitude);

        Task<LocastUserDetailsDto> GetUserDetails();

        Task<List<LocastChannelDto>> GetEpgForDmaAsync(string dma, DateTimeOffset? startTime = null, int? hours = 0);

        Task<LocastStationDto> GetStationAsync(long stationId, double latitude, double longitude);

        Task<string> GetM3U8FileAsString(Uri uri);
    }
}
