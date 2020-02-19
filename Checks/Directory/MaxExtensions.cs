using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Auditor.SDK;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Directory
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class MaxExtensions : DirectoryCheck
	{
		public override string Name => "Directory Max Extensions";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks if the target directories only contain valid extensions";

		public int Count { get; set; } = 1;
		public List<string> Ignore { get; set; } = new List<string>();

		public override CheckResult Test(DirectoryTarget target)
		{
			var files = target.Directory.GetFiles("*", SearchOption.TopDirectoryOnly);

			var exts = files
				.Where(f => !this.Ignore.Any(i => Matcher.IsMatch(f.Name, i)))
				.Select(f => f.Extension.TrimStart('.'))
				.Distinct()
				.ToArray();

			if (exts.Length > this.Count)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Contains {exts.Length} file extensions: {string.Join(", ", exts)}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
