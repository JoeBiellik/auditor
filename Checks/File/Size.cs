using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Auditor.SDK.Checks;
using Auditor.SDK.Interpreters;
using Auditor.SDK.Targets;

namespace Auditor.Checks.File
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Size : FileCheck
	{
		public override string Name => "File Size";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks if the target files are within a size range";

		public string Min { get; set; } = null;
		public string Max { get; set; } = null;

		public override Task Load()
		{
			if (string.IsNullOrWhiteSpace(this.Min) && string.IsNullOrWhiteSpace(this.Max)) throw new ArgumentException("Neither Min or Max size values are set");

			return base.Load();
		}

		public override CheckResult Test(FileTarget target)
		{
			var size = target.File.Length;

			if (!string.IsNullOrWhiteSpace(this.Min) && size < this.Min.ToFileSize())
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Size ({size}) is below minimum of {this.Min.ToFileSize()}" }
				};
			}

			if (!string.IsNullOrWhiteSpace(this.Max) && size > this.Max.ToFileSize())
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Size ({size}) is above maximum of {this.Max.ToFileSize()}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
