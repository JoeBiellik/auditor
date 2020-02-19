using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Image
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class FileCompression : ImageCheck
	{
		public override string Name => "Image Compression";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public string Compression { get; set; } = null;

		public override CheckResult Test(ImageTarget target)
		{
			if (target.Properties.Compression.ToString() != this.Compression)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Incorrect compression {target.Properties.Compression.ToString()}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
