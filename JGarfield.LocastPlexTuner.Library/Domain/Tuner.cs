namespace JGarfield.LocastPlexTuner.Library.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class Tuner
    {
        /// <summary>
        /// 
        /// </summary>
        public TunerScanStatus ScanStatus { get; set; }

        /// <summary>
        /// The Channel that the Tuner is currently pointed to.
        /// </summary>
        public decimal CurrentChannel { get; set; }
    }
}
