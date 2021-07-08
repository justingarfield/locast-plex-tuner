using System.Collections.Generic;

namespace JGarfield.LocastPlexTuner.Domain
{
    /// <summary>
    /// Designated Market Areas
    /// <br />
    /// See: <see href="https://www.thebalancecareers.com/what-is-a-designated-market-area-dma-2315180"/>
    /// </summary>
    public class DesignatedMarketArea
    {
        /// <summary>
        /// The identifier of the DMA Location.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Whether or not the DMA Location is considered active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// A URI pointing to a 1920 x 1080 sized picture the reprensets the DMA Location.
        /// 
        /// TODO: (sizes may very? need to confirm)
        /// </summary>
        public string LargeUrl { get; set; }

        /// <summary>
        /// The Latitude of the DMA Location.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The Longitude of the DMA Location.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Friendly name of the DMA Location (e.g. Boston)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Public IP Address of the machine / container that Locast sees running this library.
        /// </summary>
        public string PublicIP { get; set; }

        /// <summary>
        /// A URI pointing to a 600 x 225 sized picture the reprensets the DMA Location.
        /// 
        /// TODO: (sizes may very? need to confirm)
        /// </summary>
        public string SmallUrl { get; set; }

        /// <summary>
        /// Any Announcements related to the DMA Location.
        /// </summary>
        public List<DmaAnnouncement> Announcements { get; set; }
    }
}
