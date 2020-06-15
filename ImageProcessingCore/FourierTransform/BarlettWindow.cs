using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.FourierTransform
{
    public class BarlettWindow: IWindow
    {
        public double[] WindowFactors(int m)
        {
            var result = new double[m];

            for (int i = 0; i < m; i++)
            {
                result[i] = 2 / (m - 1) * ((m - 1) / 2 - Math.Abs(i - (m - 1) / 2));
            }

            return result;
        }

        public double[] Windowing(double[] data)
        {
            var n = data.Length;

            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= 2 / (n - 1) * ((n - 1) / 2 - Math.Abs(i - (n - 1) / 2));
            }

            return data;
        }
    }
}
