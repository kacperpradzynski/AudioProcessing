using ImageProcessingCore;
using ImageProcessingCore.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageProcessingView
{
    /// <summary>
    /// Logika interakcji dla klasy SegmentationView.xaml
    /// </summary>
    public partial class SegmentationView : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<Sound> _sound;
        public List<Sound> soundList;
        private int _sampleRate;
        public WavModel input, output;

        public ObservableCollection<Sound> Sound
        {
            get { return _sound; }
            set
            {
                if (_sound != value)
                {
                    _sound = value;
                    SetHeaders(_sound);
                    OnPropertyChanged();
                    if(_sound.Count > 0)
                    {
                        GenerateButton.IsEnabled = true;
                    } else
                    {
                        GenerateButton.IsEnabled = false;
                    }
                }
            }
        }

        public int SampleRate
        {
            get { return _sampleRate; }
            set
            {
                if (_sampleRate != value)
                {
                    _sampleRate = value;
                    OnPropertyChanged();
                }
            }
        }

        private void SetHeaders(ObservableCollection<Sound> sound)
        {
            int i = 1;
            foreach (var s in sound)
            {
                s.Name = "Frequency " + i++;
            }
        }

        public SegmentationView()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            if(input != null)
            {
                output = SoundHelper.GenerateWavFromInput(input, SoundHelper.GenerateSound(soundList, input.SampleRate, input.BitsPerSample));
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    FileName = "Generated tone",
                    Filter = "Audio Files (*.wav)|*.WAV",
                    RestoreDirectory = true
                };
                var result = sfd.ShowDialog();
                if (result == true)
                {
                    WavReader.SaveWav(output, sfd.FileName);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
