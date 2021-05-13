namespace JGarfield.LocastPlexTuner.Library.DTOs.Tuner
{
    public class Channel
    {
        /// <summary>
        /// This is currently always '0'.
        /// </summary>
        public static int Drm => 0;

        /// <summary>
        /// 
        /// </summary>
        public string ChannelIdentifier => $"id://{Number}";

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// This is currently always 'LocastPlexTuner'.
        /// </summary>
        public static string Origin => "LocastPlexTuner";
        
        /// <summary>
        /// The actual Channel Number (e.g. 27.2 or 4.1)
        /// </summary>
        public decimal Number { get; init; }

        /// <summary>
        /// This is currently always 'tv'.
        /// </summary>
        public static string Type => "tv";

        /// <summary>
        /// Default constructor for Xml Serialization.
        /// </summary>
        public Channel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        public Channel(string name, decimal number)
        {
            Name = name;
            Number = number;
        }
    }
}
