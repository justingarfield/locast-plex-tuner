using JGarfield.LocastPlexTuner.Library.Clients;
using JGarfield.LocastPlexTuner.Library.Domain;
using JGarfield.LocastPlexTuner.Library.DTOs.FCC;
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
    public interface IFccService
    {
        Task<IEnumerable<FccStation>> GetFccStationsAsync();
    }

    public class FccService : IFccService
    {
        #region Private Members

        private readonly ILogger<FccService> _logger;

        private readonly IFccClient _fccClient;

        #endregion Private Members

        #region Constructor

        public FccService(ILogger<FccService> logger, IFccClient fccClient) {
            _logger = logger;
            _fccClient = fccClient;
        }

        #endregion Constructor

        #region Public Methods

        public async Task<IEnumerable<FccStation>> GetFccStationsAsync()
        {
            if (LmsFacilityDbExists())
            {
                _logger.LogInformation("Found existing local copy of LMS Facility Database.");

                var localTimestamp = GetLocalLmsFacilityDbModifiedTime();
                var timeDifference = DateTimeOffset.UtcNow.Subtract(localTimestamp);

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
                _logger.LogWarning($"Found {results.Count()} valid station records");
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

        private Uri GetLmsFacilityDbUri()
        {
            var now = DateTimeOffset.UtcNow;
            var date = now.ToString(Constants.FCC_LMS_FACILITY_DB_DATEFORMAT);

            // The FCC LMS Facilities URI conforms to the following pattern: https://enterpriseefiling.fcc.gov/dataentry/api/download/dbfile/05-09-2021/facility.zip
            var lmsFacilityDbUri = new Uri(Constants.FCC_LMS_BASE_URI, $"download/dbfile/{date}/{Constants.FCC_LMS_FACILITY_DB_FILENAME}");

            return lmsFacilityDbUri;
        }

        private async Task DownloadLmsFacilityDb()
        {
            _logger.LogDebug($"Entering {nameof(DownloadLmsFacilityDb)}");

            var uri = GetLmsFacilityDbUri();

            var stream = await _fccClient.GetLmsFacilityDbAsync(uri);

            var filename = Path.Combine(Constants.APPLICATION_DATA_PATH, "downloads/facility.zip");

            await using var fs = File.Create(filename);

            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fs);

            _logger.LogDebug($"Leaving {nameof(DownloadLmsFacilityDb)}");
        }

        private async Task ExtractLmsFacilityDbZipAsync()
        {
            var zipFilename = Path.Combine(Constants.APPLICATION_DATA_PATH, "downloads/facility.zip");
            var extractionPath = Path.Combine(Constants.APPLICATION_DATA_PATH, "extracted");

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
