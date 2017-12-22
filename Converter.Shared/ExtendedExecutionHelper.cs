using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Notifications;

namespace Converter
{
    internal static class ExtendedExecutionHelper
    {
        private const string SuspendedNotificationGroup = "SuspendedGroup";
        private static readonly object CountSyncRoot = new object();
        private static readonly object SessionSyncRoot = new object();
        private static ExtendedExecutionSession extendedExeSession = null;
        private static int count;

        public static Deferral GetDeferral()
        {
            bool requestSession = false;
            lock (CountSyncRoot)
            {
                ++count;
                requestSession = count == 1;
            }

            if (requestSession)
            {
                RequestExtendedExecutionAsync();
            }

            return new Deferral(TaskFinished);
        }

        private static void TaskFinished()
        {
            bool clearSession = false;
            lock (CountSyncRoot)
            {
                --count;
                clearSession = count == 0;
            }

            if (clearSession)
            {
                ClearExtendedExeSession();
            }
        }

        private static Task RequestExtendedExecutionAsync()
        {
            return Task.Run(() =>
            {
                lock (SessionSyncRoot)
                {
                    if (extendedExeSession != null)
                    {
                        extendedExeSession.Dispose();
                        extendedExeSession = null;
                    }

                    var newSession = new ExtendedExecutionSession();
                    newSession.Reason = ExtendedExecutionReason.Unspecified;
                    newSession.Revoked += ExtendedExecutionRevoked;

                    var asyncTask = newSession.RequestExtensionAsync().AsTask();
                    asyncTask.Wait();
                    ExtendedExecutionResult result = asyncTask.Result;

                    switch (result)
                    {
                        case ExtendedExecutionResult.Allowed:
                            extendedExeSession = newSession;
                            break;
                        default:
                        case ExtendedExecutionResult.Denied:
                            newSession.Dispose();
                            break;
                    }
                }
            });
        }

        private static void ClearExtendedExeSession()
        {
            lock (SessionSyncRoot)
            {
                if (extendedExeSession != null)
                {
                    extendedExeSession.Dispose();
                    extendedExeSession = null;
                }
            }
        }

        private static void ExtendedExecutionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            lock (SessionSyncRoot)
            {
                if (extendedExeSession != null)
                {
                    extendedExeSession.Dispose();
                    extendedExeSession = null;
                }
            }
        }

        public static void OnSuspending()
        {
            if (count > 0)
            {
                var loader = new ResourceLoader();
                ToastContent toastContent = new ToastContent()
                {
                    Visual = new ToastVisual
                    {
                        BindingGeneric = new ToastBindingGeneric
                        {
                            Children =
                            {
                                new AdaptiveText
                                {
                                    Text = loader.GetString("TranscodingManager_ExtendedExecutionRevoked")
                                }
                            }
                        }
                    }
                };
                var notification = new ToastNotification(toastContent.GetXml())
                {
                    Group = SuspendedNotificationGroup
                };
                ToastNotificationManager.CreateToastNotifier().Show(notification);
            }
        }

        public static void OnResuming()
        {
            ToastNotificationManager.History.RemoveGroup(SuspendedNotificationGroup);
        }
    }
}
