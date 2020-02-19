using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public abstract class RangeAudioCheck : AudioCheck
	{
		public int? Min { get; set; } = null;
		public int? Max { get; set; } = null;
		public int? Exactly { get; set; } = null;

		protected abstract string Prefix { get; }

		protected abstract int TestValue(AudioTarget target);
		protected virtual string FormatActual(int? i) => $"{this.Prefix} ({i})";
		protected virtual string FormatExpected(int? i) => $"{i}";

		public override Task Load()
		{
			if (!this.Min.HasValue && !this.Max.HasValue && !this.Exactly.HasValue) throw new Exception("Neither Min, Max or Exactly values set");
			if (this.Exactly.HasValue && (this.Min.HasValue || this.Max.HasValue)) throw new Exception("Cannot combine Exactly with Min or Max values");

			return base.Load();
		}

		public override CheckResult Test(AudioTarget target)
		{
			var value = TestValue(target);

			if (this.Min.HasValue && value < this.Min) return new CheckResult
			{
				Success = false,
				Messages = new[] { $"{FormatActual(value)} is below minimum of {FormatExpected(this.Min)}" }
			};

			if (this.Max.HasValue && value > this.Max) return new CheckResult
			{
				Success = false,
				Messages = new[] { $"{FormatActual(value)} is above maximum of {FormatExpected(this.Max)}" }
			};

			if (this.Exactly.HasValue && value != this.Exactly) return new CheckResult
			{
				Success = false,
				Messages = new[] { $"{FormatActual(value)} does not match {FormatExpected(this.Exactly)}" }
			};

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
