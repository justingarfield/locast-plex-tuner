using JGarfield.LocastPlexTuner.Library.DTOs.Locast;
using JGarfield.LocastPlexTuner.Library.DTOs.Locast.Station;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients
{
    public interface ILocastClient
    {
        Task<LocastDmaLocationDto> GetLocationByZipCodeAsync(string zipCode);

        Task<LocastDmaLocationDto> GetLocationByIpAddressAsync(string ipAddress);

        Task<LocastDmaLocationDto> GetLocationByLatLongAsync(double latitude, double longitude);

        Task<LocastUserDetailsDto> GetUserDetails();

        Task<List<LocastEpgStationDto>> GetEpgForDmaAsync(string dma, DateTimeOffset? startTime = null);

        Task<LocastStationDto> GetStationAsync(long stationId, double latitude, double longitude);

        Task<string> GetM3U8FileAsString(Uri uri);
    }
}
