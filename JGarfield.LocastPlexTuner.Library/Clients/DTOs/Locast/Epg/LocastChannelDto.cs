using System.Collections.Generic;

namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Epg
{
    /// <summary>
    /// Represents a Channel in the Locast Electronic Programming Guide (EPG).
    /// </summary>
    public class LocastChannelDto
    {
        /// <summary>
        /// Whether or not the Channel is considered active for the DMA it's associated with.
        /// </summary>
        public bool active { get; set; }

        /// <summary>
        /// e.g. "2.2 WORLD" or "4.1 CBS"
        /// </summary>
        public string callSign { get; set; }

        /// <summary>
        /// The DMA that the Channel is associated with.
        /// </summary>
        public int dma { get; set; }

        /// <summary>
        /// Locast Unique Identifier of the Channel
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// Sometimes a 226px wide logo, but seems to vary channel-to-channel.
        /// </summary>
        public string logo226Url { get; set; }
        
        /// <summary>
        /// Another logo related to the Channel. Dimensions vary channel-to-channel.
        /// </summary>
        public string logoUrl { get; set; }

        /// <summary>
        /// The name of the Channel itself (e.g. "WBZDT" or "WGBHDT")
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// No idea how this gets assigned or what its value is.
        /// </summary>
        // TODO: Figure out what this id actually points to
        public int sequence { get; set; }

        /// <summary>
        /// No idea how this gets assigned or what its value is. Maybe FCC Station Id?
        /// </summary>
        // TODO: Figure out what this id actually points to
        public string stationId { get; set; }

        /// <summary>
        /// Honestly have no idea what this is for. Something for Tivo most likely.
        /// </summary>
        // TODO: Figure out what this id actually points to
        public long tivoId { get; set; }

        /// <summary>
        /// No idea what this is for.
        /// </summary>
        // TODO: Figure out what this id actually points to
        public int transcodeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<LocastProgramDto> listings { get; set; }
    }
}
