using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.FourierTransform
{
    public interface IWindow
    {
        double[] Windowing(double[] data);
        double[] WindowFactors(int m);
    }
}
