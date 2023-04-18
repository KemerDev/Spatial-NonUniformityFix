using Emgu.CV;
using Emgu.CV.Structure;
using System.Diagnostics;
using System.Drawing.Imaging;
using CsvHelper;
using System.Text;

namespace EmguCvInter
{
    public class CropAndNormal : IDisposable
    {
        public bool Disposed;
        private PictureBox p2;
        private PictureBox p3;
        private PictureBox p4;
        private PictureBox p5;
        private int Count;
        private int rows = 4;
        private int cols = 3;
        private Bitmap nivalated = null;
        private Bitmap cropedTileBmp = null;
        private Bitmap cropedNivBmp = null;
        private Bitmap cropedNivFinalBmp = null;
        private int lineCount = 0;
        private double maxPixelVal;

        CalcStdDiv stdDiv = new CalcStdDiv();
        CalculationsClass calculations = new CalculationsClass();

        public CropAndNormal(PictureBox p2, PictureBox p3, PictureBox p4, PictureBox p5, int Count)
        {
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
            this.p5 = p5;
            this.Count = Count;
        }

        public void CropImage(Image<Bgr, Byte> unprocessedImage, Image<Gray, Byte> nivImage)
        {
            try
            {
                // 35ms run time
                var original = unprocessedImage.Convert<Gray, byte>().Rotate(4, new Gray());

                var skewNiv = nivImage.Convert<Gray, byte>().Rotate(4, new Gray());

                Point point1 = new Point(570, 300);
                Point point2 = new Point(1950, 1680);

                /*Stopwatch sw = Stopwatch.StartNew();
                sw.Stop();
                Debug.WriteLine("Gray bitlock Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);*/

                // --------------

                var rect = calculations.createRectangle(point1.X, point1.Y, point2.X, point2.Y);

                original.ROI = rect;
                skewNiv.ROI = rect;

                var cropedTile = original.Copy();
                var cropedNiv = skewNiv.Copy();

                original.ROI = Rectangle.Empty;
                skewNiv.ROI = Rectangle.Empty;

                cropedTileBmp = cropedTile.ToBitmap();
                cropedNivBmp = cropedNiv.ToBitmap();

                maxPixelVal = calculations.FindArrayMaxVal(cropedTileBmp);

                calculations.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /*public void NivImgHomogeity()
        {
            var cropImgWidth = cropedTileBmp.Width / cols;
            var cropImgHeight = cropedTileBmp.Height / rows;

            var bmpToImg = cropedNivBmp.ToImage<Gray, byte>();

            MCvScalar mean = new MCvScalar();
            MCvScalar stdv = new MCvScalar();

            CvInvoke.MeanStdDev(bmpToImg, ref mean, ref stdv);

            byte[,] valueArr = new byte[cropedNivBmp.Height, cropedNivBmp.Width];

            unsafe
            {

                BitmapData bitmapData = cropedNivBmp.LockBits(new Rectangle(0, 0, cropedNivBmp.Width, cropedNivBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                byte* NivPointer = (byte*)bitmapData.Scan0;

                for (int x = 0; x < cropedNivBmp.Height; x++)
                {
                    for (int y = 0; y < cropedNivBmp.Width; y++)
                    {
                        valueArr[x, y] = (byte)(NivPointer[0] - mean.V0);

                        NivPointer += 4;
                    }
                    NivPointer += bitmapData.Stride - (bitmapData.Width * 4);
                }
                cropedNivBmp.UnlockBits(bitmapData);
            }

            cropedNivFinalBmp = new Bitmap(cropedNivBmp.Width, cropedNivBmp.Height);

            unsafe
            {
                BitmapData bmpNivData = cropedNivFinalBmp.LockBits(new Rectangle(0, 0, cropedNivBmp.Width, cropedNivBmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                byte* NivPointer = (byte*)bmpNivData.Scan0;

                BitmapData bmpGrayData = cropedNivBmp.LockBits(new Rectangle(0, 0, cropedNivBmp.Width, cropedNivBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                byte* NivGrayPointer = (byte*)bmpGrayData.Scan0;

                for (int x = 0; x < cropedNivBmp.Height; x++)
                {
                    for (int y = 0; y < cropedNivBmp.Width; y++)
                    {
                        NivPointer[0] = (byte)(NivGrayPointer[0] - valueArr[x, y]);
                        NivPointer[1] = (byte)(NivGrayPointer[0] - valueArr[x, y]);
                        NivPointer[2] = (byte)(NivGrayPointer[0] - valueArr[x, y]);

                        NivPointer += 4;
                        NivGrayPointer += 4;
                    }
                    NivPointer += bmpNivData.Stride - (bmpNivData.Width * 4);
                    NivGrayPointer += bmpGrayData.Stride - (bmpGrayData.Width * 4);
                }
                cropedNivFinalBmp.UnlockBits(bmpNivData);
                cropedNivBmp.UnlockBits(bmpGrayData);
            }

            string savePath = "C:\\Users\\kemerios\\Desktop\\imageProcess\\procesed\\";

            cropedNivFinalBmp.Save(savePath + "test" + Count.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);

            calculations.Dispose();
        }*/

        /*public void Nivalation()
        {

            var cropImgWidth = cropedTileBmp.Width / cols;
            var cropImgHeight = cropedTileBmp.Height / rows;

            nivalated = new Bitmap(cropedTileBmp.Width, cropedTileBmp.Height, PixelFormat.Format32bppRgb);
            string savePath = "C:\\Users\\kemerios\\Desktop\\imageProcess\\procesed\\";
            double[,] pixelData = new double[nivalated.Width, nivalated.Height];

            try
            {
                unsafe
                {

                    BitmapData bitmapDataTile = cropedTileBmp.LockBits(new Rectangle(0, 0, cropedTileBmp.Width, cropedTileBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    BitmapData bitmapDataNiv = cropedNivBmp.LockBits(new Rectangle(0, 0, cropedNivBmp.Width, cropedNivBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

                    byte* TilePointer = (byte*)bitmapDataTile.Scan0;
                    byte* NivPointer = (byte*)bitmapDataNiv.Scan0;

                    for (int i = 0; i < bitmapDataTile.Height; i++)
                    {
                        for (int j = 0; j < bitmapDataTile.Width; j++)
                        {
                            pixelData[j, i] = Math.Sqrt(255 * TilePointer[0] / NivPointer[0] + 1);

                            TilePointer += 4;
                            NivPointer += 4;
                        }

                        TilePointer += bitmapDataTile.Stride - (bitmapDataTile.Width * 4);
                        NivPointer += bitmapDataNiv.Stride - (bitmapDataNiv.Width * 4);
                    }

                    cropedTileBmp.UnlockBits(bitmapDataTile);
                    cropedNivBmp.UnlockBits(bitmapDataNiv);

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

                            var temp = pixelData[j, i] * 255 / maxPixelVal;

                            NivalatedPointer[0] = (byte)temp;
                            NivalatedPointer[1] = (byte)temp;
                            NivalatedPointer[2] = (byte)temp;

                            NivalatedPointer += 4;
                        }
                        NivalatedPointer += bitmapDataNiv.Stride - (bitmapDataNiv.Width * 4);
                    }
                    nivalated.UnlockBits(bitmapDataNiv);
                }

                nivalated.Save(savePath + Count.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);

                var emguImg = nivalated.ToImage<Gray, byte>();

                this.p4.Image = emguImg.ToBitmap();

                var nivParts = calculations.SplitImageToParts(emguImg.ToBitmap(), rows, cols);
                var meanStdvList = calculations.CalcPartMeanStdv(nivParts, rows, cols);

                this.p5.Image = stdDiv.createNewPartImg(nivParts, meanStdvList[1], cropedTileBmp.Width, cropedTileBmp.Height, cropImgWidth, cropImgHeight);
            }

            finally
            {
                if (nivalated != null)
                {
                    nivalated.Dispose();
                }
            }
        }*/

        public void NivalationUn()
        {
            nivalated = new Bitmap(cropedTileBmp.Width, cropedTileBmp.Height, PixelFormat.Format32bppRgb);

            Debug.WriteLine(maxPixelVal);

            Stopwatch sw = Stopwatch.StartNew();

            unsafe
            {

                BitmapData bitmapDataTile = cropedTileBmp.LockBits(new Rectangle(0, 0, cropedTileBmp.Width, cropedTileBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                BitmapData bitmapDataNiv = cropedNivBmp.LockBits(new Rectangle(0, 0, cropedNivBmp.Width, cropedNivBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                BitmapData bitmapNivalated = nivalated.LockBits(new Rectangle(0, 0, nivalated.Width, nivalated.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

                byte* TilePointer = (byte*)bitmapDataTile.Scan0;
                byte* NivPointer = (byte*)bitmapDataNiv.Scan0;
                byte* NivalatedPointer = (byte*)bitmapNivalated.Scan0;

                for (int i = 0; i < bitmapDataTile.Height; i++)
                {
                    for (int j = 0; j < bitmapDataTile.Width; j++)
                    {
                        var tempSqrt = Math.Sqrt(255.0 * TilePointer[0] / NivPointer[0] + 1);

                        var tempVal = tempSqrt * 255.0 / maxPixelVal;

                        NivalatedPointer[0] = (byte)tempVal;
                        NivalatedPointer[1] = (byte)tempVal;
                        NivalatedPointer[2] = (byte)tempVal;

                        TilePointer += 4;
                        NivPointer += 4;
                        NivalatedPointer += 4;
                    }

                    TilePointer += bitmapDataTile.Stride - (bitmapDataTile.Width * 4);
                    NivPointer += bitmapDataNiv.Stride - (bitmapDataNiv.Width * 4);
                    NivalatedPointer += bitmapNivalated.Stride - (bitmapNivalated.Width * 4);
                }

                cropedTileBmp.UnlockBits(bitmapDataTile);
                cropedNivBmp.UnlockBits(bitmapDataNiv);
                nivalated.UnlockBits(bitmapNivalated);
            }

            sw.Stop();
            Debug.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
        }

        public void graphPrepare()
        {
            string savePathCsv = @"D:/temp/";
            string savePathProc = @"D:/temp/";

            var cropImgWidth = cropedTileBmp.Width / cols;
            var cropImgHeight = cropedTileBmp.Height / rows;

            this.p2.Image = nivalated;

            var picNiv = nivalated.ToImage<Gray, byte>();

            var smoothNiv = new Image<Gray, byte>(nivalated.Width, nivalated.Height);

            CvInvoke.GaussianBlur(picNiv, smoothNiv, new Size(33, 33), 0, 0);

            var dividedNiv = new Image<Gray, byte>(nivalated.Width, nivalated.Height);

            CvInvoke.Divide(picNiv, smoothNiv, dividedNiv, maxPixelVal);

            this.p4.Image = smoothNiv.ToBitmap();

            this.p5.Image = dividedNiv.ToBitmap();

            var nivParts = calculations.SplitImageToParts(dividedNiv.ToBitmap(), rows, cols);
            var meanStdvList = calculations.CalcPartMeanStdv(nivParts, rows, cols);

            this.p3.Image = stdDiv.createNewPartImg(nivParts, meanStdvList[1], cropedNivBmp.Width, cropedNivBmp.Height, cropImgWidth, cropImgHeight);

            var finalStdv = stdDiv.CalcFinalDev(meanStdvList[1]);

            var picMean = 0d;

            for (int i = 0; i < meanStdvList[1].GetLength(0); i++)
            {
                for (int j = 0; j < meanStdvList[1].GetLength(1); j++)
                {
                    picMean += meanStdvList[1][i, j];
                }
            }

            picMean = picMean / (meanStdvList[1].GetLength(0) * meanStdvList[1].GetLength(1));

            using (var fileWrite = new StreamWriter(File.Open(savePathCsv + "output.csv", FileMode.Append)))
            {
                for (int i = 0; i < meanStdvList[1].GetLength(0); i++)
                {
                    for (int j = 0; j < meanStdvList[1].GetLength(1); j++)
                    {
                        var line = String.Format("{0},{1}", meanStdvList[1][i, j], meanStdvList[1][i,j] - picMean);

                        fileWrite.WriteLine(line);
                        fileWrite.Flush();
                    }
                }

                var space = String.Format("{0}", Environment.NewLine);

                fileWrite.WriteLine(space);
                fileWrite.Flush();
            }

            dividedNiv.ToBitmap().Save(savePathProc + Count.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
        }
    }
}
