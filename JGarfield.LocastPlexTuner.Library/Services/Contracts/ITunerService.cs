using JGarfield.LocastPlexTuner.Library.Clients.DTOs.Tuner;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface ITunerService
    {
        Task<string> GetRmgIdentification();

        Task<string> GetDeviceXml();

        Task<Discover> GetDiscoverAsync();

        Task<LineupStatus> GetLineupStatusAsync();

        Task<List<LineupItem>> GetChannelLineupAsync();

        Task TuneToStation(long stationId, CancellationToken cancellationToken);

        Task StopAllScanningForAllTuners();

        /// <summary>
        /// Used during a clean shutdown to stop any actively open streams attached to a tuner.
        /// </summary>
        /// <returns></returns>
        Task StopAllActiveStreams();

        Task StartStationScan();

        Task<string> GetRmgDeviceDiscoverXml();

        Task<string> GetRmgScanStatusXml();

        Task<string> GetRmgScanProvidersXml();

        Task<MediaContainer> GetRmgDeviceChannelItemsXml();

        Task<string> GetRmgDeviceIdentityXml();
    }
}
