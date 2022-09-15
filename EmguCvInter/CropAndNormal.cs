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

namespace EmguCvInter
{
    public class CropAndNormal
    {
        private PictureBox p2;
        private PictureBox p3;

        public CropAndNormal(PictureBox p2, PictureBox p3)
        { 
            this.p2 = p2;
            this.p3 = p3;
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

                var original = unprocessedImage.Copy().Rotate(4, new Bgr());
                var unProsImg = unprocessedImage.Convert<Gray, byte>().Rotate(4, new Gray());

                var x1Point = new Point();
                var x2Point = new Point();

                var corners = new Image<Gray, float>(original.Size);

                CvInvoke.CornerHarris(unProsImg, corners, 10, 19, 0.04);

                CvInvoke.Dilate(corners, corners, dilateMatrixKernel, new Point(0, 0), 2, 0, new MCvScalar(255, 0, 0));

                corners.MinMax(out matMin, out matMax, out matMinLoc, out matMaxLoc);

                Stopwatch sw = Stopwatch.StartNew();
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
                sw.Stop();

                Debug.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);

                var rect = createRectangle(x1Point.X, x1Point.Y, x2Point.X, x2Point.Y);

                original.ROI = rect;

                var cropedTile = original.Copy();

                original.ROI = Rectangle.Empty;

                this.p3.Image = cropedTile.ToBitmap();

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

        private static Rectangle createRectangle(int x1, int y1, int x2, int y2)
        {
            Rectangle rect = new Rectangle();

            rect.X = Math.Min(x1, x2);
            rect.Y = Math.Min(y1, y2);
            rect.Width = Math.Abs(x1 - x2);
            rect.Height = Math.Abs(y1 - y2);

            return rect;
        }
    }
}
