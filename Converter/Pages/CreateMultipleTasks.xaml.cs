using Converter.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class CreateMultipleTasks : Page
    {
        private const string CreationFailed = "CreateMultipleTasks_CreationFailed";
        private const string SpecifyConfig = "CreateMultipleTasks_SpecifyConfig";
        private const string SpecifyOutput = "CreateMultipleTasks_SpecifyOutput";
        private const string SpecifyInput = "CreateMultipleTasks_SpecifyInput";

        public CreateMultipleTasks()
        {
            this.InitializeComponent();

            inputFilesPanel.OpenFileFilters = SupportedFormats.InputFileTypes();

            outputFolderBrowser.OpenFileFilters = new string[] { "*" };
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            try
            {
                if (inputFilesPanel.SelectedFiles.Count == 0)
                {
                    throw new Exception(loader.GetString(SpecifyInput));
                }

                var config = formatPicker.SelectedConfiguration;
                if (config == null)
                {
                    throw new Exception(loader.GetString(SpecifyConfig));
                }

                var outputFolder = outputFolderBrowser.SelectedItem as StorageFolder;
                if (outputFolder == null)
                {
                    throw new Exception(loader.GetString(SpecifyOutput));
                }

                List<TranscodeTask> tasks = new List<TranscodeTask>(inputFilesPanel.SelectedFiles.Count);

                foreach (var source in inputFilesPanel.SelectedFiles)
                {
                    string destFileName = config.SaveChoice().Value.First() != "." ?
                        source.DisplayName + config.SaveChoice().Value.First() :
                        source.DisplayName;
                    var destination = await outputFolder.CreateFileAsync(
                        destFileName, CreationCollisionOption.GenerateUniqueName);

                    tasks.Add(new TranscodeTask(source, destination, config));
                }

                await Task.WhenAll(
                  tasks.Select(x => Task.Run(async () =>
                  {
                      await x.PrepareAsync();
                  })));

                foreach (var task in tasks)
                {
                    task.StartTranscode();
                    TranscodingManager.Tasks.Add(task);
                }

                Frame.GoBack();
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog(ex.Message, loader.GetString(CreationFailed));
                var result = dialog.ShowAsync();
            }
        }
    }
}
