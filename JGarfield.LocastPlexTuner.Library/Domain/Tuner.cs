using JGarfield.LocastPlexTuner.Library.Metrics;

namespace JGarfield.LocastPlexTuner.Library.Domain
{
    /// <summary>
    /// Represents a "virtual" Tuner Card.
    /// </summary>
    public class Tuner : IMetricsProvider<TunerMetrics>
    {
        private long BytesStreamedThisSession { get; set; }

        private long BytesStreamedTotal { get; set; }

        /// <summary>
        /// The Channel that the Tuner is currently pointed to.
        /// </summary>
        public decimal CurrentChannel { get; set; }

        /// <summary>
        /// The tuner's current state.
        /// </summary>
        public TunerStatus Status { get; private set; } = TunerStatus.Idle;

        /// <summary>
        /// Sets the tuner's state to <see cref="TunerStatus.Idle"/>.
        /// </summary>
        public void SetScanStatusToIdle()
        {
            Status = TunerStatus.Idle;
        }

        /// <summary>
        /// Sets the tuner's state to <see cref="TunerStatus.Scanning"/>.
        /// </summary>
        public void SetScanStatusToScanning()
        {
            Status = TunerStatus.Scanning;
        }

        /// <summary>
        /// Sets the tuner's state to <see cref="TunerStatus.Streaming"/>.
        /// </summary>
        public void SetScanStatusToTuned()
        {
            Status = TunerStatus.Streaming;
        }

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
