using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Image
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class AspectRatio : ImageCheck
	{
		public override string Name => "Image Aspect Ratio";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public double? Min { get; set; } = null;
		public double? Max { get; set; } = null;
		public double? Exactly { get; set; } = null;

		public override Task Load()
		{
			if (this.Min == null && this.Max == null && this.Exactly == null) throw new Exception("Neither Min, Max or Exactly values set");
			if (this.Exactly != null && (this.Min != null || this.Max != null)) throw new Exception("Cannot combine Exactly with Min or Max values");

			return base.Load();
		}

		public override CheckResult Test(ImageTarget target)
		{
			var ar = (double)target.Properties.Width / target.Properties.Height;

			if (this.Min != null && ar < this.Min)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Aspect ratio ({ar}) is below minimum of {this.Min}" }
				};
			}

			if (this.Max != null && ar > this.Max)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Aspect ratio ({ar}) is above maximum of {this.Max}" }
				};
			}

			if (this.Exactly != null && Math.Abs(ar - this.Exactly.Value) > 0.000001)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Aspect ratio ({ar}) does not match {this.Exactly}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
