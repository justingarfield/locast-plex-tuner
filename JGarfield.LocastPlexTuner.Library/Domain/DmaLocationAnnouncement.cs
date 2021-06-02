namespace JGarfield.LocastPlexTuner.Library.Domain
{
    public class DmaLocationAnnouncement
    {
        /// <summary>
        /// The message text of the announcement.
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// This was null the only time I've had this object populated.
        /// </summary>
        public string MessageHtml { get; set; }

        /// <summary>
        /// This was 'Incident' the only time I've had this object populated.
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// The Title of the Announcement (e.g. 'Boston Site Repair')
        /// </summary>
        public string Title { get; set; }
    }
}
