using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;

namespace Converter.Classes
{
    internal static class MediaEncodingProfileHelper
    {
        public const uint DefaultBitsPerSample = 16;
        public const uint DefualtChannelCount = 2;
        public const uint DefaultSampleRate = 44100;

        private static readonly Guid MfMtAudioChannelMask = new Guid("55fb5765-644a-4caf-8479-938983bb1588");

        /// <summary>
        /// 将BitsPerSample、ChannelCount, SampleRate、ChannelMask与目标匹配
        /// </summary>
        public static void MatchAudio(this MediaEncodingProfile obj, MediaEncodingProfile target)
        {
            if (target.Audio == null)
            {
                throw new ArgumentException("Target doesn't have audio properties", nameof(target));
            }

            obj.Audio.BitsPerSample = target.Audio.BitsPerSample;
            obj.Audio.ChannelCount = target.Audio.ChannelCount;
            obj.Audio.SampleRate = target.Audio.SampleRate;
            if (target.Audio.Properties.ContainsKey(MfMtAudioChannelMask))
            {
                obj.Audio.Properties[MfMtAudioChannelMask] =
                    target.Audio.Properties[MfMtAudioChannelMask];
            }
            else if (obj.Audio.Properties.ContainsKey(MfMtAudioChannelMask))
            {
                obj.Audio.Properties.Remove(MfMtAudioChannelMask);
            }
        }

        public static uint? GetChannelMask(this AudioEncodingProperties obj)
        {
            try
            {
                return (uint)obj.Properties[MfMtAudioChannelMask];
            }
            catch
            {
                return null;
            }
        }

        public static void SetChannelMask(this AudioEncodingProperties obj, uint? value)
        {
            if (value.HasValue)
            {
                obj.Properties[MfMtAudioChannelMask] = value.Value;
            }
            else if (obj.Properties.ContainsKey(MfMtAudioChannelMask))
            {
                obj.Properties.Remove(MfMtAudioChannelMask);
            }
        }
    }
}
