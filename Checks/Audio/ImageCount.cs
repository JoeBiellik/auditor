using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class ImageCount : RangeAudioCheck
	{
		public override string Name => "Image Count";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks a song's embedded cover art count matches target";

		protected override string Prefix => "Cover art count";

		protected override int TestValue(AudioTarget target) => target.Tags.Pictures.Count();
	}
}
