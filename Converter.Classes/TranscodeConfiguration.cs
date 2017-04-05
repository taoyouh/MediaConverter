using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;

namespace Converter.Classes
{
    public class TranscodeConfiguration
    {
        public static TranscodeConfiguration CreateAlac()
        {
            var result = new TranscodeConfiguration()
            {
                Audio = new AudioConfiguration()
                {
                    Subtype = "ALAC",
                    Bitrate = 96
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = "MPEG4"
                }
            };

            return result;
        }

        public static TranscodeConfiguration CreateFlac()
        {
            var result = new TranscodeConfiguration()
            {
                Audio = new AudioConfiguration()
                {
                    Subtype = "FLAC",
                    Bitrate = 100
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = "FLAC"
                }
            };

            return result;
        }

        public static TranscodeConfiguration CreateAac()
        {
            var result = new TranscodeConfiguration()
            {
                Audio = new AudioConfiguration()
                {
                    Subtype = "AAC"
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = "MPEG4"
                }
            };

            return result;
        }

        public static TranscodeConfiguration CreateMp3()
        {
            var result = new TranscodeConfiguration()
            {
                Audio = new AudioConfiguration()
                {
                    Subtype = "MP3"
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = "MP3"
                }
            };

            return result;
        }

        public static TranscodeConfiguration CreateWma()
        {
            var result = new TranscodeConfiguration()
            {
                Audio = new AudioConfiguration()
                {
                    Subtype = "WMA9"
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = "ASF"
                }
            };

            return result;
        }

        public static TranscodeConfiguration CreateMp4Avc()
        {
            var result = new TranscodeConfiguration()
            {
                Audio = new AudioConfiguration()
                {
                    Subtype = "AAC",
                },
                Video = new VideoConfiguration()
                {
                    Subtype = "H264",
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = "MPEG4"
                }
            };

            return result;
        }

        public AudioConfiguration Audio { get; set; }

        public ContainerConfiguration Container { get; set; }

        public VideoConfiguration Video { get; set; }

        public KeyValuePair<string, IList<string>> SaveChoice()
        {
            string name;
            string[] fileTypes;
            switch (Container?.Subtype?.ToUpper())
            {
                case "FLAC":
                    name = "FLAC";
                    fileTypes = new string[] { ".flac" };
                    break;
                case "MPEG4":
                    if (Video == null)
                    {
                        name = "MPEG4 Audio";
                        fileTypes = new string[] { ".m4a", ".mp4" };
                    }
                    else
                    {
                        name = "MPEG4 Video";
                        fileTypes = new string[] { ".mp4", ".m4v" };
                    }

                    break;
                case "MP3":
                    name = "MP3";
                    fileTypes = new string[] { ".mp3" };
                    break;
                case "ASF":
                    if (Video == null)
                    {
                        name = "Windows Media Audio";
                        fileTypes = new string[] { ".wma", ".asf" };
                    }
                    else
                    {
                        name = "Windows Media Video";
                        fileTypes = new string[] { ".wmv", ".asf" };
                    }

                    break;
                default:
                    if (Container == null)
                    {
                        name = "Unknown";
                        fileTypes = new string[] { "." };
                    }
                    else
                    {
                        name = Container.Subtype;
                        fileTypes = new string[] { "." + Container.Subtype.ToLower() };
                    }

                    break;
            }

            return new KeyValuePair<string, IList<string>>(name, fileTypes);
        }

        public MediaEncodingProfile Profile(MediaEncodingProfile source)
        {
            var result = new MediaEncodingProfile()
            {
                Audio = Audio?.EncodingProperties(source.Audio),
                Video = Video?.EncodingProfiles(source.Video),
                Container = Container?.EncodingProperties(source.Container)
            };

            return result;
        }
    }
}
