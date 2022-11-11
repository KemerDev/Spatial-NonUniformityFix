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
    public class CalcStdDiv
    {
        private int rows = 4;
        private int cols = 3;
        private double[,] stdMean;

        public double CalcDev(Bitmap bmp)
        {
            var splitImg = SplitImageToParts(bmp);

            double[,] stdValues = new double[rows, cols];
            stdMean = new double[rows, cols];

            for (int i = 0; i < splitImg.GetLength(0); i++)
            {
                for (int j = 0; j < splitImg.GetLength(1); j++)
                {
                    MCvScalar mean = new MCvScalar();
                    MCvScalar stdv = new MCvScalar();

                    CvInvoke.MeanStdDev(splitImg[i, j].ToImage<Gray, byte>(), ref mean, ref stdv);

                    stdValues[i, j] = stdv.V0;
                    stdMean[i, j] = mean.V0;
                }
            }

            return CalcFinalDev(stdValues);
        }

        private double CalcFinalDev(double[,] stdValues)
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

        private Bitmap[,] SplitImageToParts(Bitmap bmp)
        {
            var partsArray = new Bitmap[rows, cols];

            int width = bmp.Width;
            int height = bmp.Height;

            int cropImgWidth = width / cols;
            int cropImgHeight = height / rows;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    partsArray[i, j] = new Bitmap(cropImgWidth, cropImgHeight);
                    var graphics = Graphics.FromImage(partsArray[i, j]);
                    graphics.DrawImage(bmp, new Rectangle(0, 0, cropImgWidth, cropImgHeight), new Rectangle(j * cropImgWidth, i * cropImgHeight, cropImgWidth, cropImgHeight), GraphicsUnit.Pixel);
                    graphics.Dispose();
                }
            }
            return partsArray;
        }
    }
}
