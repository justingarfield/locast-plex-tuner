using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface IEpg2XmlService
    {
        Task GenerateEpgFile();

        Task<string> GetEpgFile();
    }
}
