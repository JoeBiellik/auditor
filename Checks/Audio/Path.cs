using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Auditor.SDK.Checks;
using Auditor.SDK.Extensions;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Path : AudioCheck
	{
		public override string Name => "Path Matches Tags";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks a song's path matches target tags";

		public bool IgnoreCase { get; set; } = true;
		public string Pattern { get; set; }

		public override CheckResult Test(AudioTarget target)
		{
			if (string.IsNullOrWhiteSpace(this.Pattern))
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { "No pattern defined" }
				};
			}

			// TODO: More fields
			var data = new
			{
				target.Tags.TrackNumber,
				Title = Sanitize(target.Tags.Title),
				Album = Sanitize(target.Tags.Album, true),
				Artist = Sanitize(target.Tags.Artist, true),
				Year = target.Tags.Year.ToString(),
				Ext = target.File.Extension.Substring(1)
			};

			var result = this.Pattern.FormatWith(data);

			if (!target.Location.Equals(result, this.IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture))
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Path does not match target {result}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}

		private static string Sanitize(string input, bool trimDot = false)
		{
			input = System.IO.Path.GetInvalidFileNameChars().Concat(System.IO.Path.GetInvalidPathChars()).Distinct().Aggregate(input, (current, c) => current.Replace(c, '-'));

			while (trimDot && input.StartsWith(".")) input = input[1..];
			while (trimDot && input.EndsWith(".")) input = input[..^1];

			return input;
		}
	}
}
