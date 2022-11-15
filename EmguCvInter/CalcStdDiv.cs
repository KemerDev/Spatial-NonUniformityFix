using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguCvInter
{
    public class CalcStdDiv : IDisposable
    {
        public bool Disposed;

        public unsafe Bitmap createNewPartImg(Bitmap[,] splitImg, double[,] stdMean, int width, int height, int cropImgWidth, int cropImgHeight)
        {
            for (int i = 0; i < splitImg.GetLength(0); i++)
            {
                for (int j = 0; j < splitImg.GetLength(1); j++)
                {
                    BitmapData splitImgData = splitImg[i, j].LockBits(new Rectangle(0, 0, cropImgWidth, cropImgHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                    byte* splitImgPointer = (byte*)splitImgData.Scan0;

                    for (int k = 0; k < cropImgHeight; k++)
                    {
                        for (int l = 0; l < cropImgWidth; l++)
                        {
                            splitImgPointer[0] = (byte)stdMean[i, j];
                            splitImgPointer[1] = (byte)stdMean[i, j];
                            splitImgPointer[2] = (byte)stdMean[i, j];

                            splitImgPointer += 4;
                        }
                        splitImgPointer += splitImgData.Stride - (splitImgData.Width * 4);
                    }
                    splitImg[i, j].UnlockBits(splitImgData);
                }
            }

            Bitmap bmp = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < splitImg.GetLength(0); i++)
                {
                    for (int j = 0; j < splitImg.GetLength(1); j++)
                    {
                        graphics.DrawImage(splitImg[i, j], cropImgWidth * j, cropImgHeight * i);
                    }
                }
            }

            return bmp;
        }

        public double CalcFinalDev(double[,] stdValues)
        {
            double tempMean = 0;
            double tempVar = 0;
            double tempDev = 0;
            double[] tempDivArray = new double[12];

            var tempStdVals = stdValues.Cast<double>().Select(x => x).ToArray();    

            for (int i = 0; i < tempStdVals.Length; i++)
            {
                tempMean += tempStdVals[i];

            }

            tempMean = tempMean / tempStdVals.Length;

            for (int i = 0; i < tempStdVals.Length; i++)
            {
                tempVar += Math.Pow(tempStdVals[i] - tempMean, 2);
            }

            tempDev = tempVar / tempStdVals.Length;

            return Math.Sqrt(tempDev);
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
        }
    }
}
