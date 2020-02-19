using System.Collections.Generic;

namespace Auditor.SDK.Checks
{
	public class CheckResult
	{
		public bool Success { get; set; }

		public IEnumerable<string> Messages { get; set; }
	}
}
