using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.FourierTransform
{
    public class HanningWindow: IWindow
    {
        public double[] WindowFactors(int m)
        {
            var result = new double[m];

            for (int i = 0; i < m; i++)
            {
                result[i] = 0.5 * (1 - Math.Cos(2 * Math.PI * i / (m - 1)));
            }

            return result;
        }

        public double[] Windowing(double[] data)
        {
            var n = data.Length;

            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= 0.5 * (1 - Math.Cos(2 * Math.PI * i / (n - 1)));
            }

            return data;
        }
    }
}
