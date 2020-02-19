using System.IO;

namespace Auditor.SDK.Targets
{
	public class DirectoryTarget : Target
	{
		public DirectoryInfo Directory { get; }

		public DirectoryTarget(FileSystemInfo @base, string location) : base(@base, location)
		{
			this.Directory = new DirectoryInfo(Path.Combine(this.Base.FullName, this.Location));
		}
	}
}
