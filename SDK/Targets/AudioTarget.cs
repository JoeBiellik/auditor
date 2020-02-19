using System.IO;
using Auditor.SDK.Audio;
using TagLib;
using TagLib.Flac;

namespace Auditor.SDK.Targets
{
	public class AudioTarget : FileTarget
	{
		public AudioTags Tags { get; }

		public AudioProperties Properties { get; }

		public AudioTarget(FileSystemInfo @base, string location) : base(@base, location)
		{
			using var audio = TagLib.File.Create(this.File.FullName);

			var id3 = (TagLib.Id3v2.Tag)audio.GetTag(TagTypes.Id3v2);
			var flac = (Metadata)audio.GetTag(TagTypes.FlacMetadata);

			this.Tags = id3 ?? flac ?? audio.Tag;
			this.Properties = audio.Properties;
		}
	}
}
