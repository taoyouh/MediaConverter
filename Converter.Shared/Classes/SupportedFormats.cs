using Converter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;

namespace Converter.Classes
{
    internal static class SupportedFormats
    {
        public static IEnumerable<string> InputFileTypes()
        {
            return new string[]
            {
                "*"
            };
        }

        public static IEnumerable<TranscodeConfigViewModel> TranscodeConfigs()
        {
            return new TranscodeConfigViewModel[]
            {
                new TranscodeConfigViewModel()
                {
                    DisplayName = "ALAC",
                    Configuration = TranscodeConfiguration.CreateAlac()
                },
                new TranscodeConfigViewModel()
                {
                    DisplayName = "FLAC",
                    Configuration = TranscodeConfiguration.CreateFlac()
                },
                new TranscodeConfigViewModel()
                {
                    DisplayName = "MP3",
                    Configuration = TranscodeConfiguration.CreateMp3()
                },
                new TranscodeConfigViewModel()
                {
                    DisplayName = "M4A (AAC)",
                    Configuration = TranscodeConfiguration.CreateAac()
                },
                new TranscodeConfigViewModel()
                {
                    DisplayName = "WMA9",
                    Configuration = TranscodeConfiguration.CreateWma()
                },
                new TranscodeConfigViewModel
                {
                    DisplayName = "AC3",
                    Configuration = new TranscodeConfiguration(
                        MediaEncodingSubtypes.Mpeg4,
                        "AC3",
                        null)
                },
                new TranscodeConfigViewModel
                {
                    DisplayName = "MP4 (AVC + AAC)",
                    Configuration = TranscodeConfiguration.CreateMp4Avc()
                }
            };
        }
    }
}
