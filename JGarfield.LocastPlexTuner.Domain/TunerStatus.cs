namespace JGarfield.LocastPlexTuner.Domain
{
    /// <summary>
    /// The different states a tuner instance can be in.
    /// </summary>
    public enum TunerStatus
    {
        /// <summary>
        /// The tuner is currently idle and awaiting use.
        /// </summary>
        Idle,

        /// <summary>
        /// The tuner is currently performing a channel scan.
        /// </summary>
        Scanning,

        /// <summary>
        /// The tuner is currently tuned to and streaming a particular channel.
        /// </summary>
        Streaming
    }
}
