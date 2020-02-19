using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Auditor.SDK.Checks;
using Auditor.SDK.Extensions;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Image
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class FileColorspace : ImageCheck
	{
		public override string Name => "Image Colorspace";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public dynamic Colorspace { get; set; } = null;

		public override CheckResult Test(ImageTarget target)
		{
			var colorspaces = new List<string>();

			if (YamlExtensions.IsString(this.Colorspace)) colorspaces.Add(YamlExtensions.GetString(this.Colorspace));
			if (YamlExtensions.IsList(this.Colorspace)) colorspaces.AddRange(YamlExtensions.GetList<string>(this.Colorspace));

			if (!colorspaces.Contains(target.Properties.ColorSpace.ToString()))
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Incorrect colorspace {target.Properties.ColorSpace.ToString()}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
