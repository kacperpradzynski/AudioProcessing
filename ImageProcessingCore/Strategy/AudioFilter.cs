using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.Strategy
{
    public class AudioFilter
    {
        public IFilterStrategy strategy;
        private WavModel input;

        public AudioFilter(IFilterStrategy strategy, WavModel input)
        {
            this.strategy = strategy;
            this.input = input;
        }

        public WavModel Process()
        {
            return strategy.Process(input);
        }
    }
}
