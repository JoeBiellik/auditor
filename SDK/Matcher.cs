using System;
using System.Collections.Generic;
using Minimatch;

namespace Auditor.SDK
{
	public static class Matcher
	{
		public static bool IsMatch(string subject, string pattern) => IsMatch(subject, new[] { pattern });

		public static bool IsMatch(string subject, IEnumerable<string> patterns) => Match(subject, patterns) != null;

		public static string Match(string subject, IEnumerable<string> patterns)
		{
			string match = null;

			var ops = new Options
			{
				AllowWindowsPaths = true,
				NoComment = true,
				NoNegate = true,
				NoCase = false
			};

			foreach (var pattern in patterns)
			{
				var patternPath = pattern;
				var negative = false;

				if (pattern.StartsWith("!"))
				{
					patternPath = pattern[1..];
					negative = true;
				}

				ops.NoCase = false;

				if (patternPath.IndexOf('?') > 0)
				{
					var flags = patternPath.Substring(patternPath.IndexOf('?') + 1);
					patternPath = patternPath[..patternPath.IndexOf('?')];

					foreach (var flag in flags)
					{
						// ReSharper disable once ConvertSwitchStatementToSwitchExpression
						// ReSharper disable once SwitchStatementMissingSomeCases
						switch (flag)
						{
							case 'i':
								ops.NoCase = true;
								break;
							default:
								throw new ArgumentException($"Unknown pattern flag: {flag}");
						}
					}
				}

				var matcher = new Minimatcher(patternPath, ops);

				if (matcher.IsMatch(subject))
				{
					match = negative ? null : pattern;
				}
			}

			return match;
		}
	}
}
