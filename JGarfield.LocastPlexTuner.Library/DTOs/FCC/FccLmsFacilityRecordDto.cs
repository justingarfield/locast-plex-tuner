using System;

namespace JGarfield.LocastPlexTuner.Library.DTOs.FCC
{
    /// <summary>
    /// Represents a single FCC LMS GIS_FACILITY record.
    /// </summary>
    public class FccLmsFacilityRecordDto
    {
        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: This currently seems to always be 'Y' in the facility.dat file.
        /// </para>
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: This currently seems to always be 'N' / 'Y' / 'Blank'.
        /// </para>
        /// </summary>
        public bool? ATSC3 { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// TODO: Understand if this can be 'Blank' or not.
        /// </para>
        /// </summary>
        public string Callsign { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public DateTimeOffset? CallsignEffectiveDate { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public int? Channel { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: This currently seems to always be 'N' / 'Y' / 'Blank'.
        /// </para>
        /// </summary>
        public bool? ChannelSharing { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// TODO: Understand if this can be 'Blank' or not.
        /// </para>
        /// </summary>
        public string CommunityServedCity { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public string CommunityServedState { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// TODO: Understand if this can be 'Blank' or not.
        /// </para>
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public DateTimeOffset? ExpirationDate { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// TODO: Understand if this can be 'Blank' or not.
        /// </para>
        /// </summary>
        public int FacilityId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string FacilityStatus { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public string FacilityType { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// TODO: Understand if this can be 'Blank' or not.
        /// </para>
        /// </summary>
        public string FacilityUUID { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: This currently seems to always be '1', '2', '3', or 'Blank' in the facility.dat file.
        /// </para>
        /// </summary>
        public int? FacilityZoneCode { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public decimal? Frequency { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// TODO: Understand if this can be 'Blank' or not.
        /// </para>
        /// </summary>
        public DateTimeOffset LastUpdated { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public string LatestFilingVersionId { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// TODO: Understand if this can be 'Blank' or not.
        /// </para>
        /// </summary>
        public string LicenseFilingId { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public string NetworkAffiliation { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public string NielsenDmaRank { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public int? PrimaryStation { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public bool? SatelliteTv { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Never 'Blank'.
        /// </para>
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public char? StationType { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// TODO: Understand if this can be 'Blank' or not.
        /// </para>
        /// </summary>
        public DateTimeOffset StatusDate { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public int? TsidDtv { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public int? TsidNtsc { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: Can be 'Blank'.
        /// </para>
        /// </summary>
        public int? TvVirtualChannel { get; set; }

        /// <summary>
        /// <para>
        /// 
        /// </para>
        /// <para>
        /// RESEARCH: This currently seems to always be '^' in the facility.dat file.
        /// </para>
        /// </summary>
        public char EndOfRecord { get; set; }
    }
}
