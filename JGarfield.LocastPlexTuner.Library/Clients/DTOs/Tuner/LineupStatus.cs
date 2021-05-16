namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Tuner
{
    public class LineupStatus
    {
        public bool ScanInProgress { get; set; }

        public int Progress { get; set; }

        public int Found { get; set; }

        public bool ScanPossible { get; set; }

        public string Source { get; set; }

        public string[] SourceList { get; set; }
    }
}
