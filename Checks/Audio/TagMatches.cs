using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Auditor.SDK.Checks;
using Auditor.SDK.Extensions;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class TagMatches : AudioCheck
	{
		public override string Name => "Tag Matches Regex";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks a song's embedded cover art count matches target";

		public dynamic Tags { get; set; } = null; // string or list of PhotoTypes
		public string Pattern { get; set; } = string.Empty;

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

			var tags = new List<string>();


			if (YamlExtensions.IsString(this.Tags))
			{
				tags.Add(YamlExtensions.GetString(this.Tags));
			}

			if (YamlExtensions.IsList(this.Tags))
			{
				foreach (string str in YamlExtensions.GetList<string>(this.Tags))
				{
					tags.Add(str);
				}
			}

			if (tags.Count < 1)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { "No tags defined" }
				};
			}

			var errors = new List<string>();

			foreach (var tag in tags)
			{
				string tagValue;
				try
				{
					tagValue = ("{" + tag + "}").FormatWith(target.Tags);
				}
				catch (Exception)
				{
					errors.Add($"Invalid tag {tag}");
					continue;
				}

				if (!new Regex(this.Pattern).IsMatch(tagValue))
				{
					errors.Add($"Tag {tag} value {tagValue} did not match pattern");
				}
			}

			if (errors.Count > 0)
			{
				return new CheckResult
				{
					Success = false,
					Messages = errors
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
