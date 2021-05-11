using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.TypeConverter;

namespace JGarfield.LocastPlexTuner.Library.TinyCsvParser
{
    public class NullableBoolConverter : ITypeConverter<bool?>
	{
		public bool TryConvert(string value, out bool? result)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				result = false;
				return true;
			}

			if (bool.TryParse(value, out bool parsedValue))
			{
				result = parsedValue;
				return true;
			}

			var trimmedValue = value.Trim();
			if (trimmedValue.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
				result = true;
				return true;
			}

			if (trimmedValue.Equals("N", StringComparison.InvariantCultureIgnoreCase))
			{
				result = false;
				return true;
			}

			result = false;
			return false;
		}

		public Type TargetType
		{
			get { return typeof(bool?); }
		}
	}

	public class BoolConverter : ITypeConverter<bool>
	{
		public bool TryConvert(string value, out bool result)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				result = false;
				return true;
			}

			if (bool.TryParse(value, out bool parsedValue))
			{
				result = parsedValue;
				return true;
			}

			var trimmedValue = value.Trim();
			if (trimmedValue.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
			{
				result = true;
				return true;
			}

			if (trimmedValue.Equals("N", StringComparison.InvariantCultureIgnoreCase))
			{
				result = false;
				return true;
			}

			result = false;
			return false;
		}

		public Type TargetType
		{
			get { return typeof(bool); }
		}
	}
}
