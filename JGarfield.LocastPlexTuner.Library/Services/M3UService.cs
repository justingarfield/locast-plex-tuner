using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using System.Text;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    /// <summary>
    /// Provides capabilities for generating M3U files used throughout the application.
    /// <br /><br />
    /// For more information on what M3U files are, see: <see href="https://en.wikipedia.org/wiki/M3U" />
    /// </summary>
    public class M3UService : IM3UService
    {
        #region Private Members

        /// <summary>
        /// M3U <b>File Header</b> Directive.
        /// </summary>
        private const string M3U_FILE_HEADER_DIRECTIVE = "#EXTM3U";

        /// <summary>
        /// M3U <b>Track Information</b> Directive
        /// <br /><br />
        /// Also used for the <b>Additional Properties as Key-Value Pairs</b> <i>(IPTV)</i> Directive.
        /// </summary>
        private const string M3U_TRACK_INFORMATION_DIRECTIVE = "#EXTINF";

        /// <summary>
        /// The Stations Service instance used to pull all Stations and Channels for a particular DMA.
        /// </summary>
        private readonly IStationsService _stationsService;

        private readonly ApplicationContext _applicationContext;

        #endregion Private Members

        #region Constructor

        /// <summary>
        /// Instantiates a new <see cref="M3UService"/> using the provided <see cref="IStationsService"/> implementation.
        /// </summary>
        /// <param name="stationsService"></param>
        public M3UService(IStationsService stationsService, ApplicationContext applicationContext)
        {
            _stationsService = stationsService;
            _applicationContext = applicationContext;
        }

        #endregion Constructor

        #region Public Methods

        /// <inheritdoc />
        public async Task<string> GetChannelsM3U()
        {
            var base_url = "localhost:6077";
            var xmlTvUrl = $"http://{base_url}/xmltv.xml";

            var stations = await _stationsService.GetDmaStationsAndChannels(_applicationContext.DMA.DMA);

            var sb = new StringBuilder();
            sb.AppendLine($"{M3U_FILE_HEADER_DIRECTIVE} url-tvg=\"{xmlTvUrl}\" x-tvg-url=\"{xmlTvUrl}\"");

            foreach (var station in stations)
            {
                var logoText = string.IsNullOrWhiteSpace(station.Value.logoUrl) ? string.Empty : $"tvg-logo=\"{station.Value.logoUrl}\"";

                sb.AppendLine($"{M3U_TRACK_INFORMATION_DIRECTIVE}:-1 channelID=\"{station.Key}\" tvg-chno=\"{station.Value.channel}\" tvg-name=\"{station.Value.friendlyName}\" tvg-id=\"{station.Key}\" {logoText} group-title=\"LocastPlexTuner\",{station.Value.friendlyName}");
                sb.AppendLine($"http://{base_url}/watch/{station.Key}");
            }

            return sb.ToString();
        }

        #endregion Public Methods
    }
}
