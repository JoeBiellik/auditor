using System.IO;
using ImageMagick;

namespace Auditor.SDK.Targets
{
	public class ImageTarget : FileTarget
	{
		public MagickImageInfo Properties { get; }

		public ImageTarget(FileSystemInfo @base, string location) : base(@base, location)
		{
			this.Properties = new MagickImageInfo(this.File.FullName);
		}
	}
}
