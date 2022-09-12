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
        public List<Image<Gray, Byte>> UnprocessedImageOpener()
        {
            List<Image<Gray, Byte>> imageList = new List<Image<Gray, byte>>();
            try
            {
                string[] fileArray = Directory.GetFiles(@"C:\Users\kemerios\Desktop\imageProcess\unprosF");

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

        public List<Image<Gray, Byte>> NivImageOpener()
        {
            List<Image<Gray, Byte>> imageList = new List<Image<Gray, byte>>();

            try
            {
                string[] fileArray = Directory.GetFiles(@"C:\Users\kemerios\Desktop\imageProcess\normal_maps");

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
