using ImageProcessingCore.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore
{
    public static class WavReader
    {
        public static WavModel ReadWav(string file)
        {
            WavModel output = new WavModel();
            using (var fs = File.Open(file, FileMode.Open))
            using (var reader = new BinaryReader(fs)) 
            {
                output.ChunkId = reader.ReadInt32();
                output.ChunkSize = reader.ReadInt32();
                output.Format = reader.ReadInt32();

                output.Subchunk1Id = reader.ReadInt32();
                output.Subchunk1Size = reader.ReadInt32();
                output.AudioFormat = reader.ReadInt16();
                output.NumChannels = reader.ReadInt16();
                output.SampleRate = reader.ReadInt32();
                output.ByteRate = reader.ReadInt32();
                output.BlockAlign = reader.ReadInt16();
                output.BitsPerSample = reader.ReadInt16();

                if (output.ChunkSize == 18)
                {
                    var extraDataSize = reader.ReadInt16();
                    output.ExtraData = reader.ReadBytes(extraDataSize);
                }

                output.Subchunk2Id = reader.ReadInt32();
                output.Subchunk2Size = reader.ReadInt32();
                output.RawData = reader.ReadBytes(output.Subchunk2Size);

                var samplesSize = output.Subchunk2Size / (output.BitsPerSample / 8);
                var samples = new double[samplesSize];

                switch (output.BitsPerSample)
                {
                    case 64:
                        var samples64 = new long[samplesSize];
                        Buffer.BlockCopy(output.RawData, 0, samples64, 0, output.Subchunk2Size);
                        samples = Array.ConvertAll(samples64, e => (double)e);
                        break;
                    case 32:
                        var samples32 = new int[samplesSize];
                        Buffer.BlockCopy(output.RawData, 0, samples32, 0, output.Subchunk2Size);
                        samples = Array.ConvertAll(samples32, e => (double)e);
                        break;
                    case 16:
                        var samples16 = new short[samplesSize];
                        Buffer.BlockCopy(output.RawData, 0, samples16, 0, output.Subchunk2Size);
                        samples = Array.ConvertAll(samples16, e => (double)e);
                        break;
                }

                var numberOfChunks = samplesSize / SoundHelper.GetWindowSize(output.SampleRate);
                var chunkedSamples = new double[numberOfChunks][];

                for(int i = 0; i < numberOfChunks; i++)
                {
                    chunkedSamples[i] = new double[SoundHelper.GetWindowSize(output.SampleRate)];
                    for(int j = 0; j < SoundHelper.GetWindowSize(output.SampleRate); j++)
                    {
                        chunkedSamples[i][j] = samples[SoundHelper.GetWindowSize(output.SampleRate) * i + j];
                    }
                }
                output.ChunkedSamples = chunkedSamples;
            }

            return output;
        }


        public static void SaveWav(WavModel output, string path)
        {
            using (var fs = File.Open(path, FileMode.Create))
            using (var writer = new BinaryWriter(fs))
            {
                writer.Write(output.ChunkId);
                writer.Write(output.ChunkSize);
                writer.Write(output.Format);

                writer.Write(output.Subchunk1Id);
                writer.Write(output.Subchunk1Size);
                writer.Write(output.AudioFormat);
                writer.Write(output.NumChannels);
                writer.Write(output.SampleRate);
                writer.Write(output.ByteRate);
                writer.Write(output.BlockAlign);
                writer.Write(output.BitsPerSample);

                if (output.ChunkSize == 18)
                {
                    writer.Write(output.ExtraData);
                }

                writer.Write(output.Subchunk2Id);
                writer.Write(output.Subchunk2Size);
                writer.Write(output.RawData);
            }
        }
    }
}
