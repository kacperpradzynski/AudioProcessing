using ImageProcessingCore.Filters;
using ImageProcessingCore.FourierTransform;
using ImageProcessingCore.Strategy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ImageProcessingView.Operations
{
    /// <summary>
    /// Logika interakcji dla klasy Equalizer.xaml
    /// </summary>
    public partial class Equalizer : UserControl, IFilter, INotifyPropertyChanged
    {
        public Equalizer()
        {
            InitializeComponent();
        }

        public IFilterStrategy GetOperationStrategy()
        {
            double[] eq = new double[] { (int)Slider1.Value,
                (int)Slider2.Value,
                (int)Slider3.Value,
                (int)Slider4.Value,
                (int)Slider5.Value,
                (int)Slider6.Value,
                (int)Slider7.Value,
                (int)Slider8.Value,
                (int)Slider9.Value,
                (int)Slider10.Value};
            return new EqualizerOperator(new HanningWindow(), 2049, 1024, 2049, eq);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
