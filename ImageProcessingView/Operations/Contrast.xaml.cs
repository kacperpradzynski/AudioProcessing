using ImageProcessingCore.FrequencyFinders;
using ImageProcessingCore.Strategy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

namespace ImageProcessingView.Operations
{
    /// <summary>
    /// Logika interakcji dla klasy Contrast.xaml
    /// </summary>
    public partial class Contrast : UserControl, IProcessing, INotifyPropertyChanged
    {
        private ObservableCollection<string> _windows;
        private string _selectedWindow;
        public ObservableCollection<string> Windows
        {
            get { return _windows; }
            set
            {
                if (_windows != value)
                {
                    _windows = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SelectedWindow
        {
            get { return _selectedWindow; }
            set
            {
                if (_selectedWindow != value)
                {
                    _selectedWindow = value;
                    OnPropertyChanged();
                }
            }
        }

        public Contrast()
        {
            DataContext = this;
            InitializeComponent();
            Windows = new ObservableCollection<string>() { "Hamming window", "Hanning window", "Blackman window" };
            SelectedWindow = Windows[0];
        }

        public IProcessingStrategy GetOperationStrategy()
        {
            int window = 0;
            switch (SelectedWindow)
            {
                case "Hamming window":
                    window = 1;
                    break;
                case "Hanning window":
                    window = 2;
                    break;
                case "Blackman window":
                    window = 3;
                    break;
            }
            return new CepstralOperator(window);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
