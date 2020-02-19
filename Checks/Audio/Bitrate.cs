using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Checks;
using Auditor.SDK.Interpreters;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Bitrate : AudioCheck
	{
		public override string Name => "Audio Bitrate";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks if the target audio files is within a bitrate range";

		public string Min { get; set; } = null;
		public string Max { get; set; } = null;

		public override CheckResult Test(AudioTarget target)
		{
			if (string.IsNullOrWhiteSpace(this.Min) && string.IsNullOrWhiteSpace(this.Max)) throw new Exception("Neither Min or Max values set");

			var bitrate = target.Properties.AudioBitrate * 1000L;

			if (!string.IsNullOrWhiteSpace(this.Min) && bitrate < this.Min.ToBitRate())
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Bitrate ({bitrate}) is below minimum of {this.Min.ToBitRate()}" }
				};
			}

			if (!string.IsNullOrWhiteSpace(this.Max) && bitrate > this.Max.ToBitRate())
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Bitrate ({bitrate}) is above maximum of {this.Max.ToBitRate()}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
