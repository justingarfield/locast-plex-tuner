using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface IIpInfoService
    {
        Task<string> GetPublicIpAddressAsync();
    }
}
