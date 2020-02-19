using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Directory
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Empty : DirectoryCheck
	{
		public override string Name => "Directory Empty";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks if the target directories are empty";

		public override CheckResult Test(DirectoryTarget target)
		{
			return new CheckResult
			{
				Success = false,
				Messages = new[] { $"{target.Directory.FullName} exists" }
			};
		}
	}
}
