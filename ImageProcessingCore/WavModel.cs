using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore
{
    public class WavModel
    {
        public int ChunkId { get; internal set; }
        public int ChunkSize { get; internal set; }
        public int Format { get; internal set; }
        public int Subchunk1Id { get; internal set; }
        public int Subchunk1Size { get; internal set; }
        public short AudioFormat { get; internal set; }
        public short NumChannels { get; internal set; }
        public int SampleRate { get; internal set; }
        public int ByteRate { get; internal set; }
        public short BlockAlign { get; internal set; }
        public short BitsPerSample { get; internal set; }
        public int Subchunk2Id { get; internal set; }
        public int Subchunk2Size { get; internal set; }
        public byte[] RawData { get; internal set; }
        public double[][] ChunkedSamples { get; internal set; }
        public byte[] ExtraData { get; internal set; }
        public double[] Samples { get; internal set; }
    }
}
