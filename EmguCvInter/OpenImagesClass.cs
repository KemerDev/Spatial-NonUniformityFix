using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace EmguCvInter
{
    public class OpenImagesClass
    {
        public List<Image<Bgr, Byte>> UnprocessedImageOpener()
        {
            List<Image<Bgr, Byte>> imageList = new List<Image<Bgr, byte>>();
            try
            {
                string[] fileArray = Directory.GetFiles(@"D:\temp\tempF");

                foreach (string file in fileArray)
                {
                    imageList.Add(new Image<Bgr, Byte>(file));
                }

                return imageList;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return imageList;
            }
        }

        public List<Image<Gray, Byte>> NivImageOpener()
        {
            List<Image<Gray, Byte>> imageList = new List<Image<Gray, byte>>();

            try
            {
                string[] fileArray = Directory.GetFiles(@"D:\temp\temp_maps");

                foreach (string file in fileArray)
                {
                    imageList.Add(new Image<Gray, Byte>(file));
                }

                return imageList;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return imageList;
            }
        }
    }
}
