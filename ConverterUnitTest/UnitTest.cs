using Converter.Classes;
using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Media.MediaProperties;
using Windows.Storage;
using System.Threading.Tasks;

namespace ConverterUnitTest
{
    [TestClass]
    public class TranscodeTestFromFlac
    {
        StorageFile inputFile;
        StorageFile outputFile;

        [TestInitialize]
        public async Task TaskInit()
        {
            inputFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///TestMedia/Flac.flac"));
            var folder = ApplicationData.Current.TemporaryFolder;
            outputFile = await folder.CreateFileAsync("new.test", CreationCollisionOption.GenerateUniqueName);
        }

        [TestMethod]
        public async Task FlacToAac()
        {
            var config = TranscodeConfiguration.CreateAac();
            await TestTranscodingAsync(config);
        }

        [TestMethod]
        public async Task FlacToAlac()
        {
            var config = TranscodeConfiguration.CreateAlac();
            await TestTranscodingAsync(config);
        }

        [TestMethod]
        public async Task FlacToMp3()
        {
            var config = TranscodeConfiguration.CreateMp3();
            await TestTranscodingAsync(config);
        }

        [TestMethod]
        public async Task FlacToWma()
        {
            var config = TranscodeConfiguration.CreateWma();
            await TestTranscodingAsync(config);
        }

        public async Task TestTranscodingAsync(TranscodeConfiguration config)
        {
            var transcodeTask = new TranscodeTask(inputFile, outputFile, config);
            await transcodeTask.PrepareAsync();
            Assert.AreEqual(TranscodeTask.TranscodeStatus.ReadyToStart, transcodeTask.Status);
            transcodeTask.StartTranscode();
        }
    }

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

        private static void AssertConfigSubtypeMatch(TranscodeConfiguration config, MediaEncodingProfile profile)
        {
            AssertStringCaseInsensitive(profile.Container.Subtype, config.Container.Subtype);
            AssertStringCaseInsensitive(profile.Audio?.Subtype, config.Audio?.Subtype);
            AssertStringCaseInsensitive(profile.Video?.Subtype, config.Video?.Subtype);
        }

        private static void AssertStringCaseInsensitive(string expected, string actual)
        {
            Assert.AreEqual(expected?.ToUpperInvariant(), actual?.ToUpperInvariant());
        }
    }
}
