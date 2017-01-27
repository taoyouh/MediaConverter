using Converter.Classes;
using Converter.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Converter.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CreateTask : Page
    {
        public CreateTask()
        {
            this.InitializeComponent();

            inputFilePicker.OpenFileFilters = SupportedFormats.InputFileTypes();

            outputFilePicker.SaveFileChoices = new List<KeyValuePair<string, IList<string>>>()
            {
                (formatPicker.SelectedConfiguration ?? new TranscodeConfiguration()).SaveChoice()
            };
        }

        private void FormatPicker_SelectedConfigChanged(object sender, EventArgs newConfig)
        {
            var formatPicker = sender as TranscodeConfigPicker;
            outputFilePicker.SaveFileChoices = new List<KeyValuePair<string, IList<string>>>()
            {
                (formatPicker?.SelectedConfiguration
                ?? new TranscodeConfiguration()).SaveChoice()
            };
        }

        private async void SubmitButtion_Click(object sender, RoutedEventArgs e)
        {
            var config = formatPicker.SelectedConfiguration;
            if (config == null)
            {
                throw new Exception();
            }

            var source = inputFilePicker.SelectedItem as StorageFile;
            if (source == null)
            {
                throw new Exception();
            }

            var destination = outputFilePicker.SelectedItem as StorageFile;
            if (destination == null)
            {
                throw new Exception();
            }

            TranscodeTask task = new TranscodeTask(source, destination, config);
            await task.PrepareAsync();
            if (task.Status == TranscodeTask.TranscodeStatus.ReadyToStart)
            {
                task.StartTranscode();
                TranscodingManager.Tasks.Add(task);
                Frame.GoBack();
            }
        }
    }
}