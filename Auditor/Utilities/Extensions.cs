using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Pastel;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Auditor.Utilities
{
	public static class DirectoryInfoExtensions
	{
		public static IEnumerable<string> EnumerateTree(this DirectoryInfo baseDirectory, EnumerationOptions opts)
		{
			foreach (var file in baseDirectory.EnumerateFiles("*", opts)) yield return file.FullName;

			foreach (var directory in baseDirectory.EnumerateDirectories("*", opts))
			{
				yield return directory.FullName + Path.DirectorySeparatorChar;

				foreach (var n in directory.EnumerateTree(opts)) yield return n;
			}
		}
	}

	public static class OutputExtensions
	{
		private static readonly Dictionary<string, Color> Styles = new Dictionary<string, Color>
		{
			{ "bold", Color.White },
			{ "check", Color.FromArgb(0xb5cea8) },
			{ "path", Color.FromArgb(0xce9178) },
			{ "count", Color.FromArgb(0xb5cea8) },
			{ "error", Color.Red }
		};

		public static string Theme(this string input, string style) => input.Pastel(Styles[style]);

		public static string Theme(this int input, string style) => input.ToString("N0").Pastel(Styles[style]);

		public static string Theme(this IEnumerable<string> input, string theme)
		{
			var items = input as string[] ?? input.ToArray();
			return items.Any() ? "[ " + string.Join(", ", items.Select(i => i.Theme(theme))) + " ]" : "none";
		}
	}

	public static class YamlExtensions
	{
		public static T ToNode<T>(this YamlNode node) where T : YamlNode => node as T;

		public static YamlScalarNode ToScalar(this YamlNode node) => node?.ToNode<YamlScalarNode>();

		public static YamlSequenceNode ToSequence(this YamlNode node) => node?.ToNode<YamlSequenceNode>();

		public static YamlMappingNode ToMapping(this YamlNode node) => node?.ToNode<YamlMappingNode>();

		public static IParser ToParser(this YamlNode node) => new EventStreamParserAdapter(YamlNodeToEventStreamConverter.ConvertToEventStream(node));

		public static YamlNode Child(this YamlMappingNode node, YamlNode key) => node.Children.ContainsKey(key) ? node.Children[key] : null;

		public static string ChildValue(this YamlMappingNode node, YamlNode key) => node.Child(key)?.ToScalar()?.Value;

		public static List<string> ChildList(this YamlMappingNode node, YamlNode key)
		{
			var value = node.ChildValue(key);
			return value != null ? new List<string> { value } : node.Child(key)?.ToSequence()?.Cast<YamlScalarNode>().Select(n => n.Value).ToList() ?? new List<string>();
		}
	}
}
