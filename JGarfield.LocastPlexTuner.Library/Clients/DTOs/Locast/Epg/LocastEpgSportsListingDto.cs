namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Epg
{
    public class LocastEpgSportsListingDto
    {
        public string audioProperties { get; set; }

        public string description { get; set; }

        public int duration { get; set; }

        /// <summary>
        /// Episode | Sports | Show | Movie
        /// </summary>
        public string entityType { get; set; }

        public string eventTitle { get; set; }

        /// <summary>
        /// TODO: Need to investigate proper datatype
        /// </summary>
        public string gameDate { get; set; }

        /// <summary>
        /// TODO: Need to investigate proper datatype
        /// </summary>
        public string gameTime { get; set; }

        /// <summary>
        /// TODO: Need to investigate proper datatype
        /// </summary>
        public string gameTimeZone { get; set; }

        public string genres { get; set; }

        public bool hasImageArtwork { get; set; }

        public bool hasSeriesArtwork { get; set; }

        public bool isNew { get; set; }

        public LocastEpgOrganizationDto organization { get; set; }

        public string organizationId { get; set; }

        public string preferredImage { get; set; }

        public int preferredImageHeight { get; set; }

        public int preferredImageWidth { get; set; }

        public string programId { get; set; }

        public string seriesId { get; set; }

        public string shortDescription { get; set; }

        public string showType { get; set; }

        public long startTime { get; set; }

        public int stationId { get; set; }

        public string teams { get; set; }

        public string title { get; set; }

        public string videoProperties { get; set; }
    }
}
