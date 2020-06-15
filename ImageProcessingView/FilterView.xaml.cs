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
using ImageProcessingCore;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace ImageProcessingView
{
    /// <summary>
    /// Logika interakcji dla klasy FilterView.xaml
    /// </summary>
    public partial class FilterView : UserControl, INotifyPropertyChanged
    {
        private OxyPlot.PlotModel inputModel, outputModel;
        public WavModel input, output;
        public OxyPlot.PlotModel InputModel
        {
            get
            {
                return inputModel;
            }
            set
            {
                inputModel = value;
                OnPropertyChanged("InputModel");
            }
        }
        public OxyPlot.PlotModel OutputModel
        {
            get
            {
                return outputModel;
            }
            set
            {
                outputModel = value;
                OnPropertyChanged("OutputModel");
            }
        }
        public WavModel Input
        {
            get { return input; }
            set
            {
                if (input != value)
                {
                    input = value;
                    InputModel = DrawChart(input);
                    OnPropertyChanged();
                }
            }
        }

        public WavModel Output
        {
            get { return output; }
            set
            {
                if (output != value)
                {
                    output = value;
                    OutputModel = DrawChart(output);
                    OnPropertyChanged();
                }
            }
        }

        public FilterView()
        {
            DataContext = this;
            InitializeComponent();
        }

        private PlotModel DrawChart(WavModel input)
        {
            int index = 0;
            PlotModel toReturn = new PlotModel();
            toReturn.DefaultColors = new List<OxyColor> { OxyColor.FromRgb(63, 81, 181) };
            var lineSerie = new OxyPlot.Series.LineSeries
            {
                CanTrackerInterpolatePoints = false,
                Smooth = false,
            };

            input.Samples.ToList().ForEach(d => lineSerie.Points.Add(new DataPoint(index++, d)));
            toReturn.Series.Add(lineSerie);
            toReturn.IsLegendVisible = false;
            return toReturn;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
