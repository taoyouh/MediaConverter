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
    public class TranscodeTask : DependencyObject, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private const string Cancelled = "TranscodeStatus_Cancelled";
        private const string Completed = "TranscodeStatus_Completed";
        private const string Created = "TranscodeStatus_Created";
        private const string Error = "TranscodeStatus_Error";
        private const string InProgress = "TranscodeStatus_InProgress";
        private const string Preparing = "TranscodeStatus_Preparing";
        private const string ReadyToStart = "TranscodeStatus_ReadyToStart";
        private const string Unknown = "TranscodeStatus_Unknown";

        private MediaTranscoder transcoder = new MediaTranscoder();
        private StorageFile source;
        private StorageFile destination;
        private string outputFileName;
        private TranscodeConfiguration config;
        private PrepareTranscodeResult prepareResult;

        /// <summary>
        /// 创建TranscodeTask实例。
        /// </summary>
        /// <param name="source">转码任务的输入文件（不能为null）</param>
        /// <param name="destination">转码任务的输出文件（不能为null）</param>
        /// <param name="config">转码任务的配置（不能为null）</param>
        /// <exception cref="ArgumentNullException">
        /// 当任意参数为null时引发异常。
        /// </exception>
        public TranscodeTask(StorageFile source, StorageFile destination, TranscodeConfiguration config)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this.source = source;
            this.destination = destination;
            outputFileName = this.destination.Name;
            this.config = config;
            _status = TranscodeStatus.Created;
            _progress = 0;
        }

        /// <summary>
        /// 调用此方法以准备转码。
        /// 当状态为Created时可以调用此方法。调用后进入Preparing状态。
        /// 若可以转码，则进入ReadyToStart状态并将文件名改为临时文件，否则进入Error状态。
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// 当状态部位Created时调用此方法引发异常。
        /// </exception>
        /// <returns>无返回值，可等待。</returns>
        public async Task PrepareAsync()
        {
            if (Status != TranscodeStatus.Created)
            {
                throw new InvalidOperationException();
            }
            else
            {
                Status = TranscodeStatus.Preparing;
            }

            try
            {
                var sourceProfile = await MediaEncodingProfile.CreateFromFileAsync(source);
                var destProfile = config.Profile(sourceProfile);

                prepareResult = await transcoder.PrepareFileTranscodeAsync(source, destination, destProfile);

                if (prepareResult.CanTranscode)
                {
                    Status = TranscodeStatus.ReadyToStart;
                    await destination.RenameAsync(destination.Name + ".transcodetmp", NameCollisionOption.GenerateUniqueName);
                }
                else
                {
                    Status = TranscodeStatus.Error;
                }
            }
            catch (Exception)
            {
                Status = TranscodeStatus.Error;
            }
        }

        /// <summary>
        /// 状态为ReadyStart时可调用，调用后进入InProgress状态。
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// 若调用时状态不为ReadyToStart，则引发异常。
        /// </exception>
        public void StartTranscode()
        {
            if (prepareResult == null || Status != TranscodeStatus.ReadyToStart)
            {
                throw new InvalidOperationException();
            }

            Status = TranscodeStatus.InProgress;
            var transcodeTask = prepareResult.TranscodeAsync();
            transcodeTask.Progress += TranscodeTask_ProgressChanged;
            transcodeTask.Completed += TranscodeTask_Completed;
        }

        /// <summary>
        /// 删除目标文件并将状态调整为Cancelled。
        /// </summary>
        /// <returns>
        /// 无返回值，可等待。
        /// </returns>
        public async Task DeleteDestinationFileAsync()
        {
            while (Status == TranscodeStatus.Preparing)
            {
                await Task.Delay(200);
            }

            if (Status == TranscodeStatus.ReadyToStart)
            {
                var result = prepareResult.TranscodeAsync();
                result.Cancel();
            }

            Status = TranscodeStatus.Cancelled;
            await destination.DeleteAsync(StorageDeleteOption.PermanentDelete);
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

        public string OutputFileName
        {
            get { return outputFileName; }
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
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Status)));
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(StatusString)));
                    _status = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusString)));
                }
            }
        }

        public enum TranscodeStatus
        {
            Created,
            Preparing,
            ReadyToStart,
            InProgress,
            Completed,
            Cancelled,
            Error
        }

        public string StatusString
        {
            get
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                switch (Status)
                {
                    case TranscodeStatus.Cancelled:
                        return loader.GetString(Cancelled);
                    case TranscodeStatus.Completed:
                        return loader.GetString(Completed);
                    case TranscodeStatus.Created:
                        return loader.GetString(Created);
                    case TranscodeStatus.Error:
                        return loader.GetString(Error);
                    case TranscodeStatus.InProgress:
                        return loader.GetString(InProgress);
                    case TranscodeStatus.Preparing:
                        return loader.GetString(Preparing);
                    case TranscodeStatus.ReadyToStart:
                        return loader.GetString(ReadyToStart);
                    default:
                        return loader.GetString(Unknown);
                }
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
