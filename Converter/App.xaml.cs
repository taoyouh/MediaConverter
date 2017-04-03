using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Converter
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// 初始化<see cref="App"/>类的新实例。
        /// 这是执行的创作代码的第一行，执行时逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            if (e.PrelaunchActivated)
            {
                // 不处理预启动
                return;
            }

            // 若应用已经启动，则不强制导航到MainPage
            CreateRootFrameAndNavigate(typeof(MainPage), null, false);
        }

        /// <summary>
        /// 将在创建窗口时调用。绑定Activated事件，设定窗口大小，以及绑定OnBackRequested事件
        /// </summary>
        /// <param name="args">有关创建的窗口的信息。</param>
        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            Window.Current.Activated += Window_Activated;

            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 500));
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        private void CreateRootFrameAndNavigate(Type page, object parameter, bool forceNavigate)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // 若不存在rootFrame，则创建一个rootFrame
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.Navigated += OnNavigated;
                rootFrame.NavigationFailed += OnNavigationFailed;
                Window.Current.Content = rootFrame;
            }

            // 若rootFrame是新创建的，或是强制要求导航到页面，则导航到指定的页面
            if (rootFrame.Content == null || forceNavigate)
            {
                rootFrame.Navigate(page, parameter);
            }

            Window.Current.Activate();
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            Frame frame = sender as Frame;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                frame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
            {
                Classes.TranscodingManager.Current.ToastNotificationsEnabled = true;
            }
            else
            {
                Classes.TranscodingManager.Current.ToastNotificationsEnabled = false;
            }
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        /// <param name="sender">导航失败的框架</param>
        /// <param name="e">有关导航失败的详细信息</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
