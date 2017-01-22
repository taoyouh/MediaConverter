using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Converter.Controls
{
    public sealed partial class FileBrowseControl : UserControl
    {
        public IEnumerable<string> OpenFileFilters { get; set; }
        public IEnumerable<KeyValuePair<string, IList<string>>> SaveFileChoices { get; set; }

        public enum BrowseModes { FileOpen, FileSave, Folder}
        public BrowseModes Mode { get; set; }

        public FileBrowseControl()
        {
            this.InitializeComponent();
        }

        private IStorageItem _selectedItem;
        public IStorageItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                urlBox.Text = _selectedItem?.Path ?? "";
            }
        }

        public object Header
        {
            get { return urlBox.Header; }
            set { urlBox.Header = value; }
        }

        public object BrowseButtonContent
        {
            get { return browseButton.Content; }
            set { browseButton.Content = value; }
        }

        private async void browseButton_Click(object sender, RoutedEventArgs e)
        {
            switch(Mode)
            {
                case BrowseModes.FileOpen:
                    FileOpenPicker openPicker = new FileOpenPicker();
                    if (OpenFileFilters != null)
                        foreach (var item in OpenFileFilters)
                            openPicker.FileTypeFilter.Add(item);
                    SelectedItem = await openPicker.PickSingleFileAsync();
                    break;
                case BrowseModes.FileSave:
                    FileSavePicker savePicker = new FileSavePicker();
                    if (SaveFileChoices != null)
                        foreach (var item in SaveFileChoices)
                            savePicker.FileTypeChoices.Add(item);
                    SelectedItem = await savePicker.PickSaveFileAsync();
                    break;
                case BrowseModes.Folder:
                    FolderPicker folderPicker = new FolderPicker();
                    if (OpenFileFilters != null)
                        foreach (var item in OpenFileFilters)
                            folderPicker.FileTypeFilter.Add(item);
                    SelectedItem = await folderPicker.PickSingleFolderAsync();
                    break;
            }
        }
    }
}
