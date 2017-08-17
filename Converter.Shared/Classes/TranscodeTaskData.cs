using System;
using System.Collections.Generic;
using System.Text;

namespace Converter.Classes
{
    public class TranscodeTaskData
    {
        public string SourcePath { get; set; }

        public string DestPath { get; set; }

        public string OutputFileName { get; set; }

        public string ContainerSubtype { get; set; }

        public bool HasAudio { get; set; }

        public string AudioSubtype { get; set; }

        public uint? AudioBitsPerSample { get; set; }

        public uint? AudioChannelCount { get; set; }

        public uint? AudioSampleRate { get; set; }

        public uint? AudioBitRate { get; set; }

        public uint? AudioChannelMask { get; set; }

        public bool HasVideo { get; set; }

        public string VideoSubtype { get; set; }

        public uint? VideoHeight { get; set; }

        public uint? VideoWidth { get; set; }

        public uint? VideoBitrate { get; set; }

        /// <summary>
        /// 获取或设置帧速率的分母。当帧速率为null时，此项为null。
        /// </summary>
        public uint? VideoFrameRateDenominator { get; set; }

        /// <summary>
        /// 获取或设置帧速率的分子。当帧速率为null时，此项为null。
        /// </summary>
        public uint? VideoFrameRateNumerator { get; set; }

        public uint? VideoPixelAspectRatioDenominator { get; set; }

        public uint? VideoPixelAspectRatioNumerator { get; set; }
    }
}
