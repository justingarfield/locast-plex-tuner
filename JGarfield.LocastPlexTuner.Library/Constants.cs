using System;
using System.IO;

namespace JGarfield.LocastPlexTuner.Library
{
    public static class Constants
    {
        public const string DEFAULT_USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36 Edg/90.0.818.56";
        
        public static readonly Uri LOCAST_API_BASE_URI = new Uri("https://api.locastnet.org/api/");

        public static readonly Uri IPINFO_API_BASE_URI = new Uri("https://ipinfo.io/ip");

        public static readonly Uri FCC_LMS_BASE_URI = new Uri("https://enterpriseefiling.fcc.gov/dataentry/api/");

        public const string FCC_LMS_FACILITY_DB_DATEFORMAT = "MM-dd-yyyy";

        public const string FCC_LMS_FACILITY_DB_FILENAME = "facility.zip";

        public static readonly string APPLICATION_DATA_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LocastPlexTuner");

        public static readonly string APPLICATION_DOWNLOADED_FILES_PATH = Path.Combine(APPLICATION_DATA_PATH, "downloads");

        public static readonly string APPLICATION_EXTRACTED_FILES_PATH = Path.Combine(APPLICATION_DATA_PATH, "extracted");

        public static readonly string APPLICATION_PARSED_FILES_PATH = Path.Combine(APPLICATION_DATA_PATH, "parsed");

        public static readonly string APPLICATION_CACHE_PATH = Path.Combine(APPLICATION_DATA_PATH, "cache");
    }
}
