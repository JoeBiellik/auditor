using Auditor.SDK.Targets;

namespace Auditor.SDK.Checks
{
	public abstract class FileCheck : Check, IFileCheck
	{
		public override TargetType Accepts => TargetType.File;

		public abstract CheckResult Test(FileTarget target);
	}
}
