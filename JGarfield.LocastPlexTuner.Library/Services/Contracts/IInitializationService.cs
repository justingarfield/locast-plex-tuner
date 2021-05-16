using System.Threading;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface IInitializationService
    {
        void LogInitializationBanner();

        Task VerifyEnvironmentAsync(CancellationToken cancellationToken);

        Task InitializeEnvironmentAsync();
    }
}
