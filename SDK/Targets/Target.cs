using System.IO;

namespace Auditor.SDK.Targets
{
	public abstract class Target
	{
		public FileSystemInfo Base { get; }

		public string Location { get; }

		protected Target(FileSystemInfo @base, string location)
		{
			this.Base = @base;
			this.Location = location;
		}
	}
}
