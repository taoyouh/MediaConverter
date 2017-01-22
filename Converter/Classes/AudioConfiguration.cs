using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;

namespace Converter.Classes
{
    public class AudioConfiguration
    {
        public string Subtype { get; set; }
        public uint? BitsPerSample { get; set; }
        public uint? ChannelCount { get; set; }
        public uint? SampleRate { get; set; }
        public uint? Bitrate { get; set; }
        public uint? ChannelMask { get; set; }

        public AudioEncodingProperties EncodingProperties(AudioEncodingProperties source)
        {
            if (source == null)
                return null;

            var result = new AudioEncodingProperties()
            {
                Subtype = Subtype ?? source.Subtype,
                BitsPerSample = BitsPerSample ?? source.BitsPerSample,
                ChannelCount = ChannelCount ?? source.ChannelCount,
                SampleRate = SampleRate ?? source.SampleRate,
                Bitrate = Bitrate ?? source.Bitrate
            };
            result.SetChannelMask(ChannelMask ?? source.GetChannelMask());

            return result;
        }
    }
}
