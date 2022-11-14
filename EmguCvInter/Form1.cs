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
        

        private int counter = -1;
        private List<Image<Bgr, Byte>> imageUnList = null;
        private List<Image<Gray, Byte>> imageNivList = null;

        public Form1()
        {
            InitializeComponent();
            imageUnList = UnprocessedImageList.UnprocessedImageOpener();
            imageNivList = NivImageList.NivImageOpener();
        }

        private void initImages(int count)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;

            using (var processedImageCC = new CropAndNormal(MyPictureBox2, MyPictureBox3, MyPictureBox4, MyPictureBox5, count))
            {

                if (count > -1 && count < imageUnList.Count)
                {
                    pictureBox1.Image = imageUnList[count].ToBitmap();
                    processedImageCC.CropImage(imageUnList[count], imageNivList[count]);
                }
            }
        }
        public PictureBox MyPictureBox2
        {
            get
            {
                return pictureBox2;
            }
        }

        public PictureBox MyPictureBox3
        {
            get
            {
                return pictureBox3;
            }
        }

        public PictureBox MyPictureBox4
        {
            get
            {
                return pictureBox4;
            }
        }

        public PictureBox MyPictureBox5
        {
            get
            {
                return pictureBox5;
            }
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}