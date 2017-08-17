using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;

namespace Converter.Classes
{
    public class VideoConfiguration
    {
        public string Subtype { get; set; }

        public uint? Height { get; set; }

        public uint? Width { get; set; }

        public uint? Bitrate { get; set; }

        public Ratio FrameRate { get; set; }

        public Ratio PixelAspectRatio { get; set; }

        public VideoEncodingProperties EncodingProfiles(VideoEncodingProperties source)
        {
            if (source == null)
            {
                return null;
            }

            var result = new VideoEncodingProperties()
            {
                Subtype = Subtype ?? source.Subtype,
                Height = Height ?? source.Height,
                Width = Width ?? source.Width,
                Bitrate = Bitrate ?? source.Bitrate
            };
            result.FrameRate.Denominator = FrameRate?.Denominator ?? source.FrameRate.Denominator;
            result.FrameRate.Numerator = FrameRate?.Numerator ?? source.FrameRate.Numerator;
            result.PixelAspectRatio.Denominator = PixelAspectRatio?.Denominator ?? source.PixelAspectRatio.Denominator;
            result.PixelAspectRatio.Numerator = PixelAspectRatio?.Numerator ?? source.PixelAspectRatio.Numerator;

            return result;
        }
    }
}
