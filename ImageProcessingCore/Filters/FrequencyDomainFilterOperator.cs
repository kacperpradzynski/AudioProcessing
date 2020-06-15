using ImageProcessingCore.FourierTransform;
using ImageProcessingCore.Helpers;
using ImageProcessingCore.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.Filters
{
    public class FrequencyDomainFilterOperator : IFilterStrategy
    {
        private IWindow window;
        private int windowLength;
        private int windowHopSize;
        private bool isCausal;
        private int filterLength;
        private double cutFreq;
        private int? n;

        public FrequencyDomainFilterOperator(IWindow window, int windowLength, int windowHopSize, bool isCausal, int filterLength, double cutFreq, int? n = null)
        {
            this.window = window;
            this.windowLength = windowLength;
            this.windowHopSize = windowHopSize;
            this.isCausal = isCausal;
            this.filterLength = filterLength;
            this.cutFreq = cutFreq;
            this.n = n;
        }
        public WavModel Process(WavModel input)
        {
            var n = this.n ?? FFT.GetExpandedPow2(windowLength + filterLength - 1);
            var size = input.Samples.Length + n - windowLength;
            var result = new double[size];

            var windows = new double[size / windowHopSize][];
            var windowsComplex = new Complex[size / windowHopSize][];

            for (int i = 0; i < windows.Length; i++)
            {
                windows[i] = new double[n];
                windowsComplex[i] = new Complex[n];
            }

            var windowFactors = window.WindowFactors(windowLength);
            for (int i = 0; i < windows.Length; i++)
            {
                for (int j = 0; j < windowLength; j++)
                {
                    if (i * windowHopSize + j < input.Samples.Length)
                    {
                        windows[i][j] = windowFactors[j] * input.Samples[i * windowHopSize + j];
                    }
                    else
                    {
                        windows[i][j] = 0;
                    }
                }
                for (int j = windowLength; j < n; j++)
                {
                    windows[i][j] = 0;
                }
            }

            var windowFilterFactors = window.WindowFactors(filterLength);
            var filterFactors = FilterHelper.LowPassFilterFactors(cutFreq, input.SampleRate, filterLength);
            var filtered = new double[n];
            for (int i = 0; i < filterLength; i++)
            {
                filtered[i] = windowFilterFactors[i] * filterFactors[i];
            }

            for (int i = filterLength; i < n; i++)
            {
                filtered[i] = 0;
            }


            if (!isCausal)
            {
                var shiftNumberFilter = (filterLength - 1) / 2;

                var shiftedFilter = filtered.Take(shiftNumberFilter);
                var filteredTemp = filtered.Skip(shiftNumberFilter).ToList();
                filteredTemp.AddRange(shiftedFilter);
                filtered = filteredTemp.ToArray();
            }

            var filteredComplex = FFT.FastTransform(filtered);

            for (int i = 0; i < windows.Length; i++)
            {
                windowsComplex[i] = FFT.FastTransform(windows[i]);
                for (int j = 0; j < windowsComplex[i].Length; j++)
                {
                    windowsComplex[i][j] *= filteredComplex[j];
                }
                windows[i] = FFT.IFastTransform(windowsComplex[i]);
            }

            for (int i = 0; i < windows.Length; i++)
            {
                for (int j = 0; j < windows[i].Length; j++)
                {
                    if (i * windowHopSize + j < input.Samples.Length)
                    {
                        result[i * windowHopSize + j] += windows[i][j];
                    }
                }
            }

            return SoundHelper.GenerateWavFromInput(input, result);
        }
    }
}
