using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;

namespace Converter.Classes
{
    public class TranscodeTask
    {
        private readonly StorageFile source;
        private readonly StorageFile _destination;
        private readonly string _outputFileName;
        private readonly StorageFolder _destFolder;

        private readonly MediaTranscoder transcoder = new MediaTranscoder();
        private readonly TranscodeConfiguration config;

        private PrepareTranscodeResult prepareResult;
        private double _progress;
        private TranscodeStatus _status;

        /// <summary>
        /// 创建TranscodeTask实例。
        /// </summary>
        /// <param name="source">转码任务的输入文件（不能为null）</param>
        /// <param name="destination">转码任务的输出文件（不能为null）</param>
        /// <param name="config">转码任务的配置（不能为null）</param>
        /// <exception cref="ArgumentNullException">
        /// 当任意参数为null时引发异常。
        /// </exception>
        public TranscodeTask(
            StorageFile source,
            StorageFile destination,
            TranscodeConfiguration config,
            StorageFolder destinationFolder = null)
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
            this._destination = destination;
            _outputFileName = this._destination.Name;
            this._destFolder = destinationFolder;
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

                prepareResult = await transcoder.PrepareFileTranscodeAsync(source, _destination, destProfile);

                if (prepareResult.CanTranscode)
                {
                    Status = TranscodeStatus.ReadyToStart;
                    await _destination.RenameAsync(_destination.Name + ".transcodetmp", NameCollisionOption.GenerateUniqueName);
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
            await _destination.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        private void TranscodeTask_ProgressChanged(IAsyncActionWithProgress<double> sender, double progress)
        {
            Progress = progress;
        }

        private async void TranscodeTask_Completed(IAsyncActionWithProgress<double> sender, AsyncStatus asyncStatus)
        {
            switch (asyncStatus)
            {
                case AsyncStatus.Completed:
                    await _destination.RenameAsync(_outputFileName, NameCollisionOption.GenerateUniqueName);
                    Status = TranscodeStatus.Completed;
                    Progress = 100;
                    break;
                case AsyncStatus.Error:
                    Status = TranscodeStatus.Error;
                    break;
                case AsyncStatus.Canceled:
                    Status = TranscodeStatus.Cancelled;
                    break;
                case AsyncStatus.Started:
                    Status = TranscodeStatus.InProgress;
                    break;
                default:
                    Status = TranscodeStatus.Error;
                    break;
            }
        }

        public string OutputFileName
        {
            get { return _outputFileName; }
        }

        public StorageFile Destination
        {
            get { return _destination; }
        }

        public StorageFolder DestFolder
        {
            get { return _destFolder; }
        }

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
                    _progress = value;
                    ProgressChanged?.Invoke(this, value);
                }
            }
        }

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
                    _status = value;
                    StatusChanged?.Invoke(this, value);
                }
            }
        }

        public event EventHandler<double> ProgressChanged;

        public event EventHandler<TranscodeStatus> StatusChanged;

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
    }
}
