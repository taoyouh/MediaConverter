using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using static Converter.Classes.TranscodeTask;

namespace Converter.Classes
{
    internal class TranscodeTaskManager
    {
        private const string TasksFinished = "TranscodingManager_TasksFinished";
        private const string SessionDescription = "TranscodingManager_SessionDescription";
        private const string ExtendedExecutionRevoked = "TranscodingManager_ExtendedExecutionRevoked";
        private const string CantDeserialize = "TranscodingManager_CantDeserialize";
        private const string TranscodingManagerNotificationGroup = "Mgr";
        private const string SaveFileName = "tasks.xml";

        private static TranscodeTaskManager _current;

        public static TranscodeTaskManager Current
        {
            get => _current;
            set => _current = value;
        }

        public TranscodeTaskManager()
        {
            Tasks.CollectionChanged += Tasks_CollectionChanged;
        }

        private bool _toastNotificationsEnabled;

        public bool ToastNotificationsEnabled
        {
            get
            {
                return _toastNotificationsEnabled;
            }

            set
            {
                if (value == _toastNotificationsEnabled)
                {
                    return;
                }

                if (!value)
                {
                    ToastNotificationManager.History.RemoveGroup(TranscodingManagerNotificationGroup);
                }

                _toastNotificationsEnabled = value;
            }
        }

        private void Tasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (TranscodeTask item in e.OldItems)
                {
                    item.StatusChanged -= TranscodeTask_StatusChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (TranscodeTask item in e.NewItems)
                {
                    item.StatusChanged += TranscodeTask_StatusChanged;
                }
            }

            if (Tasks.All(x => x.Status != TranscodeStatus.InProgress))
            {
                AllTasksFinished();
            }
            else
            {
                TaskStarted();
            }
        }

        private void TranscodeTask_StatusChanged(object sender, TranscodeStatus e)
        {
            if (Tasks.All(x => x.Status != TranscodeStatus.InProgress))
            {
                AllTasksFinished();
            }
            else
            {
                TaskStarted();
            }
        }

        public ObservableCollection<TranscodeTask> Tasks
        { get; } = new ObservableCollection<TranscodeTask>();

        private static ExtendedExecutionSession extendedExeSession;

        private async void TaskStarted()
        {
            if (extendedExeSession == null)
            {
                extendedExeSession = new ExtendedExecutionSession();
                extendedExeSession.Reason = ExtendedExecutionReason.Unspecified;
                extendedExeSession.Description = new ResourceLoader().GetString(SessionDescription);

                var extendedExeResult = await extendedExeSession.RequestExtensionAsync();
                switch (extendedExeResult)
                {
                    case ExtendedExecutionResult.Allowed:
                        extendedExeSession.Revoked += ExtendedExeSession_Revoked;
                        break;
                    case ExtendedExecutionResult.Denied:
                        extendedExeSession.Dispose();
                        extendedExeSession = null;
                        break;
                }
            }
        }

