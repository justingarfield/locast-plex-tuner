using System;

namespace JGarfield.LocastPlexTuner.Domain
{
    /// <summary>
    /// Represents Stream Details provided when parsing Locast m3u8 metadata and finding the best available stream for a listing.
    /// </summary>
    public class StreamDetails
    {
        /// <summary>
        /// The Uri of the best available stream provided by Locast.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The Bitrate of the Locast stream specified in Uri.
        /// <br /><br />
        /// Note: This will be 0 if no Bitrate information was provided in the m3u8 metadata.
        /// </summary>
        public long BitrateInBytes { get; set; }
    }
}
