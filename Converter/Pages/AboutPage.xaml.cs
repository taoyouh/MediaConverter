using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Converter.Pages
{
    public sealed partial class AboutPage : Page
    {
        private const string Email = "zhaobang.china@hotmail.com";
        private const string AppId = "9ndjv6k3g2tj";

        public AboutPage()
        {
            this.InitializeComponent();

            appTitleBlock.Text = Package.Current.DisplayName;

            var version = Package.Current.Id.Version;
            versionBlock.Text = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

            publisherBlock.Text = Package.Current.PublisherDisplayName;
            installedDateBlock.Text = Package.Current.InstalledDate.ToString();

            emailBlock.Text = Email;
            emailButton.NavigateUri = new Uri($"mailto:{Email}");

            feedbackCenterButton.Visibility =
                Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported() ?
                Visibility.Visible : Visibility.Collapsed;

            reviewButton.NavigateUri = new Uri($"ms-windows-store://review/?ProductId={AppId}");
        }

        private async void FeedbackCenterButton_Click(object sender, RoutedEventArgs e)
        {
            await Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        }
    }
}
