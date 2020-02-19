using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class MpegVbr : AudioCheck
	{
		public override string Name => "Audio MPEG VBR";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public bool Vbr { get; set; } = false;

		public override CheckResult Test(AudioTarget target)
		{
			if (target.Properties.MpegHeader != null && (target.Properties.MpegHeader.Value.VBRIHeader.Present || target.Properties.MpegHeader.Value.XingHeader.Present) != this.Vbr)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { this.Vbr ? "MPEG is not VBR" : "MPEG is VBR" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
