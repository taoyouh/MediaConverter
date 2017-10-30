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
        private const string Alac = "ALAC";
        private const string Flac = "FLAC";

        public static TranscodeConfiguration CreateAlac()
        {
            var result = new TranscodeConfiguration()
            {
                Audio = new AudioConfiguration()
                {
                    Subtype = Alac,
                    Bitrate = 96
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = MediaEncodingSubtypes.Mpeg4
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
                    Subtype = Flac,
                    Bitrate = 100
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = Flac
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
                    Subtype = MediaEncodingSubtypes.Aac
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = MediaEncodingSubtypes.Mpeg4
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
                    Subtype = MediaEncodingSubtypes.Mp3
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = MediaEncodingSubtypes.Mp3
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
                    Subtype = MediaEncodingSubtypes.Wma9
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = MediaEncodingSubtypes.Asf
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
                    Subtype = MediaEncodingSubtypes.Aac
                },
                Video = new VideoConfiguration()
                {
                    Subtype = MediaEncodingSubtypes.H264
                },
                Container = new ContainerConfiguration()
                {
                    Subtype = MediaEncodingSubtypes.Mpeg4
                }
            };

            return result;
        }

        public TranscodeConfiguration()
        {
        }

        public TranscodeConfiguration(string containerName, string audioName, string videoName)
        {
            if (audioName != null)
            {
                Audio = new AudioConfiguration
                {
                    Subtype = audioName
                };
            }

            if (containerName != null)
            {
                Container = new ContainerConfiguration
                {
                    Subtype = containerName
                };
            }

            if (videoName != null)
            {
                Video = new VideoConfiguration
                {
                    Subtype = videoName
                };
            }
        }

        public AudioConfiguration Audio { get; set; }

        public ContainerConfiguration Container { get; set; }

        public VideoConfiguration Video { get; set; }

        public KeyValuePair<string, IList<string>> SaveChoice()
        {
            string name;
            string[] fileTypes;

            string containerSubType = Container?.Subtype;

            if (containerSubType == Flac)
            {
                name = "FLAC";
                fileTypes = new string[] { ".flac" };
            }
            else if (containerSubType == MediaEncodingSubtypes.Mpeg4)
            {
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
            }
            else if (containerSubType == MediaEncodingSubtypes.Mp3)
            {
                name = "MP3";
                fileTypes = new string[] { ".mp3" };
            }
            else if (containerSubType == MediaEncodingSubtypes.Asf)
            {
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
            }
            else
            {
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
