namespace JGarfield.LocastPlexTuner.Library.Clients.DTOs.Locast.Dma
{
    /// <summary>
    /// 
    /// </summary>
    public class LocastDmaAnnouncement
    {
        /// <summary>
        /// The message text of the announcement.
        /// </summary>
        public string messageText { get; set; }

        /// <summary>
        /// This was null the only time I've had this object populated.
        /// </summary>
        public string messageHtml { get; set; }

        /// <summary>
        /// This was 'Incident' the only time I've had this object populated.
        /// </summary>
        public string messageType { get; set; }

        /// <summary>
        /// The Title of the Announcement (e.g. 'Boston Site Repair')
        /// </summary>
        public string title { get; set; }
    }
}
