using System.Collections.Generic;
using Auditor.SDK.Checks;

namespace Auditor
{
	public class Rule
	{
		public string Name { get; set; }

		public IEnumerable<string> Target { get; set; }

		public ICheck Check { get; set; }
	}
}
