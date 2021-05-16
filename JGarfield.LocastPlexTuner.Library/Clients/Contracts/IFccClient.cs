using System;
using System.IO;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFccClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<Stream> GetLmsFacilityDbAsync(Uri uri);
    }
}
