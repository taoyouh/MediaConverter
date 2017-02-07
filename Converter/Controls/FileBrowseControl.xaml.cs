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

namespace Converter.Controls
{
    public abstract partial class FileBrowseControl : UserControl
    {
        public FileBrowseControl()
        {
            this.InitializeComponent();
        }

        private IStorageItem _selectedItem;

        public IStorageItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                if (SelectedItem != value)
                {
                    _selectedItem = value;
                    urlBox.Text = _selectedItem?.Path ?? string.Empty;
                    SelectionChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public event EventHandler<EventArgs> SelectionChanged;

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

        protected abstract void BrowseButton_Click(object sender, RoutedEventArgs e);
    }
}
