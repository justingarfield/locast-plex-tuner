using JGarfield.LocastPlexTuner.Domain;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    /// <summary>
    /// Defines the requirements for a service that provides Designated Market Area (DMA) related capabilities.
    /// </summary>
    public interface IDmaService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="zipCode"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="dma"></param>
        /// <param name="forceLookup"></param>
        /// <returns></returns>
        Task<DmaLocation> GetDmaLocationAsync(string zipCode = null, double latitude = default, double longitude = default, string dma = null, bool forceLookup = false);
    }
}
