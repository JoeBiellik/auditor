using System.IO;

namespace Auditor.SDK.Targets
{
	public class FileTarget : Target
	{
		public FileInfo File { get; }

		public FileTarget(FileSystemInfo @base, string location) : base(@base, location)
		{
			this.File = new FileInfo(Path.Combine(this.Base.FullName, this.Location));
		}
	}
}
