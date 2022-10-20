﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using System.Diagnostics;
using System.Drawing.Imaging;
using Emgu.CV.Reg;

namespace EmguCvInter
{
    public class CropAndNormal : IDisposable
    {
        public bool Disposed;
        private PictureBox p2;
        private PictureBox p3;
        private PictureBox p4;
        private int Count;

        public CropAndNormal(PictureBox p2, PictureBox p3, PictureBox p4, int Count)
        {
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
            this.Count = Count;
        }

        public void CropImage(Image<Bgr, Byte> unprocessedImage, Image<Gray, Byte> nivImage)
        {
            SkewCorrection newSkew = new SkewCorrection();

            double[] matMax;
            double[] matMin;
            Point[] matMinLoc;
            Point[] matMaxLoc;

            float[,] dilateKernel =
            {
                { 1, 1, 1},
                { 1, 1, 1},
                { 1, 1, 1}
            };

            ConvolutionKernelF dilateMatrixKernel = new ConvolutionKernelF(dilateKernel);

            try
            {
                // 35ms run time
                var original = newSkew.SkewOrg(nivImage);

                var skewNiv = newSkew.SkewNiv(nivImage);

                this.p2.Image = original;

                /*Stopwatch sw = Stopwatch.StartNew();
                sw.Stop();
                Debug.WriteLine("Gray bitlock Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);*/

                // --------------

                //Nivalation(unprocessedImage.ToBitmap(), nivImage.ToBitmap());
                // --------------

                //((col > 200 && col < 280 && row > 500 && row < 550) || 
                //(col > 200 && col < 280 && row > 1950 && row < 2000) ||
                //(col > 1710 && col < 1730 && row > 500 && row < 550) ||
                //(col > 1730 && col < 1750 && row > 1950 && row < 2000))

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                newSkew.Dispose();
            }
        }

        private void Nivalation(Bitmap cropedTile, Bitmap cropedNiv)
        {

            var nivalated = new Bitmap(cropedNiv.Width, cropedNiv.Height, PixelFormat.Format32bppRgb);
            string savePath = "C:\\Users\\kemerios\\Desktop\\imageProcess\\procesed\\";
            double[,] pixelData = new double[nivalated.Width, nivalated.Height];

            try
            {
                unsafe
                {
                    BitmapData bitmapDataTile = cropedTile.LockBits(new Rectangle(0, 0, cropedTile.Width, cropedTile.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    BitmapData bitmapDataNiv = cropedNiv.LockBits(new Rectangle(0, 0, cropedNiv.Width, cropedNiv.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

                    byte* TilePointer = (byte*)bitmapDataTile.Scan0;
                    byte* NivPointer = (byte*)bitmapDataNiv.Scan0;

                    Stopwatch sw = Stopwatch.StartNew();

                    for (int i = 0; i < bitmapDataTile.Height; i++)
                    {
                        for (int j = 0; j < bitmapDataTile.Width; j++)
                        {
                            pixelData[j, i] = Math.Sqrt(255 * TilePointer[0] / (NivPointer[0] + 1));

                            TilePointer += 4;
                            NivPointer += 4;
                        }

                        TilePointer += bitmapDataTile.Stride - (bitmapDataTile.Width * 4);
                        NivPointer += bitmapDataNiv.Stride - (bitmapDataNiv.Width * 4);
                    }

                    sw.Stop();
                    Debug.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);

                    cropedTile.UnlockBits(bitmapDataTile);
                    cropedNiv.UnlockBits(bitmapDataNiv);

                }

                double maxPixelVal = 0;

                for (int i = 0; i < pixelData.GetLength(1); i++)
                {
                    for (int j = 0; j < pixelData.GetLength(0); j++)
                    {
                        maxPixelVal = Math.Max(maxPixelVal, pixelData[j, i]);
                    }
                }

                unsafe
                {
                    BitmapData bitmapDataNiv = nivalated.LockBits(new Rectangle(0, 0, nivalated.Width, nivalated.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    byte* NivalatedPointer = (byte*)bitmapDataNiv.Scan0;

                    for (int i = 0; i < nivalated.Height; i++)
                    {
                        for (int j = 0; j < nivalated.Width; j++)
                        {

                            var temp = Convert.ToInt32((pixelData[j, i] * 255) / maxPixelVal);

                            NivalatedPointer[0] = (byte)temp;
                            NivalatedPointer[1] = (byte)temp;
                            NivalatedPointer[2] = (byte)temp;

                            NivalatedPointer += 4;
                        }
                        NivalatedPointer += bitmapDataNiv.Stride - (bitmapDataNiv.Width * 4);
                    }
                    nivalated.UnlockBits(bitmapDataNiv);
                }

                nivalated.Save(savePath + Count.ToString() + ".png", ImageFormat.Png);
            }
            finally
            {
                if (nivalated != null)
                {
                    nivalated.Dispose();
                }
            }
        }

        private static Rectangle createRectangle(int x1, int y1, int x2, int y2)
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
