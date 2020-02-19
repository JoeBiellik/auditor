using System;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Image
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Resolution : ImageCheck
	{
		public override string Name => "Image Resolution";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public Size Min { get; set; } = null;
		public Size Max { get; set; } = null;

		public class Size
		{
			public int? Width { get; set; } = null;

			public int? Height { get; set; } = null;
		}

		public override CheckResult Test(ImageTarget target)
		{
			if (this.Min == null && this.Max == null) throw new ArgumentException("Neither Min or Max size values are set");

			if (this.Min?.Width != null && target.Properties.Width < this.Min.Width)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Width ({target.Properties.Width}) is below minimum of {this.Min.Width}" }
				};
			}
			if (this.Min?.Height != null && target.Properties.Height < this.Min.Height)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Height ({target.Properties.Height}) is below minimum of {this.Min.Height}" }
				};
			}

			if (this.Max?.Width != null && target.Properties.Width > this.Max.Width)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Width ({target.Properties.Width}) is above maximum of {this.Max.Width}" }
				};
			}
			if (this.Max?.Height != null && target.Properties.Height > this.Max.Height)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Height ({target.Properties.Height}) is above maximum of {this.Max.Height}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
