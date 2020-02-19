using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Auditor.SDK;
using Auditor.SDK.Checks;
using Auditor.SDK.Targets;
using Auditor.Utilities;
using CommandLine;
using CommandLine.Text;
using Pastel;

namespace Auditor
{
	public static class Program
	{
		private static FileStream logFile;
		private static StreamWriter logWriter;
		private static Stopwatch timer;
		private static int countNodes;
		private static int countTargets;
		private static int countChecks;
		private static int countPass;
		private static int countFail;
		private static int countError;

		internal static readonly Output Output = new Output();

		public static async Task<int> Main(string[] args)
		{
			if (Console.IsOutputRedirected) ConsoleExtensions.Disable(); // Disable ANSI colors

			try
			{
				var parser = new Parser().ParseArguments<Options>(args);

				return await parser.MapResult(Run, e =>
				{
					Console.Error.WriteLine(HelpText.AutoBuild(parser, h =>
					{
						h.AdditionalNewLineAfterOption = false;

						return h;
					}));

					return Task.FromResult(1);
				});
			}
			catch (Exception e)
			{
				Output.Error(e, "Fatal error");

				return 1;
			}
			finally
			{
				Output.Flush();

				logWriter?.Dispose();
				logFile?.Dispose();
			}
		}

		private static async Task<int> Run(Options options)
		{
			Output.SetVerbosity(options.Verbose, options.Quiet);

			Output.Verbose($"Auditor {typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}".Theme("bold"));

			if (Console.IsInputRedirected)
			{
				options.Target = Console.In.ReadToEnd().Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
			}

			if (options.Checks)
			{
				Output.Verbose("Available checks:");

				foreach (var (_, type) in Rulebook.LoadTypes(Directory.EnumerateFiles("Checks", "*.dll")))
				{
					if (!(Activator.CreateInstance(type) is ICheck instance)) throw new Exception($"Unable to load check: {type}");

					Output.Information(string.Empty);
					Output.Information($"Type        : {type.FullName}");
					Output.Information($"Name        : {instance.Name}");
					Output.Information($"Version     : {instance.Version}");
					Output.Information($"Target type : {instance.Accepts}");
					if (!string.IsNullOrWhiteSpace(instance.Description)) Output.Information($"Description : {instance.Description}");
				}

				return 0;
			}

			await Process(options);

			return 0;
		}

		private static async Task Process(Options options)
		{
			timer = Stopwatch.StartNew();


			if (!string.IsNullOrWhiteSpace(options.Output))
			{
				logFile = File.Open(options.Output, FileMode.Create, FileAccess.Write, FileShare.Read);
				logWriter = new StreamWriter(logFile, Encoding.UTF8);
			}

			var rulebook = new Rulebook(options.Rulebook);

			var matchRules = new TransformBlock<(string @base, string path), ((string @base, string path), (string pattern, Rule check)[] actions)>(match =>
			{
				return (match, actions: rulebook.Rules
					.Where(c => match.path.EndsWith(Path.DirectorySeparatorChar) // Path type matches check type
						? c.Check.Accepts == TargetType.Directory
						: c.Check.Accepts != TargetType.Directory)
					.Select(c => (pattern: Matcher.Match(match.path, c.Target), check: c)) // Select match
					.Where(m => m.pattern != null) // Check targets this path
					.ToArray());
			}, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = options.Jobs, BoundedCapacity = options.Jobs * 10 });

