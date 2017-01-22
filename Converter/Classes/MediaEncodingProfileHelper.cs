using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;

namespace Converter.Classes
{
    static class MediaEncodingProfileHelper
    {
        public const uint DefaultBitsPerSample = 16;
        public const uint DefualtChannelCount = 2;
        public const uint DefaultSampleRate = 44100;

        static readonly Guid MF_MT_AUDIO_CHANNEL_MASK = new Guid("55fb5765-644a-4caf-8479-938983bb1588");

        /// <summary>
        /// 将BitsPerSample、ChannelCount, SampleRate、ChannelMask与目标匹配
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target"></param>
        public static void MatchAudio(this MediaEncodingProfile obj, MediaEncodingProfile target)
        {
            if (target.Audio == null)
                throw new ArgumentException("Target doesn't have audio properties", nameof(target));
            obj.Audio.BitsPerSample = target.Audio.BitsPerSample;
            obj.Audio.ChannelCount = target.Audio.ChannelCount;
            obj.Audio.SampleRate = target.Audio.SampleRate;
            if (target.Audio.Properties.ContainsKey(MF_MT_AUDIO_CHANNEL_MASK))
            {
                obj.Audio.Properties[MF_MT_AUDIO_CHANNEL_MASK] =
                    target.Audio.Properties[MF_MT_AUDIO_CHANNEL_MASK];
            }
            else if (obj.Audio.Properties.ContainsKey(MF_MT_AUDIO_CHANNEL_MASK))
            {
                obj.Audio.Properties.Remove(MF_MT_AUDIO_CHANNEL_MASK);
            }
        }

        public static uint? GetChannelMask(this AudioEncodingProperties obj)
        {
            try { return (uint)obj.Properties[MF_MT_AUDIO_CHANNEL_MASK]; }
            catch { return null; }
        }

        public static void SetChannelMask(this AudioEncodingProperties obj, uint? value)
        {
            if(value.HasValue)
            {
                obj.Properties[MF_MT_AUDIO_CHANNEL_MASK] = value.Value;
            }
            else if(obj.Properties.ContainsKey(MF_MT_AUDIO_CHANNEL_MASK))
            {
                obj.Properties.Remove(MF_MT_AUDIO_CHANNEL_MASK);
            }
        }

        //public static MediaEncodingProfile CreateAlac()
        //{
        //    MediaEncodingProfile profile = new MediaEncodingProfile()
        //    {
        //        Container = new ContainerEncodingProperties()
        //        {
        //            Subtype = "MPEG4"
        //        },
        //        Audio = new AudioEncodingProperties()
        //        {
        //            Subtype = "ALAC",
        //            Bitrate = 1,
        //            BitsPerSample =DefaultBitsPerSample,
        //            ChannelCount = DefualtChannelCount,
        //            SampleRate = DefaultSampleRate,
        //        },
        //        Video = null
        //    };

        //    return profile;
        //}

        //public static MediaEncodingProfile CreateFlac()
        //{
        //    MediaEncodingProfile profile = new MediaEncodingProfile()
        //    {
        //        Container = new ContainerEncodingProperties()
        //        {
        //            Subtype = "FLAC"
        //        },
        //        Audio = new AudioEncodingProperties()
        //        {
        //            Subtype = "FLAC",
        //            Bitrate = 1,
        //            BitsPerSample = DefaultBitsPerSample,
        //            ChannelCount = DefualtChannelCount,
        //            SampleRate = DefaultSampleRate,
        //        },
        //        Video = null
        //    };

        //    return profile;
        //}
    }
}
