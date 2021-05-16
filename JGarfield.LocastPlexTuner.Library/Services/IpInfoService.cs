using JGarfield.LocastPlexTuner.Library.Clients.Contracts;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
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
