using System;
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
using Numpy;
using System.Diagnostics;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms.VisualStyles;
using Emgu.CV.Reg;
using System.Security.Cryptography;

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

            float[,] dilateKernel =
            {
                { 1, 1, 1},
                { 1, 1, 1},
                { 1, 1, 1}
            };

            double[] matMax;
            double[] matMin;
            Point[] matMinLoc;
            Point[] matMaxLoc;

            ConvolutionKernelF dilateMatrixKernel = new ConvolutionKernelF(dilateKernel);;

            try
            {
                // 35ms run time
                var original = unprocessedImage.Copy().Rotate(4, new Bgr());

                var unProsImg = unprocessedImage.Convert<Gray, byte>().Rotate(4, new Gray());

                /*Stopwatch sw = Stopwatch.StartNew();
                sw.Stop();
                Debug.WriteLine("Gray bitlock Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);*/
                
                // --------------

                var x1Point = new Point();
                var x2Point = new Point();

                var corners = new Image<Gray, float>(original.Size);

                // 69ms run time
                CvInvoke.CornerHarris(unProsImg, corners, 10, 19, 0.04);
                // --------------

                CvInvoke.Dilate(corners, corners, dilateMatrixKernel, new Point(0, 0), 2, 0, new MCvScalar(255, 0, 0));

                corners.MinMax(out matMin, out matMax, out matMinLoc, out matMaxLoc);

                // 0.0045ms run time
                for (int i = 530; i < 540; i++)
                {
                    for (int j = 255; j < 280; j++)
                    {
                        if (corners.Data[j, i, 0] > 0.003 * matMax[0])
                        {
                            x1Point.X = i;
                            x1Point.Y = j;
                        }
                    }
                }

                for (int i = 1970; i < 1985; i++)
                {
                    for (int j = 1725; j < 1755; j++)
                    {
                        if (corners.Data[j, i, 0] > 0.003 * matMax[0])
                        {
                            x2Point.X = i;
                            x2Point.Y = j;
                        }
                    }
                }
                // --------------

                var rect = createRectangle(x1Point.X, x1Point.Y, x2Point.X, x2Point.Y);

                unProsImg.ROI = rect;
                nivImage.ROI = rect;
                var cropedTile = unProsImg.Copy();
                var cropedNiv = nivImage.Copy();

                original.ROI = Rectangle.Empty;
                nivImage.ROI = Rectangle.Empty;


                this.p2.Image = cropedTile.ToBitmap();
                this.p3.Image = cropedNiv.ToBitmap();

                Nivalation(cropedTile.ToBitmap(), cropedNiv.ToBitmap());
                // --------------

                //((col > 200 && col < 280 && row > 500 && row < 550) || 
                //(col > 200 && col < 280 && row > 1950 && row < 2000) ||
                //(col > 1710 && col < 1730 && row > 500 && row < 550) ||
                //(col > 1730 && col < 1750 && row > 1950 && row < 2000))

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Nivalation(Bitmap cropedTile, Bitmap cropedNiv)
        {

            var nivalated = new Bitmap(cropedNiv.Width, cropedNiv.Height, PixelFormat.Format32bppRgb);
            string savePath = "C:\\Users\\kemerios\\Desktop\\imageProcess\\procesed\\";
            int pixelData = 0;

            try
            {
                Stopwatch sw = Stopwatch.StartNew();

                unsafe
                {
                    BitmapData bitmapDataTile = cropedTile.LockBits(new Rectangle(0, 0, cropedTile.Width, cropedTile.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    BitmapData bitmapDataNiv = cropedNiv.LockBits(new Rectangle(0, 0, cropedNiv.Width, cropedNiv.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    BitmapData bitmapDataNivalated = nivalated.LockBits(new Rectangle(0, 0, nivalated.Width, nivalated.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

                    byte* TilePointer = (byte*)bitmapDataTile.Scan0;
                    byte* NivPointer = (byte*)bitmapDataNiv.Scan0;
                    byte* NivalatedPointer = (byte*)bitmapDataNivalated.Scan0;

                    for (int i = 0; i < bitmapDataTile.Height; i++)
                    {
                        for (int j = 0; j < bitmapDataTile.Width; j++)
                        {
                            if (TilePointer[0] == 0 && NivPointer[0] > 0)
                            {
                                pixelData = TilePointer[0] / NivPointer[0];
                            }
                            else
                            {
                                pixelData = NivPointer[0] / TilePointer[0];
                            }

                            NivalatedPointer[0] = (byte)pixelData;
                            NivalatedPointer[1] = (byte)pixelData;
                            NivalatedPointer[2] = (byte)pixelData;

                            TilePointer += 4;
                            NivPointer += 4;
                            NivalatedPointer += 4;
                        }

                        TilePointer += bitmapDataTile.Stride - (bitmapDataTile.Width * 4);
                        NivPointer += bitmapDataNiv.Stride - (bitmapDataNiv.Width * 4);
                        NivalatedPointer += bitmapDataTile.Stride - (bitmapDataTile.Width * 4);
                    }

                    nivalated.UnlockBits(bitmapDataNivalated);
                    cropedTile.UnlockBits(bitmapDataTile);
                    cropedNiv.UnlockBits(bitmapDataNiv);

                }

                sw.Stop();
                Debug.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);

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
