using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Image
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class FileFormat : ImageCheck
	{
		public override string Name => "Image Format";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public string Format { get; set; } = null;

		public override CheckResult Test(ImageTarget target)
		{
			if (target.Properties.Format.ToString() != this.Format)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Incorrect format {target.Properties.Format.ToString()}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
