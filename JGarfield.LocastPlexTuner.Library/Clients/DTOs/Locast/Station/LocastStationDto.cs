namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Station
{
    public class LocastStationDto
    {
        public long id { get; set; }

        public int dma { get; set; }

        public string stationId { get; set; }
  
        public string name { get; set; }
  
        public string callSign { get; set; }
  
        public string logoUrl { get; set; }
  
        public bool active { get; set; }

        public string streamUrl { get; set; }

        public int sequence { get; set; }

        public string logo226Url { get; set; }

        public long tivoId { get; set; }

        public int transcodeId { get; set; }
    }
}
