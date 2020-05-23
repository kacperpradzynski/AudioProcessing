using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.Helpers
{
    public static class SoundHelper
    {
        public static List<Sound> CombineFrequencies(List<Sound> output)
        {
            List<Sound> combined = new List<Sound>();
            combined.Add(output[0]);
            int lastElement = 0;
            for (int i = 1; i < output.Count; i++)
            {
                if (combined[lastElement].Frequency == output[i].Frequency)
                {
                    combined[lastElement].EndTime += output[i].Duration;
                }
                else
                {
                    lastElement++;
                    combined.Add(output[i]);
                }
            }
            return combined;
        }

        public static int GetWindowSize(int sampleRate)
        {
            return sampleRate / 20;
        }

        public static double[][] GenerateSound(List<Sound> frequencies, int sampleRate, short bitsPerSample)
        {
            var splittedFreq = new List<long>();
            var scale = bitsPerSample == 64 ? long.MaxValue : bitsPerSample == 32 ? int.MaxValue : short.MaxValue;
        
            foreach(var freq in frequencies)
            {
                for(int i = 0; i < freq.EndTime / 50 - freq.StartTime / 50; i++)
                {
                    splittedFreq.Add(freq.Frequency);
                }
            }

            var result = new double[splittedFreq.Count][];
            for(int i = 0; i < splittedFreq.Count; i++)
            {
                result[i] = new double[GetWindowSize(sampleRate)];
                var freq = (double)sampleRate / splittedFreq[i];

                for(int j = 0; j < GetWindowSize(sampleRate); j++)
                {
                    result[i][j] = scale * Math.Sin(2 * Math.PI * (i * GetWindowSize(sampleRate) + j) / freq) / 10;
                }
            }

            return result;
        }

        public static WavModel GenerateWavFromInput(WavModel input, double[][] newChunkedSamples)
        {
            var result = new WavModel();
            var samplesSize = input.Subchunk2Size / (input.BitsPerSample / 8);
            var samples = new double[samplesSize];
            var numberOfChunks = samplesSize / SoundHelper.GetWindowSize(input.SampleRate);

            for (int i = 0; i < numberOfChunks; i++)
            {
                for (int j = 0; j < SoundHelper.GetWindowSize(input.SampleRate); j++)
                {
                    samples[SoundHelper.GetWindowSize(input.SampleRate) * i + j] = newChunkedSamples[i][j];
                }
            }
            result.RawData = new byte[input.RawData.Length];
            switch (input.BitsPerSample)
            {
                case 64:
                    var samples64 = new long[samplesSize];
                    samples64 = Array.ConvertAll(samples, e => (long)e);
                    Buffer.BlockCopy(samples64, 0, result.RawData, 0, input.Subchunk2Size);
                    break;
                case 32:
                    var samples32 = new int[samplesSize];
                    samples32 = Array.ConvertAll(samples, e => (int)e);
                    Buffer.BlockCopy(samples32, 0, result.RawData, 0, input.Subchunk2Size);
                    break;
                case 16:
                    var samples16 = new short[samplesSize];
                    samples16 = Array.ConvertAll(samples, e => (short)e);
                    Buffer.BlockCopy(samples16, 0, result.RawData, 0, input.Subchunk2Size);
                    break;
            }
            result.ChunkId = input.ChunkId;
            result.ChunkSize = input.ChunkSize;
            result.Format = input.Format;

            result.Subchunk1Id = input.Subchunk1Id;
            result.Subchunk1Size = input.Subchunk1Size;
            result.AudioFormat = input.AudioFormat;
            result.NumChannels = input.NumChannels;
            result.SampleRate = input.SampleRate;
            result.ByteRate = input.ByteRate;
            result.BlockAlign = input.BlockAlign;
            result.BitsPerSample = input.BitsPerSample;
            result.ExtraData = input.ExtraData;


            result.Subchunk2Id = input.Subchunk2Id;
            result.Subchunk2Size = input.Subchunk2Size;

            result.ChunkedSamples = newChunkedSamples;

            return result;
        }
    }
}
