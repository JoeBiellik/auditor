using System;
using System.Collections.Generic;
using System.Linq;

namespace Auditor.SDK.Extensions
{
	public static class YamlExtensions
	{
		public static bool IsBool(dynamic data)
		{
			if (!(data is bool || data is string)) return false;

			if (data is bool)
			{
				try
				{
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}

			try
			{
				var str = (string)data;

				return str.IsYamlBool();
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool GetBool(dynamic data)
		{
			if (!IsBool(data)) throw new FormatException("Dynamic data is not a bool");

			if (data is bool b)
			{
				return b;
			}

			return ((string)data).ToYamlBool();
		}

		public static bool IsString(dynamic data)
		{
			if (!(data is string)) return false;

			try
			{
				var str = (string)data;

				return !str.IsYamlBool();
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static string GetString(dynamic data)
		{
			if (!IsString(data)) throw new FormatException("Dynamic data is not a string");

			return (string)data;
		}

		public static bool IsList(dynamic data)
		{
			if (!(data is List<object>)) return false;

			try
			{
				var _ = (List<object>)data;

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static IEnumerable<object> GetList(dynamic data)
		{
			if (!IsList(data)) throw new FormatException("Dynamic data is not a list");

			return (IEnumerable<object>)data;
		}

		public static IEnumerable<T> GetList<T>(dynamic data)
		{
			if (!IsList(data)) throw new FormatException("Dynamic data is not a list");

			IEnumerable<object> list = GetList(data);

			return list.Cast<T>();
		}

		public static bool IsYamlBool(this string str)
		{
			return str.Equals("y") || str.Equals("true") || str.Equals("yes") || str.Equals("on") || str.Equals("n") || str.Equals("false") || str.Equals("no") || str.Equals("off");
		}

		public static bool ToYamlBool(this string str)
		{
			if (!str.IsYamlBool()) throw new FormatException("Dynamic data is not a bool");

			return str.Equals("y") || str.Equals("true") || str.Equals("yes") || str.Equals("on");
		}
	}
}
