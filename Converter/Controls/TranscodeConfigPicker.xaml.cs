using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Converter.Classes;
using Converter.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Converter.Controls
{
    public sealed partial class TranscodeConfigPicker : UserControl
    {
        public TranscodeConfigPicker()
        {
            this.InitializeComponent();

            transcodeConfigComboBox.DataContext = new List<TranscodeConfigListItem>()
            {
                new TranscodeConfigListItem(){DisplayName="ALAC",Configuration=TranscodeConfiguration.CreateAlac() },
                new TranscodeConfigListItem(){DisplayName="FLAC",Configuration=TranscodeConfiguration.CreateFlac() }
            };
        }

        public TranscodeConfiguration SelectedConfiguration
        {
            get
            {
                return (transcodeConfigComboBox.SelectedItem as TranscodeConfigListItem)?.
                    Configuration;
            }
        }

        private void TranscodeConfigComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedConfigChanged?.Invoke(this, SelectedConfiguration);
        }

        public delegate void ConfigChangedEventHandler(TranscodeConfigPicker sender, TranscodeConfiguration newConfig);

        public event ConfigChangedEventHandler SelectedConfigChanged;
    }
}
