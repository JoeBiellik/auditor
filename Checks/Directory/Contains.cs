using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Auditor.SDK;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Directory
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Contains : DirectoryCheck
	{
		public override string Name => "Directory Contains";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks if the target directories contain a pattern";

		public List<string> Matches { get; set; }

		public override CheckResult Test(DirectoryTarget target)
		{
			foreach (var pattern in this.Matches)
			{
				var files = target.Directory.GetFiles();

				if (!files.Any(f => Matcher.IsMatch(f.Name, pattern)))
				{
					return new CheckResult
					{
						Success = false,
						Messages = new[] { $"{pattern} does not exist" }
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
