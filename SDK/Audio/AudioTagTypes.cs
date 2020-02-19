using System;
using System.Diagnostics.CodeAnalysis;

namespace Auditor.SDK.Audio
{
	[Flags]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public enum AudioTagTypes : uint
	{
		None = 0U,
		Xiph = 1U,
		Id3v1 = 2U,
		Id3v2 = 4U,
		Ape = 8U,
		Apple = 16U,
		Asf = 32U,
		RiffInfo = 64U,
		MovieId = 128U,
		DivX = 256U,
		FlacMetadata = 512U,
		AudibleMetadata = 1024U,
		TiffIFD = AudibleMetadata,
		XMP = 2048U,
		JpegComment = 4096U,
		GifComment = 8192U,
		Png = 16384U,
		IPTCIIM = 32768U,
		AllTags = 4294967295U
	}
}
