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
using System.Diagnostics;
using System.Drawing.Imaging;
using Emgu.CV.Reg;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EmguCvInter
{
    public class SkewCorrection : IDisposable
    {
        public bool Disposed;
        Point anglePoint1 = new Point();
        Point anglePoint2 = new Point();

        public Bitmap SkewOrg(Image<Gray, Byte> img)
        {
            var bmp = img.ToBitmap();

            var img_height = bmp.Height;
            var img_width = bmp.Width;

            int x = img_width / 2;
            int y = img_height / 2;

            Color idxColor = bmp.GetPixel(x, y);

            Debug.WriteLine(idxColor);

            Color colorT = Color.FromArgb(255, 255, 255, 255);
            Color colorF = Color.FromArgb(255, 1, 1, 1);

            return FloodFill(bmp, idxColor, colorT, colorF); ;
        }

        private Bitmap FloodFill(Bitmap bmp, Color idxColor, Color colorT, Color colorF)
        {
            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height);

            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                BitmapData newbitmapData = newBmp.LockBits(new Rectangle(0, 0, newBmp.Width, newBmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

                byte* floodPointer = (byte*)bitmapData.Scan0;
                byte* newfloodPointer = (byte*)newbitmapData.Scan0;

                int temp = 0;

                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        var color = getPixel(bmp, bitmapData, j, i);

                        if (color.B >= idxColor.B - 85 && color.B <= idxColor.B + 85)
                        {
                            temp = colorT.B;
                        }
                        else
                        {
                            temp = colorF.B;
                        }

                        newfloodPointer[0] = (byte)temp;
                        newfloodPointer[1] = (byte)temp;
                        newfloodPointer[2] = (byte)temp;

                        floodPointer += 4;
                        newfloodPointer += 4;
                    }

                    floodPointer += bitmapData.Stride - (bitmapData.Width * 4);
                    newfloodPointer += newbitmapData.Stride - (newbitmapData.Width * 4);
                }
                bmp.UnlockBits(bitmapData);
                newBmp.UnlockBits(newbitmapData);
            }

            return newBmp;
        }

        private unsafe Color getPixel(Bitmap bmp, BitmapData bmData, int x, int y)
        {

            int pixWidth = 4;

            IntPtr scan0 = bmData.Scan0;
            int stride = bmData.Stride;
            byte* p = (byte*)scan0.ToPointer() + y * stride;
            int px = x * pixWidth;
            Color color = Color.FromArgb(255, p[px + 2], p[px + 1], p[px + 0]);

            return color;
        }


        public Image<Gray, Byte> SkewNiv(Image<Gray, Byte> img)
        {
            var unProsImg = img.Copy();

            var radian = Math.Atan2((anglePoint2.Y - anglePoint1.Y), (anglePoint2.X - anglePoint1.X));
            var angle = radian * (180 / Math.PI);

            return unProsImg.Rotate(Math.Abs(angle), new Gray());
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
        }
    }
}