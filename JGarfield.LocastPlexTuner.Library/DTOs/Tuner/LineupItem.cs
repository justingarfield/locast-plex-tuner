using System;

namespace JGarfield.LocastPlexTuner.Library.DTOs.Tuner
{
    public class LineupItem
    {
        public LineupItem() { }

        /// <summary>
        /// The channel number that a guide should use for display.
        /// </summary>
        public string GuideNumber { get; set; }

        /// <summary>
        /// The friendly name of the station that a guide should use for display.
        /// </summary>
        public string GuideName { get; set; }

        /// <summary>
        /// The Uri to use if the consumer wishes to attempt streaming the station.
        /// </summary>
        public string URL { get; set; }
    }
}
