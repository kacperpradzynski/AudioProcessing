using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.Strategy
{
    public interface IFilterStrategy
    {
        WavModel Process(WavModel input);
    }
}
