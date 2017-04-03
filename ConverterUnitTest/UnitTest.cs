using Converter.Classes;
using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Media.MediaProperties;
using Windows.Storage;
using System.Threading.Tasks;

namespace ConverterUnitTest
{
    [TestClass]
    public class ConfigsSubtypeMatch
    {
        [TestMethod]
        public void Aac()
        {
            var aac = TranscodeConfiguration.CreateAac();
            var originalAac = MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Auto);
            AssertConfigSubtypeMatch(aac, originalAac);
        }

        [TestMethod]
        public void Mp3()
        {
            var mp3 = TranscodeConfiguration.CreateMp3();
            var originalMp3 = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Auto);
            AssertConfigSubtypeMatch(mp3, originalMp3);
        }

        [TestMethod]
        public void Avc()
        {
            var avc = TranscodeConfiguration.CreateMp4Avc();
            var originalAvc = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);
            AssertConfigSubtypeMatch(avc, originalAvc);
        }

        [TestMethod]
        public void Wma()
        {
            var wma = TranscodeConfiguration.CreateWma();
            var originalWma = MediaEncodingProfile.CreateWma(AudioEncodingQuality.Auto);
            AssertConfigSubtypeMatch(wma, originalWma);
        }

        private void AssertConfigSubtypeMatch(TranscodeConfiguration config, MediaEncodingProfile profile)
        {
            Assert.AreEqual(config.Container.Subtype, profile.Container.Subtype);
            Assert.AreEqual(config.Audio?.Subtype, profile.Audio?.Subtype);
            Assert.AreEqual(config.Video?.Subtype, profile.Video?.Subtype);
        }
    }
}
