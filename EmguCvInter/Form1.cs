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
        

        private int counterImg = -1;
        private int counterNiv = -1;
        private List<Image<Bgr, Byte>> imageUnList = null;
        private List<Image<Gray, Byte>> imageNivList = null;

        public Form1()
        {
            InitializeComponent();
            imageUnList = UnprocessedImageList.UnprocessedImageOpener();
            imageNivList = NivImageList.NivImageOpener();
        }

        private void initImages(int countImg, int countNiv)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;

            using (var processedImageCC = new CropAndNormal(MyPictureBox2, MyPictureBox3, MyPictureBox4, MyPictureBox5, countImg))
            {

                if (countImg > -1 && countImg < imageUnList.Count)
                {
                    //pictureBox1.Image = imageUnList[countImg].ToBitmap();
                    processedImageCC.CropImage(imageUnList[countImg], imageNivList[countNiv]);
                    //processedImageCC.NivImgHomogeity();
                    processedImageCC.NivalationUn();
                    //processedImageCC.Nivalation();
                    processedImageCC.graphPrepare();
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
            if (counterImg < this.imageUnList.Count)
            {
                counterImg++;

                if (counterNiv < 1)
                {
                    counterNiv++;
                }
                else
                {
                    counterNiv = 0;
                }

                this.initImages(counterImg, counterNiv);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (counterImg > -1)
            {
                counterImg--;

                if (counterNiv == 0)
                {
                    counterNiv = 1;
                }
                else
                {
                    counterNiv = 0;
                }

                this.initImages(counterImg, counterNiv);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}