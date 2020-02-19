using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Auditor.SDK.Checks;
using Auditor.SDK.Extensions;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Frames : AudioCheck
	{
		public override string Name => "Frames";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public dynamic Present { get; set; } = null; // string, list, false
		public dynamic Absent { get; set; } = null; // string, list, true, false
		public dynamic Optional { get; set; } = null; // string, list

		public override CheckResult Test(AudioTarget target)
		{
			var present = new List<string>();
			var absent = new List<string>();
			var optional = new List<string>();
			var checkPresent = false;
			var checkAbsent = false;
			var errors = new List<string>();

			if (YamlExtensions.IsBool(this.Present))
			{
				checkPresent = YamlExtensions.GetBool(this.Present);
			}
			if (YamlExtensions.IsString(this.Present))
			{
				checkPresent = true;
				present.Add(YamlExtensions.GetString(this.Present));
			}
			if (YamlExtensions.IsList(this.Present))
			{
				checkPresent = true;
				present.AddRange(YamlExtensions.GetList<string>(this.Present));
			}

			if (YamlExtensions.IsBool(this.Absent))
			{
				checkAbsent = YamlExtensions.GetBool(this.Absent);
			}
			if (YamlExtensions.IsString(this.Absent))
			{
				checkAbsent = true;
				absent.Add(YamlExtensions.GetString(this.Absent));
			}
			if (YamlExtensions.IsList(this.Absent))
			{
				checkAbsent = true;
				absent.AddRange(YamlExtensions.GetList<string>(this.Absent));
			}

			if (YamlExtensions.IsString(this.Optional))
			{
				optional.Add(YamlExtensions.GetString(this.Optional));
			}
			if (YamlExtensions.IsList(this.Optional))
			{
				optional.AddRange(YamlExtensions.GetList<string>(this.Optional));
			}

			if (checkPresent)
			{
				errors.AddRange(present.Except(optional)
					.Where(frame => !target.Tags.Frames.ContainsKey(frame))
					.Select(frame => $"Missing frame {frame}"));
			}

			if (checkAbsent)
			{
				if (absent.Count == 0)
				{
					errors.AddRange(target.Tags.Frames
						.Select(f => f.Key)
						.Except(present).Except(optional)
						.Select(f => $"Has frame {f}"));
				}
				else
				{
					errors.AddRange(absent.Except(optional)
						.Where(frame => target.Tags.Frames.ContainsKey(frame))
						.Select(frame => $"Has frame {frame}"));
				}
			}

			if (errors.Count > 0)
			{
				return new CheckResult
				{
					Success = false,
					Messages = errors
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
