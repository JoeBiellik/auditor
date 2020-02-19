using Auditor.SDK.Targets;

namespace Auditor.SDK.Checks
{
	public abstract class VideoCheck : Check, IVideoCheck
	{
		public override TargetType Accepts => TargetType.Video;

		public abstract CheckResult Test(VideoTarget target);
	}
}
