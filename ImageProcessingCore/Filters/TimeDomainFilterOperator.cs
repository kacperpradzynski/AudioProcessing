using ImageProcessingCore.FourierTransform;
using ImageProcessingCore.Helpers;
using ImageProcessingCore.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.Filters
{
    public class TimeDomainFilterOperator : IFilterStrategy
    {
        private IWindow window;
        private  int filterLength;
        private  double cutFreq;

        public TimeDomainFilterOperator(IWindow window, int filterLength, double cutFreq)
        {
            this.window = window;
            this.filterLength = filterLength;
            this.cutFreq = cutFreq;
        }
        public WavModel Process(WavModel input)
        {
            var result = new double[input.Samples.Length + filterLength - 1];

            var filterFactors = FilterHelper.LowPassFilterFactors(cutFreq, input.SampleRate, filterLength);
            var filtered = window.Windowing(filterFactors);

            var data = input.Samples.ToList();
            var zeros = new double[filterLength - 1];

            data.InsertRange(0, zeros);
            data.AddRange(zeros);

            for (int i = filterLength - 1; i < data.Count; i++)
            {

                for (int j = 0; j < filtered.Length; j++)
                {
                    result[i - filterLength + 1] += data[i - j] * filtered[j];
                }
            }

            return SoundHelper.GenerateWavFromInput(input, result);
        }
    }
}
