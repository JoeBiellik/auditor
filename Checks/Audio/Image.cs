using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Auditor.SDK.Audio;
using Auditor.SDK.Checks;
using Auditor.SDK.Extensions;
using Auditor.SDK.Targets;

namespace Auditor.Checks.Audio
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Image : AudioCheck
	{
		public override string Name => "Image";
		public override Version Version => new Version(1, 0, 0);
		public override string Description => "Checks a song's embedded cover art is present and within a certain resolution";

		public dynamic Present { get; set; } = null; // true for any photo, string or list of PhotoTypes
		public dynamic Absent { get; set; } = null; // true for all photos, unless Present is set, then all which aren't listed as Present, string or list of PhotoTypes

		public CheckSize Size { get; set; } = null;

		public class CheckSize
		{
			public int? Width { get; set; } = null;
			public int? MinWidth { get; set; } = null;
			public int? MaxWidth { get; set; } = null;
			public int? Height { get; set; } = null;
			public int? MinHeight { get; set; } = null;
			public int? MaxHeight { get; set; } = null;
		}

		public override CheckResult Test(AudioTarget target)
		{
			var present = new List<AudioPictureType>();
			var absent = new List<AudioPictureType>();
			var checkPresent = false;
			var checkAbsent = false;
			var errors = new List<string>();

			if (YamlExtensions.IsBool(this.Present))
			{
				checkPresent = YamlExtensions.GetBool(this.Present);
			}

			if (YamlExtensions.IsString(this.Present))
			{
				checkPresent = true;
				string str = YamlExtensions.GetString(this.Present);

				try
				{
					present.Add((AudioPictureType)Enum.Parse(typeof(AudioPictureType), str));
				}
				catch (Exception ex)
				{
					throw new FormatException($"Present target {str} is not valid", ex);
				}
			}

			if (YamlExtensions.IsList(this.Present))
			{
				checkPresent = true;

				foreach (string str in YamlExtensions.GetList<string>(this.Present))
				{
					try
					{
						present.Add((AudioPictureType)Enum.Parse(typeof(AudioPictureType), str));
					}
					catch (Exception ex)
					{
						throw new FormatException($"Present target {str} is not valid", ex);
					}
				}
			}

			if (YamlExtensions.IsBool(this.Absent))
			{
				checkAbsent = YamlExtensions.GetBool(this.Absent);
			}
			else if (YamlExtensions.IsString(this.Absent))
			{
				checkAbsent = true;
				string str = YamlExtensions.GetString(this.Absent);

				try
				{
					absent.Add((AudioPictureType)Enum.Parse(typeof(AudioPictureType), str));
				}
				catch (Exception ex)
				{
					throw new FormatException($"Absent target {str} is not valid", ex);
				}
			}
			else if (YamlExtensions.IsList(this.Absent))
			{
				checkAbsent = true;

				foreach (string str in YamlExtensions.GetList<string>(this.Absent))
				{
					try
					{
						absent.Add((AudioPictureType)Enum.Parse(typeof(AudioPictureType), str));
					}
					catch (Exception ex)
					{
						throw new FormatException($"Absent target {str} is not valid", ex);
					}
				}
			}

			if (checkPresent)
			{
				if (present.Count < 1 && !target.Tags.Pictures.Any())
				{
					return new CheckResult
					{
						Success = false,
						Messages = new[] { "No photos" }
					};
				}

				errors.AddRange(from type in present where target.Tags.Pictures.All(p => p.Type != type) select $"Photo {type} not found");

				if (this.Size != null)
				{
					if (this.Size.Width.HasValue || this.Size.Height.HasValue)
					{
						foreach (var picture in target.Tags.Pictures)
						{
							using var image = picture.Image;

							if (this.Size.Width.HasValue && image.Width != this.Size.Width || this.Size.Height.HasValue && image.Height != this.Size.Height)
							{
								errors.Add($"Photo {picture.Type} size is wrong, {image.Width}x{image.Height}px, not {this.Size.Width}x{this.Size.Height}px");
							}
						}
					}
				}
			}

			if (checkAbsent)
			{
				if (absent.Count < 1 && present.Count > 0)
				{
					var all = Enum.GetValues(typeof(AudioPictureType)).Cast<AudioPictureType>().ToList();

					foreach (var type in present)
					{
						all.Remove(type);
					}

					errors.AddRange(from type in all where target.Tags.Pictures.Any(p => p.Type == type) select $"Photo {type} found");
				}

				errors.AddRange(from type in absent where target.Tags.Pictures.All(p => p.Type == type) select $"Photo {type} found");
			}

			if (errors.Count > 0)
			{
				return new CheckResult
				{
					Success = false,
					Messages = errors
				};
			}

			return new CheckResult
			{
				Success = true
			};
		}
	}
}
