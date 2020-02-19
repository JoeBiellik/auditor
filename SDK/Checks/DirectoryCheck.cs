using Auditor.SDK.Targets;

namespace Auditor.SDK.Checks
{
	public abstract class DirectoryCheck : Check, IDirectoryCheck
	{
		public override TargetType Accepts => TargetType.Directory;

		public abstract CheckResult Test(DirectoryTarget target);
	}
}
