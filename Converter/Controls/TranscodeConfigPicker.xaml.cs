using Converter.Classes;
using Converter.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Converter.Controls
{
    public sealed partial class TranscodeConfigPicker : UserControl
    {
        public TranscodeConfigPicker()
        {
            this.InitializeComponent();

            transcodeConfigComboBox.DataContext = SupportedFormats.TranscodeConfigs();

            AudioPanel.Visibility = _selectedConfiguration?.Audio == null ?
                Visibility.Collapsed : Visibility.Visible;
            VideoPanel.Visibility = _selectedConfiguration?.Video == null ?
                Visibility.Collapsed : Visibility.Visible;
        }

        private TranscodeConfiguration _selectedConfiguration;

        public TranscodeConfiguration SelectedConfiguration
        {
            get
            {
                if (_selectedConfiguration == null)
                {
                    return null;
                }

                if (_selectedConfiguration.Audio != null)
                {
                    _selectedConfiguration.Audio.BitsPerSample = audioBitsPerSample;
                    _selectedConfiguration.Audio.SampleRate = audioSampleRate;
                    _selectedConfiguration.Audio.Bitrate = audioBitrate;
                }

                if (_selectedConfiguration.Video != null)
                {
                    _selectedConfiguration.Video.Height = videoHeight;
                    _selectedConfiguration.Video.Width = videoWidth;
                    _selectedConfiguration.Video.Bitrate = videoBitrate;
                }

                return _selectedConfiguration;
            }
        }

        private void TranscodeConfigComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedConfiguration =
                (transcodeConfigComboBox.SelectedItem as TranscodeConfigViewModel)?.
                    Configuration;
            SelectedFormatChanged?.Invoke(this, new EventArgs());
            AudioPanel.Visibility = _selectedConfiguration?.Audio == null ?
                Visibility.Collapsed : Visibility.Visible;
            VideoPanel.Visibility = _selectedConfiguration?.Video == null ?
                Visibility.Collapsed : Visibility.Visible;
        }

        public delegate void ConfigChangedEventHandler(object sender, EventArgs e);

        public event ConfigChangedEventHandler SelectedFormatChanged;

        private void UpdateValueOrWarnError(TextBox senderBox, ref uint? valueToUpdate)
        {
            if (senderBox == null)
            {
                throw new ArgumentNullException(nameof(senderBox));
            }

            uint value;
            if (!string.IsNullOrEmpty(senderBox.Text))
            {
                if (uint.TryParse(senderBox.Text, out value))
                {
                    valueToUpdate = value;
                    senderBox.Foreground = Resources["TextBoxForegroundThemeBrush"] as Brush;
                }
                else
                {
                    senderBox.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                valueToUpdate = null;
            }
        }

        private uint? audioBitsPerSample;

        private void AudioBitsPerSampleBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateValueOrWarnError(sender as TextBox, ref audioBitsPerSample);
        }

        private uint? audioSampleRate;

        private void AudioSampleRateBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateValueOrWarnError(sender as TextBox, ref audioSampleRate);
        }

        private uint? audioBitrate;

        private void AudioBitrateBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateValueOrWarnError(sender as TextBox, ref audioBitrate);
        }

        private uint? videoWidth;

        private void VideoWidthBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateValueOrWarnError(sender as TextBox, ref videoWidth);
        }

        private uint? videoHeight;

        private void VideoHeightBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateValueOrWarnError(sender as TextBox, ref videoHeight);
        }

        private uint? videoBitrate;

        private void VideoBitrateBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateValueOrWarnError(sender as TextBox, ref videoBitrate);
        }
    }
}
