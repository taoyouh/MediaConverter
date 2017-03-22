using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.ApplicationModel.Resources;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace Converter.Classes
{
    internal class TranscodingManager
    {
        private const string TasksFinished = "TranscodingManager_TasksFinished";
        private const string SessionDescription = "TranscodingManager_SessionDescription";
        private const string ExtendedExecutionRevoked = "TranscodingManager_ExtendedExecutionRevoked";
        private const string TranscodingManagerNotificationGroup = "Mgr";

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
                AllTasksFinished();
            }
            else
            {
                TaskStarted();
            }
        }

        private void Task_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TranscodeTask.Status)
                && Tasks.All(x => x.Status != TranscodeTask.TranscodeStatus.InProgress))
            {
                AllTasksFinished();
            }
            else
            {
                TaskStarted();
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
    }
}
