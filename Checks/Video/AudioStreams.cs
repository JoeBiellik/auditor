using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Video
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class AudioStreams : VideoCheck
	{
		public override string Name => "Video Audio Streams";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public int? Min { get; set; } = null;
		public int? Max { get; set; } = null;
		public int? Exactly { get; set; } = null;

		public override CheckResult Test(VideoTarget target)
		{
			if (this.Min == null && this.Max == null && this.Exactly == null) throw new Exception("Neither Min, Max or Exactly values set");
			if (this.Exactly != null && (this.Min != null || this.Max != null)) throw new Exception("Cannot combine Exactly with Min or Max values");

			var streams = target.Properties.AudioStreams.Count;

			if (this.Min != null && streams < this.Min)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Audio streams count ({streams}) is below minimum of {this.Min}" }
				};
			}

			if (this.Max != null && streams > this.Max)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Audio streams count ({streams}) is above maximum of {this.Max}" }
				};
			}

			if (this.Exactly != null && streams != this.Exactly)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Audio streams count ({streams}) does not match {this.Exactly}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
