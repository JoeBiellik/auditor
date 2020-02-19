using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Auditor.SDK.Checks;
using Auditor.Utilities;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Auditor
{
	public class Rulebook
	{
		private readonly Dictionary<string, Type> types;
		private readonly string[] ignore = { "name", "target" };


		public List<string> Target { get; protected set; }

		public List<string> Exclude { get; protected set; }

		public List<Rule> Rules { get; protected set; } = new List<Rule>();

		public Rulebook(string file)
		{
			if (!File.Exists(file)) throw new FileNotFoundException("Rulebook file not found", file);

			Program.Output.Information($"Loading rulebook: {file.Theme("path")}");

			var yaml = new YamlStream();

			try
			{
				yaml.Load(new StringReader(File.ReadAllText(file)));
			}
			catch (Exception ex)
			{
				throw new FileLoadException("Unable to load rulebook file", file, ex);
			}

			var root = yaml.Documents[0].RootNode.ToMapping();

			this.Target = root.ChildList("target");
			if (!this.Target.Any()) throw new Exception("No target specified");
			Program.Output.Information($"Target: {this.Target.Theme("path")}");

			this.Exclude = root.ChildList("exclude");
			Program.Output.Information($"Exclude target: {this.Exclude.Theme("path")}");

			this.types = LoadTypes(Directory.EnumerateFiles("Checks", "*.dll"));

			var rules = root.Children["rules"].ToSequence();
			if (!rules.Any()) throw new Exception("No rules specified in rulebook");

			Program.Output.Verbose($"Loading {rules.Count().Theme("count")} rules(s)");

			foreach (var rule in rules.Cast<YamlMappingNode>())
			{
				Program.Output.Verbose($"Loading rule \"{rule.ChildValue("name").Theme("check")}\" with check {rule.Children.Select(c => c.Key.ToScalar().Value).First(c => !this.ignore.Contains(c)).Theme("path")}");

				this.Rules.Add(BuildRule(rule));
			}

			if (!this.Rules.Any()) throw new Exception("No rules loaded");
		}

		public static Dictionary<string, Type> LoadTypes(IEnumerable<string> files)
		{
			return files
				.SelectMany(f => Assembly
					.LoadFrom(f)
					.GetTypes()
					.Where(t => t.GetInterface(typeof(ICheck).Name) != null && !t.IsAbstract && t.IsPublic)
					.Select(t => (
						name: Path.GetFileNameWithoutExtension(f).ToLowerInvariant() + "." + t.Name.ToLowerInvariant(),
						type: t
					))
				).ToDictionary(t => t.name, t => t.type);
		}

		private Rule BuildRule(YamlMappingNode node)
		{
			var rule = new Rule
			{
				Name = node.ChildValue("name") ?? throw new Exception("Rule has no name"),
				Target = node.ChildList("target")
			};

			if (!rule.Target.Any()) throw new Exception("Rule has no target");

			foreach (var (key, value) in node.Children.Where(n => this.ignore.All(i => !n.Key.Equals(new YamlScalarNode(i)))))
			{
				var name = key.ToScalar().Value;

				if (!this.types.ContainsKey(name.ToLowerInvariant())) throw new FileLoadException($"Unable to find a check of type {name}");

				rule.Check = DeserializeCheck(this.types[name.ToLowerInvariant()], value.ToMapping());

				if (rule.Check == null) throw new FileLoadException($"Unable to load a check of type {name}");

				rule.Check.Load();
			}

			return rule;
		}

		private static ICheck DeserializeCheck(Type type, YamlNode node)
		{
			if (node == null) return Activator.CreateInstance(type) as ICheck;

			try
			{
				return new DeserializerBuilder()
					.WithNamingConvention(UnderscoredNamingConvention.Instance)
					.Build()
					.Deserialize(node.ToParser(), type) as ICheck;
			}
			catch (Exception e)
			{
				throw new Exception($"Error loading check: {type.Name}", e);
			}
		}
	}
}
