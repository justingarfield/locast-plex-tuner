namespace JGarfield.LocastPlexTuner.Library.Metrics
{
    public interface IMetricsProvider<T>
    {
        public T GetMetrics();
    }
}
