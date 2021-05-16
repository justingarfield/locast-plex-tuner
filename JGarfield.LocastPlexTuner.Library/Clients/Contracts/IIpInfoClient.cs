using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients.Contracts
{
    public interface IIpInfoClient
    {
        Task<string> GetPublicIpAddressAsync();
    }
}
