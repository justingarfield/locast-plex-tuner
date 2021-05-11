using System;
using TinyCsvParser.TypeConverter;

namespace JGarfield.LocastPlexTuner.Library.TinyCsvParser
{
    public class NullableCharConverter : ITypeConverter<char?>
	{
		public bool TryConvert(string value, out char? result)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				result = default;
				return true;
			}

			if (char.TryParse(value, out char parsedValue))
			{
				result = parsedValue;
				return true;
			}

			result = default;
			return false;
		}

		public Type TargetType
		{
			get { return typeof(char?); }
		}
	}

	public class CharConverter : ITypeConverter<char>
	{
		public bool TryConvert(string value, out char result)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				result = default;
				return true;
			}

			if (char.TryParse(value, out char parsedValue))
			{
				result = parsedValue;
				return true;
			}

			result = default;
			return false;
		}

		public Type TargetType
		{
			get { return typeof(char); }
		}
	}
}
