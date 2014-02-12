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
    public partial class Tutorial : Form
    {
        int index = 0;
        Point mousePoint = new Point();
        Font myFont = new Font("微软雅黑", 3.5f);
        Image origin = new Bitmap(Properties.Resources.resPath + "tutorial_center.jpg");
        Bitmap picBoard;
        int split = 450;
        int direction;

        string[] tips = new string[6];

        public Tutorial()
        {
            InitializeComponent();
            this.BackgroundImage = new Bitmap(Properties.Resources.resPath + "tutorial.jpg");
            picBoard = (Bitmap)origin.Clone();
            pictureBox3.Image = picBoard;
            if (ConfigClass.GetValue("ShowTutorial") == "Yes")
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;
            ReadTips();
        }

        void ReadTips()
        {
            StreamReader sr = new StreamReader(@".\使用说明.txt", Encoding.Default);
            for (int i = 0; i < 6; i++)
            {
                tips[i] += sr.ReadLine() + "\r\n";
                tips[i] += sr.ReadLine();
            }            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Tutorial_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint.X = e.X;
            mousePoint.Y = e.Y;
        }

        private void Tutorial_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Top = Control.MousePosition.Y - mousePoint.Y;
                this.Left = Control.MousePosition.X - mousePoint.X;
            }
        }
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            timer1.Dispose();
            if (index == 0)
            {
                DrawTip();
                return;
            }
            index = (--index >= 0) ? index : 0;
            direction = -1;
            split = 0;
            timer1.Start();
            label2.Text = (index + 1).ToString() + "/6";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            timer1.Dispose();
            if (index == 5)
            {
                DrawTip();
                return;
            }
            index = (++index <= 5) ? index : 5;
            direction = 1;
            split = 450;
            timer1.Start();
            label2.Text = (index + 1).ToString() + "/6";
        }

        void DrawSlide()
        {
            picBoard = (Bitmap)origin.Clone();
            Graphics g = Graphics.FromImage(picBoard);
            Rectangle rect1 = new Rectangle(split - 450, 0, 450, 165);
            Rectangle rect2 = new Rectangle(split, 0, 450, 165);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            if (direction == 1)
            {
                g.DrawString(tips[index - 1], myFont, Brushes.Black, rect1, stringFormat);
                g.DrawString(tips[index], myFont, Brushes.Black, rect2, stringFormat);
            }
            else if (direction == -1)
            {
                g.DrawString(tips[index], myFont, Brushes.Black, rect1, stringFormat);
                g.DrawString(tips[index + 1], myFont, Brushes.Black, rect2, stringFormat);
            }
            g.Dispose();
            pictureBox3.Image.Dispose();
            pictureBox3.Image = picBoard;
        }

        public void DrawTip()
        {
            picBoard = (Bitmap)origin.Clone();
            Graphics g = Graphics.FromImage(picBoard);
            Rectangle rect = new Rectangle(0, 0, 450, 165);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;            
            g.DrawString(tips[index], myFont, Brushes.Black, rect, stringFormat);                
            g.Dispose();
            pictureBox3.Image.Dispose();
            pictureBox3.Image = picBoard;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (split < 0 || split > 450)
            {
                timer1.Stop();
                return;
            }
            DrawSlide();
            if (direction == 1)
                split -= 30;
            else if (direction == -1)
                split += 30;
            
        }

        private void Tutorial_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                pictureBox1_Click(sender, e);                
            }
            else if (e.KeyCode == Keys.D)
            {
                pictureBox2_Click(sender, e);
            }
        }

        private void Tutorial_Shown(object sender, EventArgs e)
        {
            DrawTip();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                ConfigClass.SetValue("ShowTutorial", "Yes");
            else
                ConfigClass.SetValue("ShowTutorial", "No");
        }

    }
}
