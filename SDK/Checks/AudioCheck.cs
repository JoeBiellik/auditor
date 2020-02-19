using Auditor.SDK.Targets;

namespace Auditor.SDK.Checks
{
	public abstract class AudioCheck : Check, IAudioCheck
	{
		public override TargetType Accepts => TargetType.Audio;

		public abstract CheckResult Test(AudioTarget target);
	}
}
