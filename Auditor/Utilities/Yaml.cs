using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Auditor.Utilities
{
	public static class YamlNodeToEventStreamConverter
	{
		public static IEnumerable<ParsingEvent> ConvertToEventStream(YamlNode node) => node switch
		{
			YamlScalarNode scalar => ConvertToEventStream(scalar),
			YamlSequenceNode sequence => ConvertToEventStream(sequence),
			YamlMappingNode mapping => ConvertToEventStream(mapping),
			_ => throw new NotSupportedException($"Unsupported node type: {node.GetType().Name}")
		};

		private static IEnumerable<ParsingEvent> ConvertToEventStream(YamlScalarNode scalar)
		{
			yield return new Scalar(scalar.Anchor, scalar.Tag, scalar.Value, scalar.Style, false, false);
		}

		private static IEnumerable<ParsingEvent> ConvertToEventStream(YamlSequenceNode sequence)
		{
			yield return new SequenceStart(sequence.Anchor, sequence.Tag, false, sequence.Style);

			foreach (var node in sequence.Children)
			{
				foreach (var evt in ConvertToEventStream(node))
				{
					yield return evt;
				}
			}

			yield return new SequenceEnd();
		}

		private static IEnumerable<ParsingEvent> ConvertToEventStream(YamlMappingNode mapping)
		{
			yield return new MappingStart(mapping.Anchor, mapping.Tag, false, mapping.Style);

			foreach (var (key, value) in mapping.Children)
			{
				foreach (var evt in ConvertToEventStream(key)) yield return evt;
				foreach (var evt in ConvertToEventStream(value)) yield return evt;
			}

			yield return new MappingEnd();
		}
	}

	public class EventStreamParserAdapter : IParser
	{
		private readonly IEnumerator<ParsingEvent> enumerator;

		public ParsingEvent Current => this.enumerator.Current;

		public EventStreamParserAdapter(IEnumerable<ParsingEvent> events)
		{
			this.enumerator = events.GetEnumerator();
		}

		public bool MoveNext() => this.enumerator.MoveNext();
	}
}
