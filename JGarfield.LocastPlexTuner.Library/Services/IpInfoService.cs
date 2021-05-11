using JGarfield.LocastPlexTuner.Library.Clients;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public interface IIpInfoService
    {
        Task<string> GetPublicIpAddressAsync();
    }

    public class IpInfoService : IIpInfoService
    {
        private IIpInfoClient _ipInfoClient;

        public IpInfoService(IIpInfoClient ipInfoClient)
        {
            _ipInfoClient = ipInfoClient;
        }

        public async Task<string> GetPublicIpAddressAsync()
        {
            return await _ipInfoClient.GetPublicIpAddressAsync();
        }
    }
}
