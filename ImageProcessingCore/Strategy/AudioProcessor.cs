using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.Strategy
{
    public class AudioProcessor
    {
        public IProcessingStrategy strategy;
        private WavModel input;

        public AudioProcessor(IProcessingStrategy strategy, WavModel input)
        {
            this.strategy = strategy;
            this.input = input;
        }

        public List<Sound> Process()
        {
            return strategy.Process(input);
        }

    }
}
