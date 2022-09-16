using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using Numpy;
using System.Diagnostics;
using OpenCL.Net;
using System.ComponentModel.Design;
using System.Drawing.Imaging;
using Emgu.CV.Dnn;

namespace EmguCvInter
{
    public class CropAndNormal : IDisposable
    {
        public bool Disposed;
        private PictureBox p2;
        private PictureBox p3;
        private PictureBox p4;

        public CropAndNormal(PictureBox p2, PictureBox p3, PictureBox p4)
        { 
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
        }

        public void ConnectedComponents(Image<Bgr, Byte> unprocessedImage, Image<Gray, Byte> nivImage)
        {

            float[,] dilateKernel =
            {
                { 1, 1, 1, 1, 1},
                { 1, 1, 1, 1, 1},
                { 1, 1, 1, 1, 1},
                { 1, 1, 1, 1, 1},
                { 1, 1, 1, 1, 1},
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

                // 85.2501ms run time
                var rect = createRectangle(x1Point.X, x1Point.Y, x2Point.X, x2Point.Y);

                unProsImg.ROI = rect;
                nivImage.ROI = rect;

                var cropedTile = unProsImg.Copy();
                var cropedNiv = nivImage.Copy();

                original.ROI = Rectangle.Empty;
                nivImage.ROI = Rectangle.Empty;

                nivalation(cropedTile, cropedNiv);
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

        private unsafe void nivalation(Image<Gray, Byte> cropedTile, Image<Gray, Byte> cropedNiv)
        {
            this.p2.Image = cropedTile.ToBitmap();
            this.p3.Image = cropedNiv.ToBitmap();
            int pixelData = 0;

            try
            {
                Stopwatch sw = Stopwatch.StartNew();

                // ~120ms run time
                for (int i = 0; i < cropedTile.Cols; i++)
                {
                    for (int j = 0; j < cropedTile.Rows; j++)
                    {

                        if (cropedTile.Data[j, i, 0] == 0)
                        {
                            pixelData = cropedTile.Data[j, i, 0] / cropedNiv.Data[j, i, 0];
                        }
                        else
                        {
                            pixelData = cropedNiv.Data[j, i, 0] / cropedTile.Data[j, i, 0];
                        }
                        
                        cropedTile.Data[j, i, 0] = (byte)pixelData;

                    }
                }
                // --------------
                sw.Stop();

                Debug.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            this.p4.Image = cropedTile.ToBitmap();

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
