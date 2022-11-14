using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguCvInter
{
    public class NivImgHomo : IDisposable
    {
        public bool Disposed;
        public unsafe Bitmap NivHomogeity(Bitmap bmp)
        {
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte* bmpPointer = (byte*)bmpData.Scan0;

            for (int k = 0; k < bmp.Height; k++)
            {
                for (int l = 0; l < bmp.Width; l++)
                {
                    bmpPointer[0] = (byte)(0.1 * bmpPointer[0] + 1);
                    bmpPointer[1] = (byte)(0.1 * bmpPointer[0] + 1);
                    bmpPointer[2] = (byte)(0.1 * bmpPointer[0] + 1);

                    bmpPointer += 4;
                }
                bmpPointer += bmpData.Stride - (bmpData.Width * 4);
            }
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
        }
    }
}
