using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FourPanelComicMaker
{
    public partial class Form2 : Form
    {
        Bitmap resultImg;
        public Form2(Bitmap img)
        {
            InitializeComponent();
            if (img.Height > 4000 || img.Width > 4000)
                resultImg = new Bitmap(img, new Size(img.Width / 2, img.Height / 2));
            else
                resultImg = img;
            int newHeight = pictureBox1.Width * img.Height / img.Width;
            this.Size = new Size(this.Size.Width, newHeight + 110);
            pictureBox1.Height = newHeight;
            pictureBox1.Image = resultImg;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + @"\data";
            saveFileDialog1.FileName = "作品.jpg";
            saveFileDialog1.Filter = "JPG|*.jpg";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                resultImg.Save(saveFileDialog1.FileName);
            }
        }
    }
}
