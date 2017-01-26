using Converter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Classes
{
    internal static class SupportedFormats
    {
        public static IEnumerable<string> InputFileTypes()
        {
            return new string[]
            {
                ".m4a",
                ".m4v",
                ".mp4",
                ".mp3",
                ".mpg",
                ".mpeg",
                ".flac",
                ".mts",
                ".m2ts"
            };
        }

        public static IEnumerable<TranscodeConfigListItem> TranscodeConfigs()
        {
            var a = Windows.Media.MediaProperties.MediaEncodingProfile.CreateAvi(Windows.Media.MediaProperties.VideoEncodingQuality.HD1080p);
            var b = Windows.Media.MediaProperties.MediaEncodingProfile.CreateM4a(Windows.Media.MediaProperties.AudioEncodingQuality.High);
            var c = Windows.Media.MediaProperties.MediaEncodingProfile.CreateMp3(Windows.Media.MediaProperties.AudioEncodingQuality.High);
            var d = Windows.Media.MediaProperties.MediaEncodingProfile.CreateWav(Windows.Media.MediaProperties.AudioEncodingQuality.High);
            var e = Windows.Media.MediaProperties.MediaEncodingProfile.CreateWma(Windows.Media.MediaProperties.AudioEncodingQuality.High);
            var f = Windows.Media.MediaProperties.MediaEncodingProfile.CreateWmv(Windows.Media.MediaProperties.VideoEncodingQuality.HD1080p);

            var g = a;
            return new TranscodeConfigListItem[]
            {
                new TranscodeConfigListItem()
                {
                    DisplayName = "ALAC",
                    Configuration = TranscodeConfiguration.CreateAlac()
                },
                new TranscodeConfigListItem()
                {
                    DisplayName = "FLAC",
                    Configuration = TranscodeConfiguration.CreateFlac()
                },
                new TranscodeConfigListItem()
                {
                    DisplayName = "MP3",
                    Configuration = TranscodeConfiguration.CreateMp3()
                },
                new TranscodeConfigListItem()
                {
                    DisplayName = "M4A (AAC)",
                    Configuration = TranscodeConfiguration.CreateAac()
                },
                new TranscodeConfigListItem()
                {
                    DisplayName = "WMA9",
                    Configuration = TranscodeConfiguration.CreateWma()
                },
                new TranscodeConfigListItem()
                {
                    DisplayName = "MP4 (AVC + AAC)",
                    Configuration = TranscodeConfiguration.CreateMp4Avc()
                }
            };
        }
    }
}
