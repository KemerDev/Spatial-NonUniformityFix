using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguCvInter
{
    public class CalcStdDiv
    {
        public double[] CalcPartMean(Bitmap bmp)
        {
            var matrixL = bmp.Width / 12;

            float[,] nivImageData = new float[bmp.Width,bmp.Height];

            unsafe
            {
                BitmapData bitmapDataNiv = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                byte* NivalatedPointer = (byte*)bitmapDataNiv.Scan0;

                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        nivImageData[j, i] = NivalatedPointer[0];

                        NivalatedPointer += 4;
                    }
                    NivalatedPointer += bitmapDataNiv.Stride - (bitmapDataNiv.Width * 4);
                }
                bmp.UnlockBits(bitmapDataNiv);
            }

            float[] nivImageData1D = nivImageData.Cast<float>().Select(x => x).ToArray();
            float[] meanValues = new float[12];

            float tempValue = 0;

            // mean value
            Parallel.For(0, 12, k =>
            {
                for (int i = matrixL * k; i < matrixL * (k + 1); i++)
                {
                    tempValue += nivImageData1D[i];
                }
                meanValues[k] = tempValue / matrixL;
                tempValue = 0;
            });

            return CalcPartDev(matrixL, nivImageData1D, meanValues);
        }

        private double[] CalcPartDev(int matrixL, float[] nivImageData1D, float[] meanValues)
        {
            double tempValue = 0;
            double[] tempDev = new double[12];

            Parallel.For(0, 12, k =>
            {
                for (int i = matrixL * k; i < matrixL * (k + 1); i++)
                {
                    tempValue += Math.Pow(nivImageData1D[i] - meanValues[k], 2);
                }

                tempDev[k] = Math.Sqrt(tempValue / (matrixL - 1));
            });

            return tempDev;
        }
    }
}
