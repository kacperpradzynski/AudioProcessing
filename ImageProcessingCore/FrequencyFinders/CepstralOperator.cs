using ImageProcessingCore.Helpers;
using ImageProcessingCore.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using ImageProcessingCore.FourierTransform;

namespace ImageProcessingCore.FrequencyFinders
{
    public class CepstralOperator : IProcessingStrategy
    {
        private int selectedWindow;
        public CepstralOperator(int selectedWindow)
        {
            this.selectedWindow = selectedWindow;
        }

        public List<Sound> Process(WavModel input)
        {
            int currentTimeStart = 0, currentTimeEnd;
            int count = (int)Math.Pow(2, (int)(Math.Floor(Math.Log(input.ChunkedSamples[0].Length, 2))));
            List<Sound> output = new List<Sound>();
            
            foreach (var chunk in input.ChunkedSamples)
            {
                double[] coefficientsDouble = new double[count/2];
                Complex[] coefficientsComplex = new Complex[count];
                double freq = 0;

                double[] signal = ApplyWindow(chunk);
                coefficientsComplex[0] = signal[0];
                for (int i = 1; i < count; i++)
                {
                    coefficientsComplex[i] = signal[i] - 0.95 * signal[i-1];
                }
                FFT.Transform(coefficientsComplex);
                for (int i = 0; i < count/2; i++)
                {
                    coefficientsDouble[i] = Math.Log(coefficientsComplex[i].Magnitude);
                }
                double max = coefficientsDouble.Max();
                for (int i = 0; i < count / 2; i++)
                {
                    if(coefficientsDouble[i] < 0.9*max)
                        coefficientsDouble[i] = 0;
                }
                for (int i = 2; i < count / 2; i++)
                {
                    if (coefficientsDouble[i - 2] < coefficientsDouble[i - 1] && coefficientsDouble[i - 1] > coefficientsDouble[i])
                    {
                        freq = input.SampleRate / count * (i - 1);
                        break;
                    }
                        
                }

                currentTimeEnd = currentTimeStart + (chunk.Length * 1000 / input.SampleRate);
                output.Add(new Sound(currentTimeStart, currentTimeEnd, (int)freq));
                currentTimeStart = currentTimeEnd;
            }

            return SoundHelper.CombineFrequencies(output);
        }

        private double[] ApplyWindow(double[] signal)
        {
            double[] window = new double[signal.Length];
            double[] output = new double[signal.Length];
            switch (selectedWindow)
            {

                case 1:
                    //Hamming window
                    for (int i = 0; i < window.Length; i++)
                    {
                        window[i] = 0.53836 - (0.46164 * Math.Cos((2 * Math.PI * i) / window.Length));
                    }
                    break;
                case 2:
                    //Hanning window
                    for (int i = 0; i < window.Length; i++)
                    {
                        window[i] = 0.5 - (0.5 * Math.Cos((2 * Math.PI * i) / window.Length));
                    }
                    break;
                case 3:
                    //Blackman window
                    for (int i = 0; i < window.Length; i++)
                    {
                        window[i] = 0.42 - (0.5 * Math.Cos((2 * Math.PI * i) / window.Length)) + (0.08 * Math.Cos((4 * Math.PI * i) / window.Length));
                    }
                    break;
            }
            for (int i = 0; i < signal.Length; i++)
            {
                output[i] = signal[i] * window[i];
            }
            return output;
        }
    }
}
