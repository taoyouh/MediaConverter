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
using Windows.UI.Popups;
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
        private const string CreationFailed = "CreateTask_CreationFailed";
        private const string SpecifyConfig = "CreateTask_SpecifyConfig";
        private const string SpecifyInput = "CreateTask_SpecifyInput";
        private const string SpecifyOutput = "CreateTask_SpecifyOutput";
        private const string CannotStart = "CreateTask_CannotStart";

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
            loadingControl.IsLoading = true;
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            try
            {
                var source = inputFilePicker.SelectedItem as StorageFile;
                if (source == null)
                {
                    throw new Exception(loader.GetString(SpecifyInput));
                }

                var config = formatPicker.SelectedConfiguration;
                if (config == null)
                {
                    throw new Exception(loader.GetString(SpecifyConfig));
                }

                var destination = outputFilePicker.SelectedItem as StorageFile;
                if (destination == null)
                {
                    throw new Exception(loader.GetString(SpecifyOutput));
                }

                TranscodeTask task = new TranscodeTask(source, destination, config);
                await task.PrepareAsync();
                if (task.Status != TranscodeTask.TranscodeStatus.ReadyToStart)
                {
                    throw new Exception(loader.GetString(CannotStart));
                }
                else
                {
                    task.StartTranscode();
                    TranscodingManager.Tasks.Add(task);
                    Frame.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog(ex.Message, loader.GetString(CreationFailed));
                var result = dialog.ShowAsync();
            }
            finally
            {
                loadingControl.IsLoading = false;
            }
        }

        private void InputFilePicker_SelectionChanged(object sender, EventArgs e)
        {
            var selectedFile = inputFilePicker.SelectedItem as StorageFile;
            outputFilePicker.SuggestedFileName = selectedFile?.DisplayName ?? string.Empty;
        }
    }
}