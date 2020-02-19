using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Auditor.Utilities
{
	public class Output
	{
		private readonly ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
		private readonly object progressLock = new object();
		private string progress = string.Empty;
		private VerbosityLevel verbosity = VerbosityLevel.Normal;

		public Output()
		{
			// ReSharper disable once FunctionNeverReturns
			Task.Factory.StartNew(async () =>
			{
				while (true)
				{
					Flush();

					await Task.Delay(250);
				}
			});
		}

		public void Flush()
		{
			Console.CursorVisible = false;

			while (this.queue.TryDequeue(out var message))
			{
				Console.WriteLine(message.PadRight(Console.WindowWidth));
			}

			if (Console.IsOutputRedirected) return;

			lock (this.progressLock)
			{
				var line = Console.CursorTop;
				Console.WriteLine(this.progress.PadRight(Console.WindowWidth));
				Console.SetCursorPosition(0, line);
			}

			Console.CursorVisible = true;
		}

		public void Progress(string message)
		{
			lock (this.progressLock)
			{
				this.progress = message;
			}
		}

		public void SetVerbosity(bool verbose, bool quiet)
		{
			this.verbosity = verbose ? VerbosityLevel.Verbose : quiet ? VerbosityLevel.Quiet : VerbosityLevel.Normal;
		}

		public void Write(OutputLevel level, string message)
		{
			// ReSharper disable once ConvertIfStatementToSwitchStatement
			if (level == OutputLevel.Verbose && this.verbosity != VerbosityLevel.Verbose) return;
			if (level == OutputLevel.Information && this.verbosity == VerbosityLevel.Quiet) return;

			this.queue.Enqueue(message);
		}

		public void Verbose(string message)
		{
			Write(OutputLevel.Verbose, message);
		}

		public void Information(string message)
		{
			Write(OutputLevel.Information, message);
		}

		public void Warning(string message)
		{
			Write(OutputLevel.Warning, message);
		}

		// TODO: STDERR
		public void Error(Exception e, string message)
		{
			Write(OutputLevel.Error, $"{message.Theme("error")}: {e.Message}");
		}

		public enum OutputLevel
		{
			Verbose,
			Information,
			Warning,
			Error
		}

		public enum VerbosityLevel
		{
			Verbose,
			Normal,
			Quiet
		}
	}
}
