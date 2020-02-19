using Auditor.SDK.Targets;

namespace Auditor.SDK.Checks
{
	public interface IFileCheck : ICheck
	{
		CheckResult Test(FileTarget target);
	}
}
