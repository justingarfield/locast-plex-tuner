using System.Xml.Serialization;

namespace JGarfield.LocastPlexTuner.Library.Models
{
    [XmlRoot("channel")]
    public class ElectronicProgramGuideChannelModel
    {
        [XmlAttribute("id")]
        public string Id { get; init; }

        [XmlElement("display-name")]
        public string DisplayName1 { get; init; }

        [XmlElement("display-name")]
        public string DisplayName2 { get; init; }

        [XmlElement("display-name")]
        public string DisplayName3 { get; init; }

        [XmlElement("display-name")]
        public string DisplayName4 { get; init; }

        [XmlElement("display-name")]
        public string DisplayName5 { get; init; }

        [XmlElement("display-name")]
        public string DisplayName6 { get; init; }

        [XmlElement("icon")]
        public string Icon { get; init; }
    }
}
