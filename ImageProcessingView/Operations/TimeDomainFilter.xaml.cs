using ImageProcessingCore.Filters;
using ImageProcessingCore.FourierTransform;
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
    /// Logika interakcji dla klasy TimeDomainFilter.xaml
    /// </summary>
    public partial class TimeDomainFilter : UserControl, IFilter, INotifyPropertyChanged
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
        public TimeDomainFilter()
        {
            DataContext = this;
            InitializeComponent();
            Windows = new ObservableCollection<string>() { "Hamming window", "Hanning window", "Rectangular window" };
            SelectedWindow = Windows[0];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IFilterStrategy GetOperationStrategy()
        {
            IWindow window;
            switch (SelectedWindow)
            {
                case "Hamming window":
                    window = new HammingWindow();
                    break;
                case "Hanning window":
                    window = new HanningWindow();
                    break;
                case "Rectangular window":
                    window = new RectangularWindow();
                    break;
                default:
                    window = new RectangularWindow();
                    break;
            }
            int filterLength = Int32.Parse(FilterLength.Text);
            double cutFrequency = Double.Parse(CutFrequency.Text);

            return new TimeDomainFilterOperator(window, filterLength, cutFrequency);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
