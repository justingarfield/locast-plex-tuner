using JGarfield.LocastPlexTuner.Library.DTOs.Tuner;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public interface ITunerService
    {
        Task<string> GetRmgIdentification();

        Task<string> GetDeviceXml();

        Task<Discover> GetDiscoverAsync();

        Task<LineupStatus> GetLineupStatusAsync();

        Task<List<LineupItem>> GetChannelLineupAsync();

        Task DoTuning(long stationId);

        Task StopAllScanningForAllTuners();

        Task StartStationScan();

        Task<string> GetRmgDeviceDiscoverXml();

        Task<string> GetRmgScanStatusXml();

        Task<string> GetRmgScanProvidersXml();

        Task<MediaContainer> GetRmgDeviceChannelItemsXml();

        Task<string> GetRmgDeviceIdentityXml();
    }
}
