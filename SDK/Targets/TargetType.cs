using System;

namespace Auditor.SDK.Targets
{
	[Flags]
	public enum TargetType
	{
		Directory,
		File,
		Audio,
		Image,
		Video
	}
}
