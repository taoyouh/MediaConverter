using Converter.Classes;
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
using Windows.UI.Core;
using Windows.UI.Xaml;
using static Converter.Classes.TranscodeTask;

namespace Converter.ViewModels
{
    public class TranscodeTaskViewModel : DependencyObject, INotifyPropertyChanged
    {
        private const string Cancelled = "TranscodeStatus_Cancelled";
        private const string Completed = "TranscodeStatus_Completed";
        private const string Created = "TranscodeStatus_Created";
        private const string Error = "TranscodeStatus_Error";
        private const string InProgress = "TranscodeStatus_InProgress";
        private const string Preparing = "TranscodeStatus_Preparing";
        private const string ReadyToStart = "TranscodeStatus_ReadyToStart";
        private const string Unknown = "TranscodeStatus_Unknown";

        private readonly TranscodeTask task;

        public TranscodeTask Task
        {
            get
            {
                return task;
            }
        }

        public TranscodeTaskViewModel(TranscodeTask task)
        {
            this.task = task;
            this.task.ProgressChanged += Task_ProgressChanged;
            this.task.StatusChanged += Task_StatusChanged;
        }

        public string OutputFileName
        {
            get
            {
                return task.OutputFileName;
            }
        }

        public double Progress
        {
            get
            {
                return task.Progress;
            }
        }

        private async void Task_ProgressChanged(object sender, double e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
            });
        }

        public string StatusString
        {
            get
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                switch (task.Status)
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

        private async void Task_StatusChanged(object sender, TranscodeStatus e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusString)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OpenFileButtonVisibility)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OpenFolderButtonVisibility)));
            });
        }

        public Visibility OpenFileButtonVisibility
        {
            get
            {
                return task.Status == TranscodeStatus.Completed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility OpenFolderButtonVisibility
        {
            get
            {
                if (task.Status == TranscodeStatus.Completed && task.DestFolder != null)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public async void OpenFile()
        {
            await Windows.System.Launcher.LaunchFileAsync(task.Destination);
        }

        public async void OpenFolder()
        {
            await Windows.System.Launcher.LaunchFolderAsync(task.DestFolder);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
