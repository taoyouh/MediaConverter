using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;

namespace Converter.Classes
{
    internal class TranscodingManager
    {
        private static TranscodingManager _current;

        public static TranscodingManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new TranscodingManager();
                }

                return _current;
            }
        }

        public TranscodingManager()
        {
            Tasks.CollectionChanged += Tasks_CollectionChanged;
        }

        private void Tasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (TranscodeTask item in e.OldItems)
                {
                    item.PropertyChanged -= Task_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (TranscodeTask item in e.NewItems)
                {
                    item.PropertyChanged += Task_PropertyChanged;
                }
            }

            if (Tasks.All(x => x.Status != TranscodeTask.TranscodeStatus.InProgress))
            {
                DisposeExtendedExecutionSession();
            }
            else
            {
                RequestExtentedExecution();
            }
        }

        private void Task_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TranscodeTask.Status)
                && Tasks.All(x => x.Status != TranscodeTask.TranscodeStatus.InProgress))
            {
                DisposeExtendedExecutionSession();
            }
            else
            {
                RequestExtentedExecution();
            }

            if (e.PropertyName == nameof(TranscodeTask.Progress)
                && extendedExeSession != null)
            {
                extendedExeSession.PercentProgress = (uint)Tasks.Average(x => x.Progress);
            }
        }

        public ObservableCollection<TranscodeTask> Tasks
        { get; } = new ObservableCollection<TranscodeTask>();

        private static ExtendedExecutionSession extendedExeSession;

        private async void RequestExtentedExecution()
        {
            if (extendedExeSession == null)
            {
                extendedExeSession = new ExtendedExecutionSession();
                extendedExeSession.Reason = ExtendedExecutionReason.Unspecified;
                extendedExeSession.Description = "后台格式转换";

                var extendedExeResult = await extendedExeSession.RequestExtensionAsync();
                switch (extendedExeResult)
                {
                    case ExtendedExecutionResult.Allowed:
                        break;
                    case ExtendedExecutionResult.Denied:
                        extendedExeSession.Dispose();
                        extendedExeSession = null;
                        break;
                }
            }
        }

        private void DisposeExtendedExecutionSession()
        {
            if (extendedExeSession != null)
            {
                extendedExeSession.Dispose();
                extendedExeSession = null;
            }
        }
    }
}
