using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Auditor.SDK.Checks;
using Auditor.SDK.Extensions;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class TagsMatch : AudioCheck
	{
		public override string Name => "Tags Match";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks a song's tags match each other";

		public List<string> Tags { get; set; } = new List<string>();
		public bool Match { get; set; } = true;

		public override CheckResult Test(AudioTarget target)
		{
			var values = new List<string>();

			foreach (var tag in this.Tags)
			{
				try
				{
					values.Add($"{{{tag}}}".FormatWith(target.Tags));
				}
				catch (Exception)
				{
					return new CheckResult
					{
						Success = false,
						Messages = new[] { $"Invalid tag {tag}" }
					};
				}
			}

			if (this.Match)
			{
				if (values.Skip(1).Any(v => !v.Equals(values[0])))
				{
					return new CheckResult
					{
						Success = false,
						Messages = new[] { "Tags do not match" }
					};
				}
			}
			else
			{
				if (values.Skip(1).Any(v => v.Equals(values[0])))
				{
					return new CheckResult
					{
						Success = false,
						Messages = new[] { "Tags match" }
					};
				}
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
