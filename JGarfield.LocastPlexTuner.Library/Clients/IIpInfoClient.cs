using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients
{
    public interface IIpInfoClient
    {
        Task<string> GetPublicIpAddressAsync();
    }
}
