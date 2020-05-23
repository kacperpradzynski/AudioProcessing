using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore
{
    public class Sound
    {
        public int Frequency { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int Duration { get { return EndTime - StartTime; } }
        public string Name { get; set; }
        public Sound(int startTime, int endTime, int frequency)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Frequency = frequency;
        }
    }
}
