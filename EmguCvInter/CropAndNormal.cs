using static EmguCvInter.Form1;
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
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace EmguCvInter
{
    public class CropAndNormal
    {
        public List<Image<Gray, Byte>>? ConnectedComponents(Image<Gray, Byte> unprocessedImage, Image<Gray, Byte> nivImage)
        {

            if (unprocessedImage == null)
            {
                return null;
            }

            try
            {

                List<Image<Gray, Byte>> finalList = new List<Image<Gray, Byte>>();

                var original = unprocessedImage.Copy();
                var unProsImg = unprocessedImage.Copy();
                var AdaptiveThresImg = unprocessedImage.Copy();
                var ContourImg = unprocessedImage.Copy();

                CvInvoke.AdaptiveThreshold(unProsImg, AdaptiveThresImg, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 5, 0);
                unProsImg = unProsImg.SmoothGaussian(25).ThresholdBinary(new Gray(30), new Gray(255));

                finalList.Add(unProsImg);
                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

                Mat hier = new Mat();

                finalList.Add(AdaptiveThresImg);
                CvInvoke.FindContours(unProsImg, contours, hier, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);
                CvInvoke.DrawContours(original, contours[1], -1, new MCvScalar(255, 0, 0));

                finalList.Add(original);


                /*var unprosImg = unprocessedImage.Rotate(4, new Gray(255));
                var nivImg = nivImage.Rotate(4, new Gray(255));

                Rectangle rect = new Rectangle(525, 255, 1450, 1470);

                unprosImg.ROI = rect;
                nivImg.ROI = rect;

                var unprosStep = unprosImg.Copy();
                finalList.Add(unprosStep);

                var nivStep = nivImg.Copy();

                var NormalizedImg = unprosStep.Copy();

                CvInvoke.Normalize(unprosStep, NormalizedImg, 0, 255, Emgu.CV.CvEnum.NormType.MinMax, Emgu.CV.CvEnum.DepthType.Cv8U);

                finalList.Add(nivStep);
                finalList.Add(NormalizedImg);

                //unprosImg.SetValue(new Gray(255));
                //unprosImg._Mul(unprosStep);

                unprosImg.ROI = Rectangle.Empty;
                nivImg.ROI = Rectangle.Empty;

                //var mask = nivImage.SmoothGaussian(3).ThresholdBinaryInv(new Gray(30), new Gray(255));
                //finalList.Add(mask);*/

                return finalList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }

    public class normalization
    {
        public normalization()
        {

        }
    }
}
