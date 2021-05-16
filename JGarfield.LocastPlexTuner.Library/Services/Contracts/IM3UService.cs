using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    /// <summary>
    /// Defines a contract for a class that implements capabilities for generating M3U files used throughout the application.
    /// <br /><br />
    /// For more information on what M3U files are, see: <see href="https://en.wikipedia.org/wiki/M3U" />
    /// </summary>
    public interface IM3UService
    {
        /// <summary>
        /// Generates the content required for an M3U file that holds entries for all available Channels within a particular DMA.
        /// </summary>
        /// <returns>The content required for an M3U file that holds entries for all available Channels within a particular DMA.</returns>
        Task<string> GetChannelsM3U();
    }
}
