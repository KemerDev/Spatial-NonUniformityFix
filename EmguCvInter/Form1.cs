using static EmguCvInter.OpenImagesClass;
using static EmguCvInter.CropAndNormal;
using System;
using System.Diagnostics;
using Emgu.CV;
using System.ComponentModel;
using Emgu.CV.Structure;

namespace EmguCvInter
{
    public partial class Form1 : Form
    {
        OpenImagesClass UnprocessedImageList = new OpenImagesClass();
        OpenImagesClass NivImageList = new OpenImagesClass();
        CropAndNormal processedImageCC = new CropAndNormal();
        private int counter = -1;
        private List<Image<Gray, Byte>> imageUnList = null;
        private List<Image<Gray, Byte>> imageNivList = null;

        public Form1()
        {
            InitializeComponent();
            this.imageUnList = UnprocessedImageList.UnprocessedImageOpener();
            this.imageNivList = NivImageList.NivImageOpener();
        }

        private void initImages(int count)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;

            //Mat histoReq = new Mat();

            if (count > -1 && count < this.imageUnList.Count)
            {
                pictureBox1.Image = this.imageUnList[count].ToBitmap();
                pictureBox2.Image = processedImageCC.ConnectedComponents(this.imageUnList[count], this.imageNivList[count])[0].ToBitmap();
                pictureBox3.Image = processedImageCC.ConnectedComponents(this.imageUnList[count], this.imageNivList[count])[1].ToBitmap();
                pictureBox4.Image = processedImageCC.ConnectedComponents(this.imageUnList[count], this.imageNivList[count])[2].ToBitmap();
            }

            //pictureBox2.Image = histoReq.ToBitmap();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (counter < this.imageUnList.Count)
            {
                counter++;
                this.initImages(counter);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (counter > -1)
            {
                counter--;
                this.initImages(counter);
            }
        }
    }
}