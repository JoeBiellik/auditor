using System;
using System.Collections.Generic;
using CommandLine;

namespace Auditor
{
	public class Options
	{
		[Value(0, MetaName = "Rulebook", MetaValue = "FILE", HelpText = "Rulebook file to run.")]
		public string Rulebook { get; set; } = "rules.yml";

		[Option('t', "target", MetaValue = "PATH[]", HelpText = "(Optional) One or more paths to directories to target.", SetName = "rulebook")]
		public IEnumerable<string> Target { get; set; } = null;

		[Option('e', "exclude", MetaValue = "PATH[]", HelpText = "One or more patterns to exclude from targeting.", SetName = "rulebook")]
		public IEnumerable<string> Exclude { get; set; } = null;

		[Option('o', "out", MetaValue = "FILE", HelpText = "File to write check results to.", SetName = "rulebook")]
		public string Output { get; set; } = null;

		[Option('j', "jobs", MetaValue = "NUM", HelpText = "Number of jobs (threads) to run concurrently, defaults to number of CPU cores.", SetName = "rulebook")]
		public int Jobs { get; set; } = Environment.ProcessorCount;

		[Option("checks", HelpText = "Display a list of available checks.", SetName = "checks")]
		public bool Checks { get; set; } = false;

		[Option('v', "verbose", HelpText = "Verbose output")]
		public bool Verbose { get; set; } = false;

		[Option('q', "quiet", HelpText = "Quiet output")]
		public bool Quiet { get; set; } = false;
	}
}
