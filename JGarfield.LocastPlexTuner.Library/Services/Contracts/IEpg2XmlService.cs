using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface IEpg2XmlService
    {
        Task WriteDummyXmlIfNotExists(string dma);

        Task GenerateEpgFile(string dma);

        Task<string> GetEpgFile(string dma);
    }
}
