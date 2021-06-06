using JGarfield.LocastPlexTuner.Domain;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    /// <summary>
    /// Defines the requirements for a service that provides Designated Market Area (DMA) related capabilities.
    /// </summary>
    public interface IDmaService
    {
        Task<DmaLocation> GetDmaLocationAsync(string zipCode = null);
    }
}
