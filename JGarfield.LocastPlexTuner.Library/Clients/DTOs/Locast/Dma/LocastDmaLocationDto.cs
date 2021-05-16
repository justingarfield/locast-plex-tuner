namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Dma
{
    /// <summary>
    /// DTO that matches the shape of the data returned from the Locast DMA resource endpoints.
    /// <br /><br />
    /// Note: Locast API models are case-sensitive and do not conform to any particular naming convention.
    /// </summary>
    public class LocastDmaLocationDto {
    
        /// <summary>
        /// The identifier of the DMA Location.
        /// </summary>
        public string DMA { get; set; }

        /// <summary>
        /// Whether or not the DMA Location is considered active.
        /// </summary>
        public bool active { get; set; }

        /// <summary>
        /// Any Announcements related to the DMA Location.
        /// 
        /// TODO: What sort of Announcements? What is the shape? Array of???? String?? Object??
        /// </summary>
        public string[] announcements { get; set; }

        /// <summary>
        /// A URI pointing to a 1920 x 1080 sized picture the represents the DMA Location.
        /// 
        /// TODO: (sizes may very? need to confirm)
        /// </summary>
        public string large_url { get; set; }

        /// <summary>
        /// The Latitude of the DMA Location.
        /// </summary>
        public double latitude { get; set; }

        /// <summary>
        /// The Longitude of the DMA Location.
        /// </summary>
        public double longitude { get; set; }

        /// <summary>
        /// Friendly name of the DMA Location (e.g. Boston)
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The Public IP Address of the machine / container that Locast sees running this library.
        /// </summary>
        public string publicIP { get; set; }

        /// <summary>
        /// A URI pointing to a 600 x 225 sized picture the reprensets the DMA Location.
        /// 
        /// TODO: (sizes may very? need to confirm)
        /// </summary>
        public string small_url { get; set; }
    }
}
