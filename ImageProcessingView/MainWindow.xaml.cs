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
using LiveCharts;
using LiveCharts.Wpf;
using ImageProcessingCore.Strategy;
using ImageProcessingView.Operations;
using ImageProcessingCore.Helpers;
using ImageProcessingCore;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace ImageProcessingView
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<string> _operations;
        private string _selectedOperation, _inputFileName;
        private UserControl currentOperation;
        public WavModel input, output;
        private bool unlocked;
        private long calculationTime;
        public AudioProcessor processor;
        public AudioFilter filter;
        bool isFilter = false;

        public long CalculationTime
        {
            get { return calculationTime; }
            set
            {
                if (calculationTime != value)
                {
                    calculationTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> Operations
        {
            get { return _operations; }
            set
            {
                if (_operations != value)
                {
                    _operations = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SelectedOperation
        {
            get { return _selectedOperation; }
            set
            {
                if (_selectedOperation != value)
                {
                    _selectedOperation = value;
                    OnPropertyChanged();
                    currentOperation = GetActiveUserControl(_selectedOperation);
                    ChangeActiveOperation();
                }
            }
        }

        public string InputFileName
        {
            get { return _inputFileName; }
            set
            {
                if (_inputFileName != value)
                {
                    _inputFileName = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            Operations = new ObservableCollection<string>() { "Autocorrelation frequency finder", "Fourier density frequency finder", "Time domain filter", "Frequency domain filter", "Equalizer" };
            SelectedOperation = Operations[0];
            this.unlocked = true;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        private void ChangeActiveOperation()
        {
            Brightness.Visibility = Visibility.Collapsed;
            Contrast.Visibility = Visibility.Collapsed;
            TimeDomainFilterUserControl.Visibility = Visibility.Collapsed;
            FrequencyDomainFilterUserControl.Visibility = Visibility.Collapsed;
            EqualizerUserControl.Visibility = Visibility.Collapsed;

            currentOperation.Visibility = Visibility.Visible;
        }

        private UserControl GetActiveUserControl(string operation)
        {
            switch (operation)
            {
                case "Autocorrelation frequency finder":
                    ChangeView(Brightness);
                    return Brightness;
                case "Fourier density frequency finder":
                    ChangeView(Contrast);
                    return Contrast;
                case "Time domain filter":
                    ChangeView(TimeDomainFilterUserControl);
                    return TimeDomainFilterUserControl;
                case "Frequency domain filter":
                    ChangeView(FrequencyDomainFilterUserControl);
                    return FrequencyDomainFilterUserControl;
                case "Equalizer":
                    ChangeView(EqualizerUserControl);
                    return EqualizerUserControl;
                default:
                    ChangeView(Brightness);
                    return Brightness;
            }
        }

        private void ChangeView(UserControl current)
        {
            SegmentationUserControl.Visibility = Visibility.Collapsed;
            FilterUserControl.Visibility = Visibility.Collapsed;

            if(current == Brightness || current == Contrast)
            {
                SaveMenuButton.Visibility = Visibility.Collapsed;
                isFilter = false;
                SegmentationUserControl.Visibility = Visibility.Visible;
            } else
            {
                SaveMenuButton.Visibility = Visibility.Visible;
                isFilter = true;
                FilterUserControl.Visibility = Visibility.Visible;
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Audio Files (*.wav)|*.WAV",
                RestoreDirectory = true
            };
            var result = ofd.ShowDialog();
            if (result == true)
            {
                input = WavReader.ReadWav(ofd.FileName);
                SegmentationUserControl.input = input;
                FilterUserControl.Input = input;
                SegmentationUserControl.SampleRate = input.SampleRate;
                ApplyButton.IsEnabled = true;
                InputFileName =System.IO.Path.GetFileName(ofd.FileName);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (input != null)
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    FileName = "Filtered sound",
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

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isFilter)
                {
                    processor = new AudioProcessor(((IProcessing)currentOperation).GetOperationStrategy(), input);
                    if (input != null && unlocked)
                    {
                        Task.Run(() => Process());
                    }
                } else
                {
                    filter = new AudioFilter(((IFilter)currentOperation).GetOperationStrategy(), input);
                    if (input != null && unlocked)
                    {
                        Task.Run(() => ProcessFilter());
                    }
                }
               
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message, currentOperation.Name, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        internal void Process()
        {
            var timer = new Stopwatch();
            timer.Start();
            unlocked = false;
            List<Sound> listSound = processor.Process();
            ObservableCollection<Sound> list = new ObservableCollection<Sound>();
            foreach (var item in listSound)
                list.Add(item);
            timer.Stop();
            CalculationTime = timer.ElapsedMilliseconds;
            this.Dispatcher.Invoke(() => {
                SegmentationUserControl.Sound = list;
                SegmentationUserControl.soundList = listSound;
            });
            unlocked = true;
        }

        internal void ProcessFilter()
        {
            var timer = new Stopwatch();
            timer.Start();
            unlocked = false;
            output = filter.Process();
            timer.Stop();
            CalculationTime = timer.ElapsedMilliseconds;
            this.Dispatcher.Invoke(() => {
                FilterUserControl.Output = output;
                SaveMenuButton.IsEnabled = true;
            });
            unlocked = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
