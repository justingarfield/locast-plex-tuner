using JGarfield.LocastPlexTuner.Library.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface IFccService
    {
        Task<IEnumerable<FccStation>> GetFccStationsAsync();
    }
}
