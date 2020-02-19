using Auditor.SDK.Targets;

namespace Auditor.SDK.Checks
{
	public interface IAudioCheck : ICheck
	{
		CheckResult Test(AudioTarget target);
	}
}
