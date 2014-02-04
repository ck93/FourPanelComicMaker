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
            resultImg = img;
            pictureBox1.Height = pictureBox1.Width * img.Height / img.Width;
            pictureBox1.Image = img;
            button1.Location = new Point(button1.Location.X, pictureBox1.Height + 25);
            this.Size = new Size(this.Size.Width, pictureBox1.Height + 110);
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
