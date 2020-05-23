using ImageProcessingCore.Helpers;
using ImageProcessingCore.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.FrequencyFinders
{
    public class AutocorrelationOperator : IProcessingStrategy
    {
        public List<Sound> Process(WavModel input)
        {
            int currentTimeStart = 0, currentTimeEnd;
            List<Sound> output = new List<Sound>();
            foreach(var chunk in input.ChunkedSamples)
            {
                var localMax = (Index: 0, Value: double.MinValue);
                var lastValue = double.MaxValue;
                var monotonicityChanged = 0;
                var isFalling = true;
                var found = false;
                var length = chunk.Length;
                double[] result = new double[length];

                for (int i = 1; i < length; i++)
                {
                    double sum = 0;

                    for (int j = 0; j < length - i; j++)
                    {
                        sum += chunk[j] * chunk[i + j];
                    }

                    result[i - 1] = sum;

                    if(!found && isFalling != lastValue>sum)
                    {
                        isFalling = lastValue > sum;
                        monotonicityChanged++;

                        if(monotonicityChanged == 2)
                        {
                            localMax = (i - 1, lastValue);
                            found = true;
                        }
                    }

                    lastValue = sum;
                }

                if(localMax.Index == 0)
                {
                    localMax.Index = 1;
                }

                currentTimeEnd = currentTimeStart + (chunk.Length * 1000 / input.SampleRate);
                output.Add(new Sound(currentTimeStart, currentTimeEnd, input.SampleRate / localMax.Index));
                currentTimeStart = currentTimeEnd;
            }

            return SoundHelper.CombineFrequencies(output);
        }
    }
}
