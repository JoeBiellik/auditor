using Auditor.SDK.Targets;

namespace Auditor.SDK.Checks
{
	public interface IVideoCheck : ICheck
	{
		CheckResult Test(VideoTarget target);
	}
}
