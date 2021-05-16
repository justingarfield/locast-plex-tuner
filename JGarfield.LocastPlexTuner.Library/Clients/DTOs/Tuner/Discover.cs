namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Tuner
{
    public class Discover
    {
        public string FriendlyName { get; set; }
        
        public string Manufacturer { get; set; }
        
        public string ModelNumber { get; set; }

        public string FirmwareName { get; set; }
        
        public int TunerCount { get; set; }
        
        public string FirmwareVersion { get; set; }
        
        public string DeviceID { get; set; }

        public string DeviceAuth { get; set; } = "locastplextuner";
        
        public string BaseURL { get; set; }

        public string LineupURL { get; set; }
    }
}
