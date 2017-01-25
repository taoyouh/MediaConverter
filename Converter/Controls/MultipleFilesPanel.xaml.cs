using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Converter.Controls
{
    public sealed partial class MultipleFilesPanel : UserControl
    {
        public MultipleFilesPanel()
        {
            this.InitializeComponent();

            fileList.DataContext = _selectedFiles;
        }

        private ObservableCollection<StorageFile> _selectedFiles =
            new ObservableCollection<StorageFile>();

        public IReadOnlyCollection<StorageFile> SelectedFiles
            => _selectedFiles as IReadOnlyCollection<StorageFile>;

        public IEnumerable<string> OpenFileFilters { get; set; }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            if (OpenFileFilters != null)
            {
                foreach (var item in OpenFileFilters)
                {
                    openPicker.FileTypeFilter.Add(item);
                }
            }

            foreach (var file in await openPicker.PickMultipleFilesAsync())
            {
                _selectedFiles.Add(file);
            }
        }
    }
}
