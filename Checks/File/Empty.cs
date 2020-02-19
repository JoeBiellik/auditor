using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.File
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Empty : FileCheck
	{
		public override string Name => "File Empty";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public override CheckResult Test(FileTarget target)
		{
			return new CheckResult
			{
				Success = false,
				Messages = new[] { $"{target.File.FullName} exists" }
			};
		}
	}
}
