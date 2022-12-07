using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace EmguCvInter
{
    public class CalculationsClass : IDisposable
    {
        private bool Disposed;

        public List<double[,]> CalcPartMeanStdv(Bitmap[,] bmp, int rows, int cols)
        {

            double[,] stdValues = new double[rows, cols];
            double[,] stdMean = new double[rows, cols];

            List<double[,]> meanStdvList = new List<double[,]>();

            for (int i = 0; i < bmp.GetLength(0); i++)
            {
                for (int j = 0; j < bmp.GetLength(1); j++)
                {
                    MCvScalar mean = new MCvScalar();
                    MCvScalar stdv = new MCvScalar();

                    CvInvoke.MeanStdDev(bmp[i, j].ToImage<Gray, byte>(), ref mean, ref stdv);

                    stdValues[i, j] = stdv.V0;
                    stdMean[i, j] = mean.V0;
                }
            }

            meanStdvList.Add(stdValues);
            meanStdvList.Add(stdMean);

            return meanStdvList;
        }

        public Bitmap[,] SplitImageToParts(Bitmap bmp, int rows, int cols)
        {
            var partsArray = new Bitmap[rows, cols];

            var width = bmp.Width;
            var height = bmp.Height;

            var cropImgWidth = width / cols;
            var cropImgHeight = height / rows;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    partsArray[i, j] = new Bitmap(cropImgWidth, cropImgHeight);

                    using (var graphics = Graphics.FromImage(partsArray[i, j]))
                    {
                        graphics.DrawImage(bmp, new Rectangle(0, 0, cropImgWidth, cropImgHeight), new Rectangle(j * cropImgWidth, i * cropImgHeight, cropImgWidth, cropImgHeight), GraphicsUnit.Pixel);
                    }
                }
            }

            return partsArray;
        }

        public unsafe double FindArrayMaxVal(Bitmap bmp)
        {

            double maxPixelVal = 0;

            BitmapData bitmapDataTile = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

            byte* TilePointer = (byte*)bitmapDataTile.Scan0;

            for (int i = 0; i < bitmapDataTile.Height; i++)
            {
                for (int j = 0; j < bitmapDataTile.Width; j++)
                {

                    maxPixelVal = Math.Max(maxPixelVal, TilePointer[0]);


                    TilePointer += 4;
                }

                TilePointer += bitmapDataTile.Stride - (bitmapDataTile.Width * 4);
            }

            bmp.UnlockBits(bitmapDataTile);

            return maxPixelVal;
        }

        public Rectangle createRectangle(int x1, int y1, int x2, int y2)
        {
            Rectangle rect = new Rectangle();

            rect.X = Math.Min(x1, x2);
            rect.Y = Math.Min(y1, y2);
            rect.Width = Math.Abs(x1 - x2);
            rect.Height = Math.Abs(y1 - y2);

            return rect;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
        }
    }
}
