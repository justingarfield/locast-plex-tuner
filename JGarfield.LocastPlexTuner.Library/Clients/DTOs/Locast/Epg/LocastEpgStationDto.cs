using System.Collections.Generic;

namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Epg
{
    public class LocastEpgStationDto
    {
        public bool active { get; set; }

        public string callSign { get; set; }

        public int dma { get; set; }

        public long id { get; set; }

        public List<LocastEpgListingDto> listings { get; set; }

        public string logo226Url { get; set; }
        
        public string logoUrl { get; set; }
        
        public string name { get; set; }
        
        public int sequence { get; set; }

        public string stationId { get; set; }

        public long tivoId { get; set; }

        public int transcodeId { get; set; }
    }
}
