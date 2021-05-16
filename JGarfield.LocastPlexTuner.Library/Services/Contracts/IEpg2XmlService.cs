using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface IEpg2XmlService
    {
        Task WriteDummyXmlIfNotExists();

        Task GenerateEpgFile();

        Task<string> GetEpgFile();
    }
}
