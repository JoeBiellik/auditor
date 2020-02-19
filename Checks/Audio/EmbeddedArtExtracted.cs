using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Auditor.SDK.Audio;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class EmbeddedArtExtracted : AudioCheck
	{
		public override string Name => "Embedded Art Matches Extracted";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks a song's embedded cover art matches the extracted cover art file";

		public string File { get; set; } = "folder.jpg";
		public string Position { get; set; } = "FrontCover";

		public override CheckResult Test(AudioTarget target)
		{
			AudioPictureType position;

			try
			{
				position = (AudioPictureType)Enum.Parse(typeof(AudioPictureType), this.Position);
			}
			catch (Exception)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Position ({this.Position}) is not valid" }
				};
			}

			if (target.Tags.Pictures.All(p => p.Type != position))
			{
				return new CheckResult
				{
					Success = true
				};
			}

			var file = new FileInfo(System.IO.Path.Combine(target.File.DirectoryName, this.File));

			if (!file.Exists)
			{
				return new CheckResult
				{
					Success = true
				};
			}

			var embeddedLength = target.Tags.Pictures.First(p => p.Type == position).Data.Length;

			if (embeddedLength != file.Length)
			{
				return new CheckResult
				{
					Success = false,
					Messages = new[] { $"Embedded cover art length ({embeddedLength}) does not match {this.File} length {file.Length}" }
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
