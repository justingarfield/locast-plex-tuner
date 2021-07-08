using System;
using System.IO;

namespace JGarfield.LocastPlexTuner.Library
{
    /// <summary>
    /// Provides application constants used throughout all capabilities.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The User-Agent string to use when making HTTP Requests using an <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        public const string DEFAULT_HTTPCLIENT_USERAGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36 Edg/90.0.818.56";
        
        /// <summary>
        /// Path to the root folder used for Application data storage.
        /// </summary>
        public static readonly string APPLICATION_DATA_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LocastPlexTuner");

        /// <summary>
        /// Path to any files that were the output of a data parsing process.
        /// </summary>
        public static readonly string APPLICATION_PARSED_FILES_PATH = Path.Combine(APPLICATION_DATA_PATH, "parsed");

        /// <summary>
        /// Path to any files that represent a local cache of data the Application uses.
        /// </summary>
        public static readonly string APPLICATION_CACHE_PATH = Path.Combine(APPLICATION_DATA_PATH, "cache");

        #region Need to clean these up and probably move them to TunerService or something

        public const string reporting_friendly_name = "LocastPlexTuner";

        public const string reporting_model = "lpt";

        public const string uuid = "xasudfds";

        public const string reporting_firmware_name = "locastplextuner";

        public const int tuner_count = 4;

        public const string reporting_firmware_ver = "v0.0.1";

        public const string tuner_type = "Antenna";

        #endregion Need to clean these up and probably move them to TunerService or something
    }
}
