using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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

            VisualStateManager.GoToState(this, nameof(noSelectionState), false);
            VisualStateManager.GoToState(this, nameof(noFileState), false);

            _selectedFiles.CollectionChanged += SelectedFiles_CollectionChanged;
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

        private void FileList_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Link;
            }
        }

        private async void FileList_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                loadingControl.IsLoading = true;
                try
                {
                    var items = await e.DataView.GetStorageItemsAsync();
                    foreach (var storageItem in items)
                    {
                        await AddToSelectionAsync(storageItem);
                    }
                }
                finally
                {
                    loadingControl.IsLoading = false;
                }
            }
        }

        private async Task AddToSelectionAsync(IStorageItem itemToAdd)
        {
            if (itemToAdd is StorageFile)
            {
                var file = itemToAdd as StorageFile;
                if (OpenFileFilters.Contains(file.FileType))
                {
                    _selectedFiles.Add(file);
                }
            }
            else if (itemToAdd is StorageFolder)
            {
                var folder = itemToAdd as StorageFolder;
                var childrenItems = await folder.GetItemsAsync();
                foreach (var childrenItem in childrenItems)
                {
                    await AddToSelectionAsync(childrenItem);
                }
            }
        }

        private void SelectedFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            string targetState = _selectedFiles.Count == 0 ?
                nameof(noFileState) :
                nameof(haveFileState);
            VisualStateManager.GoToState(this, targetState, true);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                var item = fileList.SelectedItems.FirstOrDefault(x => x is StorageFile) as StorageFile;
                if (item != null)
                {
                    _selectedFiles.Remove(item);
                }
                else
                {
                    break;
                }
            }
        }

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string targetState = fileList.SelectedItems.Count == 0 ?
                nameof(noSelectionState) :
                nameof(haveSelectionState);
            VisualStateManager.GoToState(this, targetState, true);
        }
    }
}
