using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace JGarfield.LocastPlexTuner.Library.Models
{
    /// <summary>
    /// Represents an Electronic Program Guide (EPG).
    /// <br /><br />
    /// See: <see href="https://en.wikipedia.org/wiki/Electronic_program_guide"/>
    /// <br />
    /// See also: <seealso href="https://github.com/XMLTV/xmltv/blob/master/xmltv.dtd"/>
    /// </summary>
    [XmlRoot("tv")]
    public class ElectronicProgramGuideModel
    {
        [XmlAttribute("date")]
        public DateTimeOffset DateListingsWereProduced { get; init; }

        [XmlAttribute("source-info-url")]
        public Uri SourceInfoUri { get; init; }

        [XmlAttribute("source-info-name")]
        public string SourceInfoName { get; init; }

        [XmlAttribute("source-data-url")]
        public Uri SourceDataUri { get; init; }

        [XmlAttribute("generator-info-name")]
        public string GeneratorInfoName { get; init; }

        [XmlAttribute("generator-info-url")]
        public Uri GeneratorInfoUri { get; init; }


        public List<ElectronicProgramGuideChannelModel> Channels { get; init; }
    }
}
