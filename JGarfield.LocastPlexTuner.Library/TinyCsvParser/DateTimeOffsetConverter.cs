using System;
using TinyCsvParser.TypeConverter;

namespace JGarfield.LocastPlexTuner.Library.TinyCsvParser
{
    public class NullableDateTimeOffsetConverter : ITypeConverter<DateTimeOffset?>
    {
		public bool TryConvert(string value, out DateTimeOffset? result)
		{
			if (string.IsNullOrWhiteSpace(value))
            {
				result = null;
				return true;
            }

			if (DateTimeOffset.TryParse(value, out DateTimeOffset parsedValue))
			{
				result = parsedValue;
				return true;
			}

			result = null;
			return false;
		}

		public Type TargetType
		{
			get { return typeof(DateTimeOffset?); }
		}
	}

	public class DateTimeOffsetConverter : ITypeConverter<DateTimeOffset>
	{
		public bool TryConvert(string value, out DateTimeOffset result)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				result = default;
				return true;
			}

			if (DateTimeOffset.TryParse(value, out DateTimeOffset parsedValue))
			{
				result = parsedValue;
				return true;
			}

			result = default;
			return false;
		}

		public Type TargetType
		{
			get { return typeof(DateTimeOffset); }
		}
	}
}
