using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguCvInter
{
    public class HomogImg
    {
        private PictureBox p4;
        public HomogImg(PictureBox p4)
        {
            this.p4 = p4;
        }

        public unsafe void ImgHomogeneity(Bitmap bmp, String savePath, int Count)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int bytesPerPixels = 4;
            int maxPointerlen = width * height * bytesPerPixels;
            int stride = width * bytesPerPixels;

            double mean;

            double[] meanFilter = { 1, 1, 1 };

            Bitmap homoBmp = new Bitmap(width, height);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData homoData = homoBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            byte* bmpPointer = (byte*)bmpData.Scan0.ToPointer();
            byte* bmpHomo = (byte*)homoData.Scan0.ToPointer();

            for (int i = 0; i < maxPointerlen / 2; i += 4)
            {
                for (int j = 0; j < width / 3; j++)
                {
                    mean = (bmpPointer[stride * (j + 0) + (i + 0) + 0] * meanFilter[0] + bmpPointer[stride * (j + 0) + (i + 1) + 0] * meanFilter[1] + bmpPointer[stride * (j + 0) + (i + 2) + 0] * meanFilter[2] +
                        bmpPointer[stride * (j + 1) + (i + 0) + 0] * meanFilter[0] + bmpPointer[stride * (j + 1) + (i + 1) + 0] * meanFilter[1] + bmpPointer[stride * (j + 1) + (i + 2) + 0] * meanFilter[2] +
                        bmpPointer[stride * (j + 2) + (i + 0) + 0] * meanFilter[0] + bmpPointer[stride * (j + 2) + (i + 1) + 0] * meanFilter[1] + bmpPointer[stride * (j + 2) + (i + 2) + 0] * meanFilter[2]) / 9;

                    bmpHomo[i + 0] = (byte)(mean);
                    bmpHomo[i + 1] = (byte)(mean);
                    bmpHomo[i + 2] = (byte)(mean);
                }
            }

            bmp.UnlockBits(bmpData);
            homoBmp.UnlockBits(homoData);

            this.p4.Image = homoBmp;

            homoBmp.Save(savePath + Count.ToString() + ".png", ImageFormat.Png);
        }
    }
}
