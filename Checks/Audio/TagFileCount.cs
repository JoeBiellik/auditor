using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Auditor.SDK;
using Auditor.SDK.Checks;
using Auditor.SDK.Extensions;
using Auditor.SDK.Targets;
using TagLib;
using TagLib.Flac;
using File = TagLib.File;
using Tag = TagLib.Id3v2.Tag;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class TagFileCount : DirectoryCheck
	{
		public override string Name => "Tag File Count";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "";

		public string Files { get; set; } = "*.*";
		public string Count { get; set; } = "";

		public override CheckResult Test(DirectoryTarget target)
		{
			var files = target.Directory.GetFiles().Where(f => Matcher.IsMatch(f.Name, this.Files)).ToList();

			var maxTrack = 0;

			if (files.Any())
			{
				maxTrack = files.Max(f =>
				{
					var file = new File.LocalFileAbstraction(f.FullName);
					using var audio = File.Create(file);

					var id3 = (Tag)audio.GetTag(TagTypes.Id3v2);
					var flac = (Metadata)audio.GetTag(TagTypes.FlacMetadata);
					var tags = id3 ?? flac ?? audio.Tag;

					var tagValue = ("{" + this.Count + "}").FormatWith(tags);

					return int.TryParse(tagValue, out var count) ? count : 0;
				});
			}

			if (files.Count != maxTrack)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new List<string> { "Track count mismatch" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
