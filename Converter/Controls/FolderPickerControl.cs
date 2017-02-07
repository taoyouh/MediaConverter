using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace Converter.Controls
{
    public sealed class FolderPickerControl : FileBrowseControl
    {
        public IEnumerable<string> FileTypeFilter { get; set; }

        protected override async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            if (FileTypeFilter != null)
            {
                foreach (var item in FileTypeFilter)
                {
                    folderPicker.FileTypeFilter.Add(item);
                }
            }

            SelectedItem = await folderPicker.PickSingleFolderAsync();
        }
    }
}
