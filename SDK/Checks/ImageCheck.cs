using Auditor.SDK.Targets;

namespace Auditor.SDK.Checks
{
	public abstract class ImageCheck : Check, IImageCheck
	{
		public override TargetType Accepts => TargetType.Image;

		public abstract CheckResult Test(ImageTarget target);
	}
}
