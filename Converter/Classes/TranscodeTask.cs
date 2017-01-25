using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Converter.Classes
{
    internal class TranscodeTask : DependencyObject, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private MediaTranscoder transcoder = new MediaTranscoder();
        private StorageFile source;
        private StorageFile destination;
        private string outputFileName;
        private TranscodeConfiguration config;
        private PrepareTranscodeResult prepareResult;

        public TranscodeTask(StorageFile source, StorageFile destination, TranscodeConfiguration config)
        {
            this.source = source;
            this.destination = destination;
            outputFileName = this.destination.Name;
            this.config = config;
            _status = TranscodeStatus.Created;
            _progress = 0;
        }

        public async Task PrepareAsync()
        {
            var sourceProfile = await MediaEncodingProfile.CreateFromFileAsync(source);
            var destProfile = config.Profile(sourceProfile);

            await destination.RenameAsync(destination.Name + ".transcodetmp", NameCollisionOption.GenerateUniqueName);

            prepareResult = await transcoder.PrepareFileTranscodeAsync(source, destination, destProfile);

            if (prepareResult.CanTranscode)
            {
                Status = TranscodeStatus.ReadyToStart;
            }
            else
            {
                Status = TranscodeStatus.Error;
            }
        }

        public async void StartTranscode()
        {
            if (prepareResult == null)
            {
                await PrepareAsync();
            }

            if (Status == TranscodeStatus.ReadyToStart)
            {
                Status = TranscodeStatus.InProgress;
                var transcodeTask = prepareResult.TranscodeAsync();
                transcodeTask.Progress += TranscodeTask_ProgressChanged;
                transcodeTask.Completed += TranscodeTask_Completed;
            }
        }

        private async void TranscodeTask_ProgressChanged(IAsyncActionWithProgress<double> sender, double progress)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Progress = progress;
            });
        }

        private async void TranscodeTask_Completed(IAsyncActionWithProgress<double> sender, AsyncStatus asyncStatus)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                switch (asyncStatus)
                {
                    case AsyncStatus.Completed:
                        await destination.RenameAsync(outputFileName, NameCollisionOption.GenerateUniqueName);
                        Status = TranscodeStatus.Completed;
                        Progress = 100;
                        break;
                    case AsyncStatus.Error:
                        Status = TranscodeStatus.Error;
                        break;
                    case AsyncStatus.Canceled:
                        Status = TranscodeStatus.Cancelled;
                        break;
                }
            });
        }

        private double _progress;

        public double Progress
        {
            get
            {
                return _progress;
            }

            set
            {
                if (_progress != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs("Progress"));
                    _progress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Progress"));
                }
            }
        }

        private TranscodeStatus _status;

        public TranscodeStatus Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs("Status"));
                    _status = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }

        public enum TranscodeStatus
        {
            Created,
            ReadyToStart,
            InProgress,
            Completed,
            Cancelled,
            Error
        }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
