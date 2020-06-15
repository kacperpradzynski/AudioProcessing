using ImageProcessingCore.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingCore.FourierTransform
{
    public static class FFT
    {
        public static Complex[] FastTransform(double[] data)
        {
            var complexData = data.Select(c => (Complex)c).ToArray();

            Transform(complexData);

            return complexData;
        }

        public static double[] IFastTransform(Complex[] complexData)
        {
            Conjugate(complexData);
            Transform(complexData);
            Conjugate(complexData);

            return complexData.Select(c => (c / complexData.Length).Real).ToArray();
        }

        public static void Transform(Complex[] buffer)
        {
            int bits = (int)Math.Log(buffer.Length, 2);

            for (int j = 1; j < buffer.Length; j++)
            {
                int swapPos = BitReverse(j, bits);
                if (swapPos <= j)
                {
                    continue;
                }
                var temp = buffer[j];
                buffer[j] = buffer[swapPos];
                buffer[swapPos] = temp;
            }

            for (int N = 2; N <= buffer.Length; N <<= 1)
            {
                for (int i = 0; i < buffer.Length; i += N)
                {
                    for (int k = 0; k < N / 2; k++)
                    {

                        int evenIndex = i + k;
                        int oddIndex = i + k + (N / 2);
                        var even = buffer[evenIndex];
                        var odd = buffer[oddIndex];

                        double term = -2 * Math.PI * k / (double)N;
                        Complex exp = new Complex(Math.Cos(term), Math.Sin(term)) * odd;

                        buffer[evenIndex] = even + exp;
                        buffer[oddIndex] = even - exp;

                    }
                }
            }
        }

        private static int BitReverse(int n, int bits)
        {
            int reversedN = n;
            int count = bits - 1;

            n >>= 1;
            while (n > 0)
            {
                reversedN = (reversedN << 1) | (n & 1);
                count--;
                n >>= 1;
            }

            return ((reversedN << count) & ((1 << bits) - 1));
        }

        public static void Conjugate(Complex[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Complex.Conjugate(data[i]);
            }
        }

        public static double[] ReduceToPow2(double[] data)
        {
            var length = (int)Math.Log(data.Length,2);
            return data.Take((int)Math.Pow(2, length)).ToArray();
        }

        public static int GetExpandedPow2(int length)
        {
            return (int)Math.Pow(2, (int)Math.Log(length, 2) + 1);
        }
    }
}
