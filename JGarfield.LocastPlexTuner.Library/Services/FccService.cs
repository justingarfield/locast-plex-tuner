using JGarfield.LocastPlexTuner.Library.Clients.Contracts;
using JGarfield.LocastPlexTuner.Library.Clients.DTOs.FCC;
using JGarfield.LocastPlexTuner.Library.Domain;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using JGarfield.LocastPlexTuner.Library.TinyCsvParser;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    /// <summary>
    /// Provides capabilities that are FCC Licensing and Management System (LMS) Facilities Database related.
    /// <br /><br />
    /// FCC LMS Facilities Databases are the the databases that hold all information related to Station Callsigns, Channel Frequencies, 
    /// what Station/Channel Listings are active and licensed to operate, etc.
    /// <br /><br />
    /// For more information, see: <see href="https://enterpriseefiling.fcc.gov/dataentry/public/tv/lmsDatabase.html" />
    /// </summary>
    public class FccService : IFccService
    {
        #region Private Members

        /// <summary>
        /// The <see cref="ILogger{T}"/> implementation used for logging.
        /// </summary>
        private readonly ILogger<FccService> _logger;

        /// <summary>
        /// The <see cref="IFccClient"/> implementation used to make calls to the FCC LMS endpoints.
        /// </summary>
        private readonly IFccClient _fccClient;

        #endregion Private Members

        #region Constructor

        /// <summary>
        /// Instantiates a new <see cref="FccService"/> using the provided <see cref="ILogger{T}"/> and <see cref="IFccClient"/> implementations.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger{T}"/> implementation used for logging.</param>
        /// <param name="fccClient">The <see cref="IFccClient"/> implementation used to make calls to the FCC LMS endpoints.</param>
        public FccService(ILogger<FccService> logger, IFccClient fccClient) {
            _logger = logger;
            _fccClient = fccClient;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<FccStation>> GetFccStationsAsync()
        {
            if (LmsFacilityDbExists())
            {
                _logger.LogInformation("Found existing local copy of LMS Facility Database.");

                var localTimestamp = GetLocalLmsFacilityDbModifiedTime();
                var timeDifference = DateTimeOffset.UtcNow.Subtract(localTimestamp);

                // FCC LMS Database URIs don't allow HEAD requests anymore, so we can no longer use that to compare against.
                if (timeDifference.Hours > 24)
                {
                    _logger.LogInformation("Local copy of LMS Facility Database is out-dated.");
                    
                    await DownloadLmsFacilityDb();
                }
                else
                {
                    _logger.LogInformation("Local copy of LMS Facility Database is current.");
                }

                await ExtractLmsFacilityDbZipAsync();
                var results = await ParseLmsFacilityDbAsync();
                _logger.LogInformation($"Found {results.Count()} valid station records");
            }
            else
            {
                await DownloadLmsFacilityDb();
                await ExtractLmsFacilityDbZipAsync();
                var results = await ParseLmsFacilityDbAsync();
                _logger.LogWarning($"Found {results.Count()} valid station records");
            }
            
            return null;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task DownloadLmsFacilityDb()
        {
            _logger.LogDebug($"Entering {nameof(DownloadLmsFacilityDb)}");

            using (var fccLmsDbFileStream = File.Create(Path.Combine(Constants.APPLICATION_DOWNLOADED_FILES_PATH, "facility.zip")))
            {
                await _fccClient.FetchLmsFacilityDbAsync(fccLmsDbFileStream);
            }

            _logger.LogDebug($"Leaving {nameof(DownloadLmsFacilityDb)}");
        }

        private async Task ExtractLmsFacilityDbZipAsync()
        {
            var zipFilename = Path.Combine(Constants.APPLICATION_DOWNLOADED_FILES_PATH, "facility.zip");
            var extractionPath = Path.Combine(Constants.APPLICATION_EXTRACTED_FILES_PATH);

            var zipArchive = ZipFile.OpenRead(zipFilename);
            var zipArchiveEntry = zipArchive.GetEntry("facility.dat");

            var tmpFilename = Path.Combine(extractionPath, "facility.tmp");
            using (var zipArchiveEntryStream = zipArchiveEntry.Open())
            {
                using var fs = File.Create(Path.Combine(extractionPath, tmpFilename));
                await zipArchiveEntryStream.CopyToAsync(fs);
                await fs.FlushAsync();
            }

            File.Move(tmpFilename, Path.Combine(extractionPath, "facility.dat"), true);
        }

        private async Task<List<FccLmsFacilityRecordDto>> ParseLmsFacilityDbAsync()
        {
            var csvParserOptions = new CsvParserOptions(true, '|');
            var csvMapper = new FccLmsFacilityRecordMapping();
            var csvParser = new CsvParser<FccLmsFacilityRecordDto>(csvParserOptions, csvMapper);
            
            var extractionPath = Path.Combine(Constants.APPLICATION_DATA_PATH, "extracted/facility.dat");
            var csvMappingResults = csvParser
                                        .ReadFromFile(extractionPath, Encoding.ASCII)
                                        .ToList();

            var results = csvMappingResults
                    .Where(_ => 
                        _.IsValid 
                        && !string.IsNullOrWhiteSpace(_.Result.FacilityStatus) 
                        && _.Result.FacilityStatus.Equals("LICEN", StringComparison.InvariantCultureIgnoreCase)
                        && _.Result.ExpirationDate.HasValue
                        && _.Result.ExpirationDate.Value > DateTimeOffset.UtcNow
                        && (
                          //  _.Result.ServiceCode.Equals("DT", StringComparison.InvariantCultureIgnoreCase)
                          //  || _.Result.ServiceCode.Equals("TX", StringComparison.InvariantCultureIgnoreCase)
                            _.Result.ServiceCode.Equals("DTV", StringComparison.InvariantCultureIgnoreCase)
                          //  || _.Result.ServiceCode.Equals("TB", StringComparison.InvariantCultureIgnoreCase)
                          //  || _.Result.ServiceCode.Equals("LD", StringComparison.InvariantCultureIgnoreCase)
                          //  || _.Result.ServiceCode.Equals("DC", StringComparison.InvariantCultureIgnoreCase)
                        )
                    )
                    .Select(_ => _.Result)
                    .ToList();

            return await Task.FromResult(results);
        }

        private bool LmsFacilityDbExists()
        {
            var downloadsPath = Path.Combine(Constants.APPLICATION_DATA_PATH, "downloads");
            var filename = Path.Combine(downloadsPath, "facility.zip");
            return File.Exists(filename);
        }

        private DateTimeOffset GetLocalLmsFacilityDbModifiedTime()
        {
            var filename = Path.Combine(Constants.APPLICATION_DATA_PATH, "facility.zip");
            return File.GetLastWriteTimeUtc(filename);
        }

        #endregion Private Methods
    }
}
