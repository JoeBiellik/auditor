using System;
using System.Threading.Tasks;
using Auditor.SDK.Targets;
using YamlDotNet.Serialization;

namespace Auditor.SDK.Checks
{
	public interface ICheck
	{
		[YamlIgnore]
		string Name { get; }

		[YamlIgnore]
		Version Version { get; }

		[YamlIgnore]
		string Description { get; }

		[YamlIgnore]
		TargetType Accepts { get; }

		Task Load();
	}
}
