using JGarfield.LocastPlexTuner.Library.Metrics;

namespace JGarfield.LocastPlexTuner.Library.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class Tuner : IMetricsProvider<TunerMetrics>
    {
        private long BytesStreamedThisSession { get; set; }

        private long BytesStreamedTotal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TunerStatus ScanStatus { get; set; }

        /// <summary>
        /// The Channel that the Tuner is currently pointed to.
        /// </summary>
        public decimal CurrentChannel { get; set; }

        #region Metrics Related

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TunerMetrics GetMetrics()
        {
            return new TunerMetrics
            {
                
            };
        }

        #endregion Metrics Related
    }
}
