using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace Converter.Controls
{
    public sealed class FileSaveControl : FileBrowseControl
    {
        public IEnumerable<KeyValuePair<string, IList<string>>> SaveFileChoices { get; set; }

        public string SuggestedFileName { get; set; }

        protected override async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            if (SaveFileChoices != null)
            {
                foreach (var item in SaveFileChoices)
                {
                    savePicker.FileTypeChoices.Add(item);
                }
            }

            savePicker.SuggestedFileName = SuggestedFileName ?? string.Empty;

            SelectedItem = await savePicker.PickSaveFileAsync();
        }
    }
}
