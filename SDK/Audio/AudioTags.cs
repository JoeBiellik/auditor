using System.Collections.Generic;
using System.Linq;
using TagLib;
using TagLib.Flac;
using TagLib.Id3v2;
using TagLib.Ogg;
using Tag = TagLib.Tag;

namespace Auditor.SDK.Audio
{
	public class AudioTags
	{
		public AudioTagTypes Type { get; private set; }
		public Dictionary<string, List<string>> Frames { get; private set; }
		public string Title { get; private set; }
		public string TitleSort { get; private set; }
		public string Artist { get; private set; }
		public string ArtistSort { get; private set; }
		public string AlbumArtist { get; private set; }
		public string AlbumArtistSort { get; private set; }
		public string Composer { get; private set; }
		public string ComposerSort { get; private set; }
		public string Album { get; private set; }
		public string AlbumSort { get; private set; }
		public string Comment { get; private set; }
		public string Genre { get; private set; }
		public uint Year { get; private set; }
		public string TrackNumber { get; private set; }
		public uint TrackNumberValue { get; private set; }
		public uint TrackCount { get; private set; }
		public uint Disc { get; private set; }
		public uint DiscCount { get; private set; }
		public string Lyrics { get; private set; }
		public string Grouping { get; private set; }
		public uint BeatsPerMinute { get; private set; }
		public string Conductor { get; private set; }
		public string Copyright { get; private set; }
		public string MusicIpId { get; private set; }
		public string AmazonId { get; private set; }
		public double ReplayGainTrackGain { get; private set; }
		public double ReplayGainTrackPeak { get; private set; }
		public double ReplayGainAlbumGain { get; private set; }
		public double ReplayGainAlbumPeak { get; private set; }
		public IEnumerable<AudioPicture> Pictures { get; private set; }

		public static implicit operator AudioTags(Tag t)
		{
			var data = new Dictionary<string, List<string>>();

			switch (t)
			{
				case TagLib.Id3v2.Tag id3:
					foreach (var frame in id3.Where(c => c.FrameId != "TXXX" && c.FrameId != "PRIV" && c.FrameId != "MCDI"))
					{
						var key = frame.FrameId.ToString();

						if (!data.ContainsKey(key)) data.Add(key, new List<string>());

						data[key].Add(frame.ToString());
					}

					foreach (var frame in id3.Where(c => c.FrameId == "TXXX").Cast<UserTextInformationFrame>())
					{
						var key = frame.Description.ToUpperInvariant();

						if (!data.ContainsKey(key)) data.Add(key, new List<string>());

						data[key].AddRange(frame.Text);
					}

					foreach (var frame in id3.Where(c => c.FrameId == "PRIV").Cast<PrivateFrame>())
					{
						var key = frame.FrameId.ToString();

						if (!data.ContainsKey(key)) data.Add(key, new List<string>());

						data[key].Add(frame.Owner);
					}

					foreach (var frame in id3.Where(c => c.FrameId == "MCDI").Cast<MusicCdIdentifierFrame>())
					{
						var key = frame.FrameId.ToString();

						if (!data.ContainsKey(key)) data.Add(key, new List<string>());

						data[key].Add(frame.Data.ToString());
					}

					break;
				case Metadata flac:
					foreach (var c in flac.Tags.Cast<XiphComment>())
					{
						data = c.ToDictionary(f => f, f => c.GetField(f).ToList());
					}

					break;
			}

			var paddedTrack = t.Track.ToString();

			switch (t)
			{
				case TagLib.Id3v2.Tag tag:
					var tt = tag.GetFrames(ByteVector.FromString("TRCK", StringType.UTF8));
					paddedTrack = tt?.FirstOrDefault()?.ToString();
					break;
				case Metadata tag:
					paddedTrack = tag.GetComment(false, t).GetField("TRACKNUMBER").FirstOrDefault();
					break;
			}

			return new AudioTags
			{
				Type = (AudioTagTypes)(uint)t.TagTypes,
				Frames = data,
				TrackNumber = paddedTrack,
				TrackNumberValue = t.Track,
				Title = t.Title,
				TitleSort = t.TitleSort,
				Album = t.Album,
				AlbumSort = t.AlbumSort,
				Artist = string.Join('/', t.Performers),
				ArtistSort = string.Join('/', t.PerformersSort),
				AlbumArtist = string.Join('/', t.AlbumArtists),
				AlbumArtistSort = string.Join('/', t.AlbumArtistsSort),
				Comment = t.Comment,
				Genre = string.Join('/', t.Genres),
				Year = t.Year,
				Composer = string.Join('/', t.Composers),
				ComposerSort = string.Join('/', t.ComposersSort),
				TrackCount = t.TrackCount,
				Disc = t.Disc,
				DiscCount = t.DiscCount,
				Lyrics = t.Lyrics,
				Grouping = t.Grouping,
				BeatsPerMinute = t.BeatsPerMinute,
				Conductor = t.Conductor,
				Copyright = t.Copyright,
				MusicIpId = t.MusicIpId,
				AmazonId = t.AmazonId,
				ReplayGainTrackGain = t.ReplayGainTrackGain,
				ReplayGainTrackPeak = t.ReplayGainTrackPeak,
				ReplayGainAlbumGain = t.ReplayGainAlbumGain,
				ReplayGainAlbumPeak = t.ReplayGainAlbumPeak,
				Pictures = t.Pictures.Select(AudioPicture.FromIPicture)
			};
		}
	}
}
