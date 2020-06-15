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
    public class EqualizerOperator: IFilterStrategy
    {
        private IWindow window;
        private int windowLength;
        private int windowHopSize;
        private int filterLength;
        private double[] gains;

        public EqualizerOperator(IWindow window, int windowLength, int windowHopSize, int filterLength, double[] gains)
        {
            this.window = window;
            this.windowLength = windowLength;
            this.windowHopSize = windowHopSize;
            this.filterLength = filterLength;
            this.gains = gains;
        }
        public WavModel Process(WavModel input)
        {
            var n = FFT.GetExpandedPow2(windowLength + filterLength - 1);
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
            var gainsComplex = GenerateGains(gains, input.SampleRate, n);
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

                windowsComplex[i] = FFT.FastTransform(windows[i]);
                windowsComplex[i] = AdjustGain(gainsComplex, windowsComplex[i]);
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

        private Complex[] AdjustGain(Complex[][] gainsComplex, Complex[] data)
        {
            var n = data.Length;

            var equalized = new Complex[n];

            for (int j = 0; j < data.Length; j++)
            {
                equalized[j] = 0;
            }

            for (int i = 0; i < gainsComplex.Length; i++)
            {
                AddGain(data, gainsComplex[i], equalized);
            }

            return equalized;
        }

        private void AddGain(Complex[] data, Complex[] gainsComplex, Complex[] equalized)
        {
            for (int j = 0; j < data.Length; j++)
            {
                equalized[j] += data[j] * gainsComplex[j];
            }
        }

        private Complex[][] GenerateGains(double[] gains, int sampleRate, int n)
        {
            var gainsComplex = new Complex[10][];
            var low = 40;
            var high = 20;

            for (int i = 0; i < 10; i++)
            {
                if (gains[i] == 0)
                {
                    gains[i] += 1;
                }
                else if (gains[i] < 0)
                {
                    gains[i] = 1.0 / Math.Abs(gains[i]);
                }

                var bandFilterFactors = FilterHelper.BandPassFilterFactors(low, high, sampleRate, filterLength, n);

                for (int j = 0; j < bandFilterFactors.Length; j++)
                {
                    bandFilterFactors[j] *= gains[i];
                }

                gainsComplex[i] = bandFilterFactors;

                low *= 2;
                high *= 2;
            }

            return gainsComplex;
        }
    }
}
