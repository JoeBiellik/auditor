using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Directory
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Exists : DirectoryCheck
	{
		public override string Name => "Directory Exists";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks if the target directories exist";

		public override CheckResult Test(DirectoryTarget target)
		{
			var exists = target.Directory.Exists;

			return new CheckResult
			{
				Success = exists,
				Messages = exists ? null : new[] { $"{target.Location} does not exist" }
			};
		}
	}
}
