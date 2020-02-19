using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Image
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class FileInterlace : ImageCheck
	{
		public override string Name => "Image Interlace";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public bool Interlace { get; set; } = false;

		public override CheckResult Test(ImageTarget target)
		{
			if (target.Properties.Interlace.HasFlag(ImageMagick.Interlace.NoInterlace) == this.Interlace)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Incorrect interlace {target.Properties.Interlace.ToString()}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
