using Auditor.SDK.Targets;

namespace Auditor.SDK.Checks
{
	public interface IDirectoryCheck : ICheck
	{
		CheckResult Test(DirectoryTarget target);
	}
}
