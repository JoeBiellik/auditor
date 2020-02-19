using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Channels : RangeAudioCheck
	{
		public override string Name => "Audio Channel Count";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		protected override string Prefix => "Channel count";

		protected override int TestValue(AudioTarget target) => target.Properties.AudioChannels;
	}
}
