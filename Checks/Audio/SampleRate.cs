using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class SampleRate : RangeAudioCheck
	{
		public override string Name => "Audio Sample Rate";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		protected override string Prefix => "Sample rate";

		protected override int TestValue(AudioTarget target) => target.Properties.AudioSampleRate;
	}
}