			// ReSharper disable once UseDeconstructionOnParameter
			var runRules = new ActionBlock<((string @base, string path) location, (string pattern, Rule check)[] actions)>(match =>
			{
				var directory = new DirectoryInfo(match.location.@base);

				foreach (var actions in match.actions.GroupBy(a => a.check.Check.Accepts))
				{
					try
					{
						var target = actions.Key switch
						{
							TargetType.Directory => (Target)new DirectoryTarget(directory, match.location.path),
							TargetType.File => new FileTarget(directory, match.location.path),
							TargetType.Audio => new AudioTarget(directory, match.location.path),
							TargetType.Image => new ImageTarget(directory, match.location.path),
							TargetType.Video => new VideoTarget(directory, match.location.path),
							_ => throw new ArgumentException("Unknown check target type")
						};

						Interlocked.Increment(ref countTargets);

						foreach (var (pattern, check) in actions)
						{
							try
							{
								Interlocked.Increment(ref countChecks);

								var result = check.Check switch
								{
									IDirectoryCheck c => c.Test(target as DirectoryTarget),
									IFileCheck c => c.Test(target as FileTarget),
									IAudioCheck c => c.Test(target as AudioTarget),
									IImageCheck c => c.Test(target as ImageTarget),
									IVideoCheck c => c.Test(target as VideoTarget),
									_ => throw new ArgumentException("Unknown check type")
								};

								if (result.Success)
								{
									Interlocked.Increment(ref countPass);
									continue;
								}

								Interlocked.Increment(ref countFail);

								Output.Warning($"{check.Name.Theme("check")} {pattern} {Path.Combine(target.Base.FullName, target.Location).Theme("path")} => {result.Messages.Theme("bold")}");
								logWriter?.WriteLine($"{check.Name} {pattern} {Path.Combine(target.Base.FullName, target.Location)} => [ {string.Join(", ", result.Messages)} ]");
							}
							catch (Exception e)
							{
								Interlocked.Increment(ref countError);

								Output.Error(e, "Check error");
							}
						}
					}
					catch (FileLoadException e)
					{
						Interlocked.Increment(ref countError);

						Output.Error(e, $"Error loading target file ({directory} {match.location.path}), skipping checks");
					}
					catch (Exception e)
					{
						Interlocked.Increment(ref countError);

						Output.Error(e, "Target error");
					}
				}
			}, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = options.Jobs, BoundedCapacity = options.Jobs * 2 });

			matchRules.LinkTo(runRules, new DataflowLinkOptions { PropagateCompletion = true }, v => v.actions.Any());
			matchRules.LinkTo(DataflowBlock.NullTarget<((string, string), (string, Rule)[] actions)>(), v => !v.actions.Any()); // Drain null

			var targets = options.Target.Any() ? options.Target : rulebook.Target;
			var exclude = options.Exclude.Any() ? options.Exclude : rulebook.Exclude;

			Output.Information(string.Empty);

			var progress = Task.Factory.StartNew(async () =>
			{
				while (!matchRules.Completion.IsCompleted)
				{
					if (countChecks > 0) Output.Progress($"Processed {countNodes:N0} objects ({countNodes / timer.Elapsed.TotalSeconds:N0}/s), {countChecks:N0} checks ({countChecks / timer.Elapsed.TotalSeconds:N0}/s)".Theme("bold"));

					await Task.Delay(500);
				}
			});

			foreach (var directory in targets.Select(d => new DirectoryInfo(d)))
			{
				if (!directory.Exists)
				{
					Output.Warning($"Specified target directory is not accessible, skipping: {directory.FullName.Theme("path")}");
					continue;
				}

				Output.Verbose($"Processing target directory: {directory.FullName.Theme("path")}");

				foreach (var n in directory.EnumerateTree(new EnumerationOptions
				{
					MatchType = MatchType.Simple,
					MatchCasing = MatchCasing.CaseInsensitive,
					IgnoreInaccessible = true,
					RecurseSubdirectories = false,
					ReturnSpecialDirectories = false
				})
					.Where(n => !Matcher.IsMatch(n, exclude))
					.Select(n => Path.GetRelativePath(directory.FullName, n))
					.OrderBy(n => n))
				{
					Interlocked.Increment(ref countNodes);

					await matchRules.SendAsync((directory.FullName, n));
				}
			}

			matchRules.Complete();

			await runRules.Completion;

			progress.Dispose();
			Output.Progress(string.Empty);

			Output.Information(string.Empty);
			Output.Information($"File system objects loaded: {countNodes.ToString("N0").Theme("count")}");
			Output.Information($"Check targets created: {countNodes.ToString("N0").Theme("count")}");
			Output.Information($"Checks run: {countChecks.ToString("N0").Theme("count")}");
			Output.Information($"Checks passed / failed: {countPass.ToString("N0").Theme("count")} / {countFail.ToString("N0").Theme("path")}");
			Output.Information($"Processing errors: {countError.ToString("N0").Theme("path")}");
			Output.Information(string.Empty);
			Output.Information($"Completed in {timer.Elapsed.TotalSeconds.ToString("N0").Theme("count")} seconds ({(countChecks / timer.Elapsed.TotalSeconds).ToString("N0").Theme("count")} checks per second)");
		}
	}
}
