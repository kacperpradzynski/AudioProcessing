using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.FourierTransform
{
    public class RectangularWindow: IWindow
    {
        public double[] WindowFactors(int m)
        {
            var result = new double[m];

            for (int i = 0; i < m; i++)
            {
                result[i] = i < m ? 1 : 0;
            }

            return result;
        }

        public double[] Windowing(double[] data)
        {
            int n = data.Length;

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = i < n ? data[i] : 0;
            }

            return data;
        }
    }
}
