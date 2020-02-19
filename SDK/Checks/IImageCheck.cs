using Auditor.SDK.Targets;

namespace Auditor.SDK.Checks
{
	public interface IImageCheck : ICheck
	{
		CheckResult Test(ImageTarget target);
	}
}
