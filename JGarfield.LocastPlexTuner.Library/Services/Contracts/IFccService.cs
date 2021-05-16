using JGarfield.LocastPlexTuner.Library.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    /// <summary>
    /// Defines a contract for a class that implements capabilities that are 
    /// FCC Licensing and Management System (LMS) Facilities Database related.
    /// <br /><br />
    /// FCC LMS Facilities Databases are the the databases that hold all information related to Station Callsigns, Channel Frequencies, 
    /// what Station/Channel Listings are active and licensed to operate, etc.
    /// <br /><br />
    /// For more information, see: <see href="https://enterpriseefiling.fcc.gov/dataentry/public/tv/lmsDatabase.html" />
    /// </summary>
    public interface IFccService
    {
        Task<IEnumerable<FccStation>> GetFccStationsAsync();
    }
}
