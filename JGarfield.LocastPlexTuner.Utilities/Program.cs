using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Utilities
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await EPGScrapeParser.Parse();
        }
    }
}
