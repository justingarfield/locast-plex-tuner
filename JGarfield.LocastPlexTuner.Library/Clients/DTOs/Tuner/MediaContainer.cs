using System.Collections.Generic;

namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Tuner
{
    /// <summary>
    /// 
    /// </summary>
    public class MediaContainer
    {
        /// <summary>
        /// The number of Channels that the MediaContainer element holds.
        /// </summary>
        public int Size
        {
            get {
                return Channels.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Channel> Channels { get; }

        /// <summary>
        /// Default Constructor for Xml Serialization.
        /// </summary>
        public MediaContainer()
        {
        }

        /// <summary>
        /// Constructor that initializes the MediaContainer with a list of channels.
        /// </summary>
        /// <param name="size"></param>
        public MediaContainer(List<Channel> channels)
        {
            Channels = channels;
        }
    }
}
