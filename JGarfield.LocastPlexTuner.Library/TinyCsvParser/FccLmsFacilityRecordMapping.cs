using JGarfield.LocastPlexTuner.Library.DTOs.FCC;
using TinyCsvParser.Mapping;

namespace JGarfield.LocastPlexTuner.Library.TinyCsvParser
{
    public class FccLmsFacilityRecordMapping : CsvMapping<FccLmsFacilityRecordDto>
    {
        public FccLmsFacilityRecordMapping()
        {
            MapProperty(0, x => x.Active, new BoolConverter());
            MapProperty(1, x => x.ATSC3, new NullableBoolConverter());
            MapProperty(2, x => x.Callsign);
            MapProperty(3, x => x.CallsignEffectiveDate, new NullableDateTimeOffsetConverter());
            MapProperty(4, x => x.Channel);
            MapProperty(5, x => x.ChannelSharing, new NullableBoolConverter());
            MapProperty(6, x => x.CommunityServedCity);
            MapProperty(7, x => x.CommunityServedState);
            MapProperty(8, x => x.Created, new DateTimeOffsetConverter());
            MapProperty(9, x => x.ExpirationDate, new NullableDateTimeOffsetConverter());
            MapProperty(10, x => x.FacilityId);
            MapProperty(11, x => x.FacilityStatus);
            MapProperty(12, x => x.FacilityType);
            MapProperty(13, x => x.FacilityUUID);
            MapProperty(14, x => x.FacilityZoneCode);
            MapProperty(15, x => x.Frequency);
            MapProperty(16, x => x.LastUpdated, new DateTimeOffsetConverter());
            MapProperty(17, x => x.LatestFilingVersionId);
            MapProperty(18, x => x.LicenseFilingId);
            MapProperty(19, x => x.NetworkAffiliation);
            MapProperty(20, x => x.NielsenDmaRank);
            MapProperty(21, x => x.PrimaryStation);
            MapProperty(22, x => x.SatelliteTv, new NullableBoolConverter());
            MapProperty(23, x => x.ServiceCode);
            MapProperty(24, x => x.StationType, new NullableCharConverter());
            MapProperty(25, x => x.StatusDate, new DateTimeOffsetConverter());
            MapProperty(26, x => x.TsidDtv);
            MapProperty(27, x => x.TsidNtsc);
            MapProperty(28, x => x.TvVirtualChannel);
            MapProperty(29, x => x.EndOfRecord, new CharConverter());
        }
    }
}
