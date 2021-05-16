namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Tuner
{
    public class LineupComplete
    {
        public bool ScanInProgress { get; set; }
        
        public bool ScanPossible { get; set; }
        
        public string Source { get; set; }

        public string[] SourceList { get; set; }
    }
}
