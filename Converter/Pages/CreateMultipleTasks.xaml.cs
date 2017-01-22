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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Converter.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CreateMultipleTasks : Page
    {
        public CreateMultipleTasks()
        {
            this.InitializeComponent();

            inputFilesPanel.OpenFileFilters = SupportedInputFiles.GetList();

            outputFolderBrowser.OpenFileFilters = new string[] { "*" };
        }

        private async void submitButton_Click(object sender, RoutedEventArgs e)
        {
            var config = formatPicker.SelectedConfiguration;
            if (config == null) throw new Exception();

            var outputFolder = outputFolderBrowser.SelectedItem as StorageFolder;
            if (outputFolder == null) throw new Exception();

            List<TranscodeTask> tasks = new List<TranscodeTask>(inputFilesPanel.SelectedFiles.Count);

            foreach (var source in inputFilesPanel.SelectedFiles)
            {
                string destFileName = config.SaveChoice().Value.First() != "." ?
                    source.DisplayName + config.SaveChoice().Value.First() :
                    source.DisplayName;
                var destination = await outputFolder.CreateFileAsync
                    (destFileName, CreationCollisionOption.GenerateUniqueName);

                tasks.Add(new TranscodeTask(source, destination, config));
            }

            await Task.WhenAll(
              tasks.Select(x => Task.Run(async () =>
              {
                  await x.PrepareAsync();
              })));

            foreach(var task in tasks)
            {
                task.StartTranscode();
                TranscodingManager.Tasks.Add(task);
            }

            Frame.GoBack();
        }
    }
}
