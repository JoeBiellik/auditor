using System;
using System.Linq;
using TagLib;
using TagLib.Mpeg;

namespace Auditor.SDK.Audio
{
	public class AudioProperties
	{
		public TimeSpan Duration { get; private set; }
		public string Format { get; private set; }
		public int AudioBitrate { get; private set; }
		public int AudioSampleRate { get; private set; }
		public int BitsPerSample { get; private set; }
		public int AudioChannels { get; private set; }
		public int PhotoWidth { get; private set; }
		public int PhotoHeight { get; private set; }
		public int PhotoQuality { get; private set; }
		public AudioHeader? MpegHeader { get; private set; }

		public static implicit operator AudioProperties(Properties p)
		{
			return new AudioProperties
			{
				Duration = p.Duration,
				Format = p.Description,
				AudioBitrate = p.AudioBitrate,
				AudioSampleRate = p.AudioSampleRate,
				BitsPerSample = p.BitsPerSample,
				AudioChannels = p.AudioChannels,
				PhotoWidth = p.PhotoWidth,
				PhotoHeight = p.PhotoHeight,
				PhotoQuality = p.PhotoQuality,
				MpegHeader = p.Codecs.Where(c => c is AudioHeader).Cast<AudioHeader>().FirstOrDefault()
			};
		}
	}
}
