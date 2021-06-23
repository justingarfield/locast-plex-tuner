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
        /// <summary>
        /// Performs a lookup for a specific Designated Market Area (DMA).
        /// </summary>
        /// <param name="dma">The unique identifier of the DMA to lookup.</param>
        /// <returns>A DTO representing a DMA (if one was found).</returns>
        Task<LocastDmaLocationDto> GetDmaAsync(string dma);

        /// <summary>
        /// Performs a lookup for a Designated Market Area (DMA) by Zip Code.
        /// </summary>
        /// <param name="zipCode">The Zip Code to use when performing the DMA lookup.</param>
        /// <returns>A DTO representing a DMA (if one was found).</returns>
        Task<LocastDmaLocationDto> GetDmaByZipCodeAsync(string zipCode);

        /// <summary>
        /// Performs a lookup for a Designated Market Area (DMA) by IP Address.
        /// </summary>
        /// <param name="ipAddress">The IP Address to use when performing the DMA lookup.</param>
        /// <returns>A DTO representing a DMA (if one was found).</returns>
        Task<LocastDmaLocationDto> GetDmaByIpAddressAsync(string ipAddress);

        /// <summary>
        /// Performs a lookup for a Designated Market Area (DMA) by Longitude and Latitude.
        /// </summary>
        /// <param name="latitude">The Latitude to use when performing the DMA lookup.</param>
        /// <param name="longitude">The Longitude to use when performing the DMA lookup.</param>
        /// <returns>A DTO representing a DMA (if one was found).</returns>
        Task<LocastDmaLocationDto> GetDmaByLatLongAsync(double latitude, double longitude);

        /// <summary>
        /// Retrieves the User Details for the currently logged-in User (whichever account was provided in the configuration steps).
        /// <br /><br />
        /// Note: This is currently used during startup to ensure that the User is currently donating, to avoid headaches with Locast advertising.
        /// </summary>
        /// <returns>A DTO representing the User Details for the currently logged-in User.</returns>
        Task<LocastUserDetailsDto> GetUserDetails();

        /// <summary>
        /// Retrieves the Electronic Programming Guide (EPG) channels and listings for a particular Designated Market Area (DMA).
        /// </summary>
        /// <param name="dma">The DMA to retrieve the EPG for.</param>
        /// <param name="startTime">Where in the timeline of the EPG data to start pulling listings.</param>
        /// <param name="hours">How many hours of listings to pull (only used if a startTime is provided).</param>
        /// <returns>EPG channels and listings for a particular Designated Market Area (DMA)</returns>
        Task<List<LocastChannelDto>> GetEpgForDmaAsync(string dma, DateTimeOffset? startTime = null, int? hours = 0);

        Task<LocastStationDto> GetStationAsync(long stationId, double latitude, double longitude);

        Task<string> GetM3U8FileAsString(Uri uri);
    }
}
