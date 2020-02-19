using System.IO;
using MediaInfo;

namespace Auditor.SDK.Targets
{
	public class VideoTarget : FileTarget
	{
		public MediaInfoWrapper Properties { get; }

		public VideoTarget(FileSystemInfo @base, string location) : base(@base, location)
		{
			this.Properties = new MediaInfoWrapper(this.File.FullName);

			if (this.Properties.MediaInfoNotloaded) throw new FileLoadException("Media info not loaded", this.File.FullName);
		}
	}
}
