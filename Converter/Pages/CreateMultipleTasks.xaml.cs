using Converter.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
        private const string SomeFailed = "CreateMultipleTasks_SomeFailed";
        private const string ProceedButtonLabel = "CreateMultipleTasks_ProceedButtonLabel";
        private const string AbortButtonLabel = "CreateMultipleTasks_AbortButtonLabel";

        public CreateMultipleTasks()
        {
            this.InitializeComponent();

            inputFilesPanel.OpenFileFilters = SupportedFormats.InputFileTypes();

            outputFolderBrowser.FileTypeFilter = new string[] { "*" };
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            loadingControl.IsLoading = true;
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            List<TranscodeTask> tasks = new List<TranscodeTask>(inputFilesPanel.SelectedFiles.Count);
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

                foreach (var source in inputFilesPanel.SelectedFiles)
                {
                    string fileType = config.SaveChoice().Value.FirstOrDefault();
                    string destFileName = fileType != null && fileType != "." ?
                        source.DisplayName + fileType :
                        source.DisplayName;
                    var destination = await outputFolder.CreateFileAsync(
                        destFileName, CreationCollisionOption.GenerateUniqueName);

                    tasks.Add(new TranscodeTask(source, destination, config));
                }

                // 执行转码准备
                await Task.WhenAll(
                  tasks.Select(x => Task.Run(async () =>
                  {
                      await x.PrepareAsync();
                  })));

                // 提示无法转码的文件
                var errorList = tasks.Where(x => x.Status != TranscodeTask.TranscodeStatus.ReadyToStart);
                if (errorList.FirstOrDefault() != null)
                {
                    StringBuilder errorString = new StringBuilder();
                    errorString.AppendFormat(loader.GetString(SomeFailed), errorList.Count());
                    errorString.AppendLine();
                    foreach (var task in errorList)
                    {
                        errorString.AppendLine(task.OutputFileName);
                    }

                    MessageDialog dialog =
                        new MessageDialog(errorString.ToString(), loader.GetString(CreationFailed));
                    UICommand proceedButton = new UICommand(loader.GetString(ProceedButtonLabel));
                    UICommand abortButton = new UICommand(loader.GetString(AbortButtonLabel));
                    dialog.Commands.Add(proceedButton);
                    dialog.Commands.Add(abortButton);
                    dialog.CancelCommandIndex = 1;
                    dialog.DefaultCommandIndex = 0;
                    var result = await dialog.ShowAsync();
                    if (result != proceedButton)
                    {
                        return;
                    }
                }

                while (true)
                {
                    var task = tasks.FirstOrDefault(x => x.Status == TranscodeTask.TranscodeStatus.ReadyToStart);
                    if (task != null)
                    {
                        tasks.Remove(task);
                        task.StartTranscode();
                        TranscodeTaskManager.Current.Tasks.Add(task);
                    }
                    else
                    {
                        break;
                    }
                }

                Frame.GoBack();
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog(ex.Message, loader.GetString(CreationFailed));
                var result = dialog.ShowAsync();
            }
            finally
            {
                while (true)
                {
                    var task = tasks.FirstOrDefault();
                    if (task != null)
                    {
                        tasks.Remove(task);
                        try
                        {
                            await task.DeleteDestinationFileAsync();
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                loadingControl.IsLoading = false;
            }
        }
    }
}
