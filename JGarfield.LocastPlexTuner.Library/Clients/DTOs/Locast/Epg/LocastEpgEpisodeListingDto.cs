namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Epg
{
    public class LocastEpgEpisodeListingDto
    {
        public long airdate { get; set; }

        public string audioProperties { get; set; }

        public string description { get; set; }

        public int duration { get; set; }

        public string entityType { get; set; }

        public int episodeNumber { get; set; }

        public string episodeTitle { get; set; }

        public string genres { get; set; }

        public bool hasImageArtwork { get; set; }

        public bool hasSeriesArtwork { get; set; }

        public bool isNew { get; set; }

        public string preferredImage { get; set; }

        public int preferredImageHeight { get; set; }

        public int preferredImageWidth { get; set; }

        public string programId { get; set; }

        public long releaseDate { get; set; }

        public int releaseYear { get; set; }

        public int seasonNumber { get; set; }

        public string seriesId { get; set; }

        public string shortDescription { get; set; }

        public string showType { get; set; }

        public long startTime { get; set; }

        public long stationId { get; set; }

        public string title { get; set; }

        public string topCast { get; set; }

        public string videoProperties { get; set; }
    }
}
