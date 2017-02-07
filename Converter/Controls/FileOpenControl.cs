using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace Converter.Controls
{
    public sealed class FileOpenControl : FileBrowseControl
    {
        public IEnumerable<string> OpenFileFilters { get; set; }

        protected override async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            if (OpenFileFilters != null)
            {
                foreach (var item in OpenFileFilters)
                {
                    openPicker.FileTypeFilter.Add(item);
                }
            }

            SelectedItem = await openPicker.PickSingleFileAsync();
        }
    }
}
