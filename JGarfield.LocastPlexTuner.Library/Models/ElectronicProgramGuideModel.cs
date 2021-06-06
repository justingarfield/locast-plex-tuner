using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace JGarfield.LocastPlexTuner.Library.Models
{
    /// <summary>
    /// Represents an Electronic Program Guide (EPG).
    /// <br /><br />
    /// See: <see href="https://en.wikipedia.org/wiki/Electronic_program_guide"/>
    /// </summary>
    [XmlRoot("tv")]
    public class ElectronicProgramGuideModel
    {
        [XmlAttribute("source-info-url")]
        public string SourceInfoName { get; init; }

        [XmlAttribute("source-info-url")]
        public Uri SourceInfoUri { get; init; }

        [XmlAttribute("generator-info-url")]
        public string GeneratorInfoName { get; init; }

        [XmlAttribute("generator-info-url")]
        public Uri GeneratorInfoUri { get; init; }


        public List<ElectronicProgramGuideChannelModel> Channels { get; init; }
    }
}
