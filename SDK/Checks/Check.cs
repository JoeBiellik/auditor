using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auditor.SDK.Targets;
using YamlDotNet.Serialization;

namespace Auditor.SDK.Checks
{
	public abstract class Check : ICheck
	{
		public abstract string Name { get; }

		public abstract Version Version { get; }

		public abstract string Description { get; }

		public abstract TargetType Accepts { get; }

		[YamlMember(Alias = "target")]
		public IEnumerable<string> Targets { get; protected set; } = new List<string>();

		public virtual async Task Load()
		{
			await Task.FromResult(0);
		}

		public override string ToString() => this.Name;
	}
}
