using System.Text;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public interface IChannelsM3UService
    {
        Task<string> GetChannelsM3U();
    }

    public class ChannelsM3UService : IChannelsM3UService
    {
        private const string FORMAT_DESCRIPTOR = "#EXTM3U";

        private const string RECORD_MARKER = "#EXTINF";

        private readonly IStationsService _stationsService;

        public ChannelsM3UService(IStationsService stationsService)
        {
            _stationsService = stationsService;
        }

        public async Task<string> GetChannelsM3U()
        {
            var base_url = "localhost:6077";
            var xmlTvUrl = $"http://{base_url}/xmltv.xml";

            var sb = new StringBuilder();
            sb.AppendLine($"{FORMAT_DESCRIPTOR} url-tvg=\"{xmlTvUrl}\" x-tvg-url=\"{xmlTvUrl}\"");

            var stations = await _stationsService.GetDmaStationsAndChannels("506");
            foreach (var station in stations)
            {
                var logoText = string.IsNullOrWhiteSpace(station.Value.logoUrl) ? string.Empty : $"tvg-logo=\"{station.Value.logoUrl}\"";
                sb.AppendLine($"{RECORD_MARKER}:-1 channelID=\"{station.Key}\" tvg-chno=\"{station.Value.channel}\" tvg-name=\"{station.Value.friendlyName}\" tvg-id=\"{station.Key}\" {logoText} group-title=\"LocastPlexTuner\",{station.Value.friendlyName}");
                sb.AppendLine($"http://{base_url}/watch/{station.Key}");
            }

            return sb.ToString();
        }
    }
}
