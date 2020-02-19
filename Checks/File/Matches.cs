using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.File
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Matches : FileCheck
	{
		public override string Name => "File Matches";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public string Match { get; set; }

		public override CheckResult Test(FileTarget target)
		{
			if (Regex.IsMatch(target.File.Name, this.Match))
			{
				return new CheckResult
				{
					Success = true
				};
			}

			return new CheckResult
			{
				Success = false,
				Messages = new[] { "File does not match" }
			};
		}
	}
}