        private void ExtendedExeSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            if (args.Reason == ExtendedExecutionRevokedReason.SystemPolicy)
            {
                PopToastIfEnabled(new ResourceLoader().GetString(ExtendedExecutionRevoked));
            }
        }

        private void AllTasksFinished()
        {
            PopToastIfEnabled(new ResourceLoader().GetString(TasksFinished));

            if (extendedExeSession != null)
            {
                extendedExeSession.Revoked -= ExtendedExeSession_Revoked;
                extendedExeSession.Dispose();
                extendedExeSession = null;
            }
        }

        private void PopToastIfEnabled(string message)
        {
            if (!ToastNotificationsEnabled)
            {
                return;
            }

            ToastContent toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = message
                            }
                        }
                    }
                }
            };
            var notification = new ToastNotification(toastContent.GetXml())
            {
                Group = TranscodingManagerNotificationGroup
            };
            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }

        public async Task SaveAsync()
        {
            List<TranscodeTaskData> taskDataList = new List<TranscodeTaskData>();
            var fa = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
            fa.Clear();
            foreach (var task in Tasks)
            {
                if (task.Status == TranscodeStatus.InProgress)
                {
                    task.Cancel();
                    if (fa.Entries.Count < fa.MaximumItemsAllowed)
                    {
                        if (!fa.CheckAccess(task.Destination))
                        {
                            if (task.DestFolder != null)
                            {
                                fa.Add(task.DestFolder);
                            }
                            else
                            {
                                fa.Add(task.Destination);
                            }
                        }
                    }

                    if (fa.Entries.Count < fa.MaximumItemsAllowed)
                    {
                        if (!fa.CheckAccess(task.Source))
                        {
                            fa.Add(task.Source);
                        }
                    }

                    taskDataList.Add(new TranscodeTaskData
                    {
                        SourcePath = task.Source.Path,
                        DestPath = task.Destination.Path,
                        OutputFileName = task.OutputFileName,
                        HasAudio = task.Config.Audio != null,
                        AudioBitRate = task.Config.Audio?.Bitrate,
                        AudioBitsPerSample = task.Config.Audio?.BitsPerSample,
                        AudioChannelCount = task.Config.Audio?.ChannelCount,
                        AudioChannelMask = task.Config.Audio?.ChannelMask,
                        AudioSubtype = task.Config.Audio?.Subtype,
                        AudioSampleRate = task.Config.Audio?.SampleRate,
                        HasVideo = task.Config.Video != null,
                        VideoBitrate = task.Config.Video?.Bitrate,
                        VideoFrameRateDenominator = task.Config.Video?.FrameRate?.Denominator,
                        VideoFrameRateNumerator = task.Config.Video?.FrameRate?.Numerator,
                        VideoHeight = task.Config.Video?.Height,
                        VideoPixelAspectRatioDenominator = task.Config.Video?.PixelAspectRatio?.Denominator,
                        VideoPixelAspectRatioNumerator = task.Config.Video?.PixelAspectRatio?.Numerator,
                        VideoSubtype = task.Config.Video?.Subtype,
                        VideoWidth = task.Config.Video?.Width,
                        ContainerSubtype = task.Config.Container.Subtype
                    });
                }
            }

            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                SaveFileName, CreationCollisionOption.ReplaceExisting);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<TranscodeTaskData>));
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                serializer.Serialize(stream, taskDataList);
            }

        }

        /// <summary>
        /// 从状态保存文件<see cref="SaveFileName"/>中加载之前未完成的任务
        /// </summary>
        /// <exception cref="LoadFailedException">
        /// 当状态保存文件存在，但无法从状态恢复
        /// </exception>
        /// <returns>无返回值，可等待</returns>
        public async Task LoadAsync()
        {
            StorageFile file;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync(SaveFileName);
            }
            catch (FileNotFoundException)
            {
                return;
            }

            List<TranscodeTaskData> taskDataErrorList = new List<TranscodeTaskData>();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<TranscodeTaskData>));
            using (var stream = await file.OpenStreamForReadAsync())
            {
                List<TranscodeTaskData> taskDataList;
                try
                {
                    taskDataList = (List<TranscodeTaskData>)serializer.Deserialize(stream);
                }
                catch (InvalidOperationException ex)
                {
                    var message = new ResourceLoader().GetString(CantDeserialize);
                    throw new LoadFailedException(message, ex);
                }

                foreach (var taskData in taskDataList)
                {
                    try
                    {
                        StorageFile source = await StorageFile.GetFileFromPathAsync(taskData.SourcePath);
                        StorageFile destination = await StorageFile.GetFileFromPathAsync(taskData.DestPath);
                        TranscodeConfiguration config = new TranscodeConfiguration
                        {
                            Audio = taskData.HasAudio ?
                                new AudioConfiguration
                                {
                                    Bitrate = taskData.AudioBitRate,
                                    BitsPerSample = taskData.AudioBitsPerSample,
                                    ChannelCount = taskData.AudioChannelCount,
                                    ChannelMask = taskData.AudioChannelMask,
                                    SampleRate = taskData.AudioSampleRate,
                                    Subtype = taskData.AudioSubtype
                                } :
                                null,
                            Video = taskData.HasVideo ?
                                new VideoConfiguration
                                {
                                    Bitrate = taskData.VideoBitrate,
                                    FrameRate = taskData.VideoFrameRateDenominator.HasValue ?
                                        new Ratio(
                                            taskData.VideoFrameRateDenominator.Value,
                                            taskData.VideoFrameRateNumerator.Value) :
                                        null,
                                    Height = taskData.VideoHeight,
                                    PixelAspectRatio = taskData.VideoPixelAspectRatioDenominator.HasValue ?
                                        new Ratio(
                                            taskData.VideoPixelAspectRatioDenominator.Value,
                                            taskData.VideoPixelAspectRatioNumerator.Value) :
                                        null,
                                    Subtype = taskData.VideoSubtype,
                                    Width = taskData.VideoWidth
                                } :
                                null,
                            Container = new ContainerConfiguration
                            {
                                Subtype = taskData.ContainerSubtype
                            }
                        };
                        TranscodeTask task = new TranscodeTask(source, destination, config);
                        task.OutputFileName = taskData.OutputFileName;
                        Tasks.Add(task);
                        await task.PrepareAsync();
                        task.StartTranscode();
                    }
                    catch
                    {
                        taskDataErrorList.Add(taskData);
                    }
                }
            }

            try
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (FileNotFoundException) { }
        }

        public class LoadFailedException : Exception
        {
            public LoadFailedException(string message, Exception innerException)
                : base(message, innerException)
            { }
        }
    }
}
