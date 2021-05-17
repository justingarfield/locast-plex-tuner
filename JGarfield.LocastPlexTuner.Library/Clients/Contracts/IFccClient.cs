using System;
using System.IO;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Clients.Contracts
{
    /// <summary>
    /// Defines a contract for a class that implements capabilities for accessing FCC Licensing and Management System (LMS) Facilities Database related information.
    /// <br /><br />
    /// For more information, see: <see href="https://enterpriseefiling.fcc.gov/dataentry/public/tv/lmsDatabase.html" />
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
