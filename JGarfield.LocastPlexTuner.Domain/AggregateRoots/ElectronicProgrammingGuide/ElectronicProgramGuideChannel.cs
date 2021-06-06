using System;

namespace JGarfield.LocastPlexTuner.Domain.AggregateRoots.ElectronicProgramGuide
{
    /// <summary>
    ///  Represents a Channel / Station in an Electronic Program Guide (EPG).
    /// </summary>
    public class ElectronicProgramGuideChannel
    {
        /// <summary>
        /// Unique identifier for the Channel / Station. (Locast calls it "StationId")
        /// </summary>
        public long Id { get; init; }

        /// <summary>
        /// The Channel Number and Callsign separated by spaces. (e.g. "5.1 ABC")
        /// </summary>
        public string DisplayName1 { get; init; }

        /// <summary>
        /// The Channel Number, Callsign, and Id separated by spaces. (e.g. "5.1 ABC 1571514243871")
        /// </summary>
        public string DisplayName2 { get; init; }

        /// <summary>
        /// The Channel Number (e.g. "5.1")
        /// </summary>
        public string DisplayName3 { get; init; }

        /// <summary>
        /// The Channel Number, Callsign, and the text "fcc" separated by spaces. (e.g. "5.1 ABC fcc")
        /// </summary>
        public string DisplayName4 { get; init; }

        /// <summary>
        /// The Channel Callsign (e.g. "ABC")
        /// </summary>
        public string DisplayName5 { get; init; }

        /// <summary>
        /// The Channel Friendly Name (e.g. "WBZDT")
        /// </summary>
        public string DisplayName6 { get; init; }

        /// <summary>
        /// The Channel Logo Uri.
        /// </summary>
        public Uri LogoUri { get; init; }
    }
}
