namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Dma
{
    /// <summary>
    /// Note: Locast API models are case-sensitive and do not conform to any particular naming convention.
    /// </summary>
    public class LocastDmaListingDto
    {
        public int id { get; set; }

        public string name { get; set; }

        public bool active { get; set; }

        public string imageSmallUrl { get; set; }

        public string imageLargeUrl { get; set; }

        public string lineupId { get; set; }

        public string tivoLineupId { get; set; }
    }
}
