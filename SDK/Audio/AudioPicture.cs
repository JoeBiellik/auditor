using System.Drawing;
using System.IO;
using TagLib;

namespace Auditor.SDK.Audio
{
	public class AudioPicture
	{
		public string MimeType { get; private set; }
		public AudioPictureType Type { get; private set; }
		public string Description { get; private set; }
		public byte[] Data { get; private set; }
		public Image Image => new Bitmap(new MemoryStream(this.Data));

		public static AudioPicture FromIPicture(IPicture p) => new AudioPicture
		{
			MimeType = p.MimeType,
			Type = (AudioPictureType)(int)p.Type,
			Description = p.Description,
			Data = p.Data.Data
		};
	}
}
