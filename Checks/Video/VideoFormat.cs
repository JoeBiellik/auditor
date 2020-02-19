using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Video
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class VideoFormat : VideoCheck
	{
		public override string Name => "Video Format";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public string Format { get; set; } = null;

		public string Codec { get; set; } = null;

		public override CheckResult Test(VideoTarget target)
		{
			if (target.Properties.VideoStreams.Any(v => v.Format != this.Format))
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Video stream format ({string.Join(", ", target.Properties.VideoStreams.Select(v => v.Format))}) does not match {this.Format}" }
				};
			}

			if (target.Properties.VideoStreams.Any(v => v.Codec.ToString() != this.Codec))
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Video stream codec ({string.Join(", ", target.Properties.VideoStreams.Select(v => v.Codec.ToString()))}) does not match {this.Codec}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
