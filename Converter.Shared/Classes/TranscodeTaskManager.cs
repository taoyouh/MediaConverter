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
using static Converter.Classes.TranscodeTask;

namespace Converter.Classes
{
    internal class TranscodeTaskManager
    {
        private const string TasksFinished = "TranscodingManager_TasksFinished";
        private const string SessionDescription = "TranscodingManager_SessionDescription";
        private const string ExtendedExecutionRevoked = "TranscodingManager_ExtendedExecutionRevoked";
        private const string TranscodingManagerNotificationGroup = "Mgr";

        private static TranscodeTaskManager _current;

        public static TranscodeTaskManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new TranscodeTaskManager();
                }

                return _current;
            }
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
                    default:
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
