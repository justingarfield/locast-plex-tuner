using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JGarfield.LocastPlexTuner.Library.Metrics
{
    public class CollectionOfTunerMetrics
    {
        private readonly ReadOnlyCollection<TunerMetrics> _tunerMetrics;

        public int NumberOfAvailableTuners => _tunerMetrics.Count;

        public int NumberOfTunersInUse => _tunerMetrics.Where(_ => _.BytesRelayed == 1).Count();

        public CollectionOfTunerMetrics(List<TunerMetrics> tunerMetrics)
        {
            _tunerMetrics = tunerMetrics.AsReadOnly();
        }
    }
}
