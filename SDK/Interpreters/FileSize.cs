using System;
using System.Collections.Generic;
using System.Globalization;

namespace Auditor.SDK.Interpreters
{
	public static class SizeExtensions
	{
		public static long ToFileSize(this string str) => new SizeConverter().Parse(str);

		public static long ToBitRate(this string str) => new SizeConverter
		{
			Units = new Dictionary<string, long>
			{
				{ "", 1000L }, // Default to KB
				{ "B", 1L },
				{ "BPS", 1L },
				{ "KB", 1000L },
				{ "KBPS", 1000L },
				{ "MB", 1000L * 1000L },
				{ "MBPS", 1000L * 1000L },
				{ "GB", 1000L * 1000L * 1000L },
				{ "GBPS", 1000L * 1000L * 1000L },
				{ "TB", 1000L * 1000L * 1000L * 1000L },
				{ "TBPS", 1000L * 1000L * 1000L * 1000L }
			}
		}.Parse(str);
	}

	public class SizeConverter
	{
		public NumberFormatInfo NumberFormat { get; set; } = new CultureInfo("en-US").NumberFormat;

		public Dictionary<string, long> Units { get; set; } = new Dictionary<string, long>
		{
			{ "", 1L }, // Default to bytes
			{ "B", 1L },
			{ "KB", 1024L },
			{ "MB", 1024L * 1024L },
			{ "GB", 1024L * 1024L * 1024L },
			{ "TB", 1024L * 1024L * 1024L * 1024L }
		};

		public long Parse(string value)
		{
			value = value.Trim();

			var unit = ExtractUnit(value);
			var sizeAsString = value.Substring(0, value.Length - unit.Length).Trim();

			if (!decimal.TryParse(sizeAsString, NumberStyles.Number, this.NumberFormat, out var size)) throw new ArgumentException("Illegal number", nameof(value));

			return (long)(Multiplier(unit) * size);
		}

		protected static bool IsDigit(char value) => value >= '0' && value <= '9';

		protected string ExtractUnit(string sizeWithUnit)
		{
			var lastChar = sizeWithUnit.Length - 1;
			var unitLength = 0;

			while (unitLength <= lastChar && sizeWithUnit[lastChar - unitLength] != ' ' && !IsDigit(sizeWithUnit[lastChar - unitLength])) unitLength++;

			return sizeWithUnit.Substring(sizeWithUnit.Length - unitLength).ToUpperInvariant();
		}

		protected long Multiplier(string unit)
		{
			unit = unit.ToUpperInvariant();

			if (!this.Units.ContainsKey(unit)) throw new ArgumentException("Illegal or unknown unit", nameof(unit));

			return this.Units[unit];
		}
	}
}
