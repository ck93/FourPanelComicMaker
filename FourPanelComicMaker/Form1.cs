using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Configuration;
//using System.Windows.Media;

namespace FourPanelComicMaker
{
    enum AddBubble
    {
        none, addBubble1, addBubble2, addBubble3, addBubble4, addBubble5
    }
    public partial class Form1 : Form
    {
        int picWidth = 500;
        int picHeight = 666;
        int bubbleWidth, bubbleHeight;
        static public int borderWidth;
        static public Color borderColor;
        static public FontFamily myFontFamily;
        static public string sign;
        Comic comic1, comic2, comic3, comic4;
        static public Bitmap[] bubbleImg;
        static public Rectangle[] textRegion = new Rectangle[5] { new Rectangle(43, 49, 126, 94), new Rectangle(35, 39, 134, 102),
           new Rectangle(37, 40, 117, 86), new Rectangle(22, 33, 164, 96), new Rectangle(24, 22, 116, 61)};
        int[] textOriginWidth = new int[5] {126, 134, 117, 164, 116};
        string filePath = @".\photo\";
        List<string> fileNames = new List<string>();
        Bitmap final;
        Image origin1, origin2, origin3, origin4;        
        AddBubble addMode = AddBubble.none;
        bool moveBubble = false;
        Bubble movingBubble = new Bubble();
        

        public Form1()
        {
            InitializeComponent();
            ReadImg();
            ReadConfig();
            bubbleImg = new Bitmap[5];
            bubbleImg[0] = new Bitmap(Properties.Resources.bubble1);
            bubbleImg[1] = new Bitmap(Properties.Resources.bubble2);
            bubbleImg[2] = new Bitmap(Properties.Resources.bubble3);
            bubbleImg[3] = new Bitmap(Properties.Resources.bubble4);
            bubbleImg[4] = new Bitmap(Properties.Resources.bubble5);
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            textBox8.Enabled = false;
            if (!Directory.Exists(@".\data"))
                Directory.CreateDirectory(@".\data");
            timer1.Start();           
        }

        private void ReadConfig()
        {
            borderWidth = Convert.ToInt32(ConfigClass.GetValue("borderWidth"));
            borderColor = Color.FromArgb(Convert.ToInt32(ConfigClass.GetValue("borderColor")));
            myFontFamily = new FontFamily(ConfigClass.GetValue("font"));
            sign = ConfigClass.GetValue("sign");
        }

        private void button1_Click(object sender, EventArgs e)
        {           
            AddText();
            Merge();
            Form2 f2 = new Form2(final);
            f2.ShowDialog();
        }

        void ResizePicBox()
        {
            int newHeight = pictureBox1.Width * picHeight / picWidth;
            pictureBox1.Location = new Point(pictureBox1.Location.X, pictureBox1.Location.Y + (pictureBox1.Height - newHeight) / 2);
            pictureBox2.Location = new Point(pictureBox2.Location.X, pictureBox2.Location.Y + (pictureBox2.Height - newHeight) / 2);
            pictureBox3.Location = new Point(pictureBox3.Location.X, pictureBox3.Location.Y + (pictureBox3.Height - newHeight) / 2);
            pictureBox4.Location = new Point(pictureBox4.Location.X, pictureBox4.Location.Y + (pictureBox4.Height - newHeight) / 2);
            pictureBox1.Height = newHeight;
            pictureBox2.Height = newHeight;
            pictureBox3.Height = newHeight;
            pictureBox4.Height = newHeight;            
        }

        void ReadImg()
        {
            DirectoryInfo folder = new DirectoryInfo(filePath);
            if (folder.GetFiles("*.jpg").Length < 4)
            {
                MessageBox.Show("文件夹中图片数量少于4张！");
                return;
            }
            fileNames.Clear();
            foreach (FileInfo file in folder.GetFiles("*.jpg"))
            {
                fileNames.Add(file.FullName);               
            }
            origin1 = Image.FromFile(fileNames[0]);
            origin2 = Image.FromFile(fileNames[1]);
            origin3 = Image.FromFile(fileNames[2]);
            origin4 = Image.FromFile(fileNames[3]);
            picWidth = origin1.Width;
            picHeight = origin1.Height;
            if (origin2.Width != picWidth || origin2.Height != picHeight)
            {
                origin2 = ResizeImg(origin1, origin2);
            }
            if (origin3.Width != picWidth || origin3.Height != picHeight)
            {
                origin3 = ResizeImg(origin1, origin3);
            }
            if (origin4.Width != picWidth || origin4.Height != picHeight)
            {
                origin4 = ResizeImg(origin1, origin4);
            }
            ResizePicBox();
            pictureBox1.Image = origin1;
            pictureBox2.Image = origin2;
            pictureBox3.Image = origin3;
            pictureBox4.Image = origin4;
            comic1 = new Comic(origin1);
            comic2 = new Comic(origin2);
            comic3 = new Comic(origin3);
            comic4 = new Comic(origin4);        
        }

        Image ResizeImg(Image destImg, Image currentImg)
        {
            Image result = new Bitmap(destImg.Width, destImg.Height);
            Graphics g = Graphics.FromImage(result);
            g.DrawImage(currentImg, new Rectangle(0, 0, destImg.Width, destImg.Height), new Rectangle(0, 0, currentImg.Width, currentImg.Height), GraphicsUnit.Pixel);
            g.Dispose();
            return (Image)result;
        }      

        void AddText()
        {
            pictureBox1.Image.Dispose();
            pictureBox2.Image.Dispose();
            pictureBox3.Image.Dispose();
            pictureBox4.Image.Dispose();
            pictureBox1.Image = comic1.DrawImage(bubbleImg);
            pictureBox2.Image = comic2.DrawImage(bubbleImg);
            pictureBox3.Image = comic3.DrawImage(bubbleImg);
            pictureBox4.Image = comic4.DrawImage(bubbleImg);
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            if (comic1.numOfBubbles > 0)
            {
                comic1.DrawStringWrap(g, textBox1.Text, 0);
                if (comic1.numOfBubbles > 1)
                    comic1.DrawStringWrap(g, textBox2.Text, 1);
            }
            g = Graphics.FromImage(pictureBox2.Image);
            if (comic2.numOfBubbles > 0)
            {
                comic2.DrawStringWrap(g, textBox3.Text, 0);
                if (comic2.numOfBubbles > 1)
                    comic2.DrawStringWrap(g, textBox4.Text, 1);
            }
            g = Graphics.FromImage(pictureBox3.Image);
            if (comic3.numOfBubbles > 0)
            {
                comic3.DrawStringWrap(g, textBox5.Text, 0);
                if (comic3.numOfBubbles > 1)
                    comic3.DrawStringWrap(g, textBox6.Text, 1);
            }
            g = Graphics.FromImage(pictureBox4.Image);
            if (comic4.numOfBubbles > 0)
            {
                comic4.DrawStringWrap(g, textBox7.Text, 0);
                if (comic4.numOfBubbles > 1)
                    comic4.DrawStringWrap(g, textBox8.Text, 1);
            }
            g.Dispose();
        }

        void Merge()
        {
            final = new Bitmap(picWidth * 2 + 3 * borderWidth, picHeight * 2 + 3 * borderWidth);
            Graphics g = Graphics.FromImage(final);
            g.Clear(borderColor);
            g.DrawImage(pictureBox1.Image, borderWidth, borderWidth);
            g.DrawImage(pictureBox2.Image, picWidth + 2 * borderWidth, borderWidth);
            g.DrawImage(pictureBox3.Image, borderWidth, picHeight + 2 * borderWidth);
            g.DrawImage(pictureBox4.Image, picWidth + 2 * borderWidth, picHeight + 2 * borderWidth);
            int size = picHeight / 20;
            SolidBrush sb = new SolidBrush(Color.White);
            g.DrawString("①", new Font(myFontFamily, size), sb, 40, picHeight - 80);
            g.DrawString("②", new Font(myFontFamily, size), sb, 50 + picWidth, picHeight - 80);
            g.DrawString("③", new Font(myFontFamily, size), sb, 40, 2 * picHeight - 70);
            g.DrawString("④", new Font(myFontFamily, size), sb, 50 + picWidth, 2 * picHeight - 70);
            sb.Dispose();
            DrawSign(g);
            g.Dispose();
        }

        void DrawSign(Graphics g)
        {
            int signSize = picHeight / 20;
            Font signFont = new Font(myFontFamily, signSize);
            int signWidth = picWidth * 2 / 3;
            Rectangle signRectangle = new Rectangle(picWidth + picWidth / 3, picHeight * 2 - 50, signWidth, picHeight / 12);
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Far;
            drawFormat.LineAlignment = StringAlignment.Center;
            while (g.MeasureString(sign, signFont).Width > signWidth)
            {
                signSize -= 2;
                signFont = new Font(myFontFamily, signSize);
            }
            g.DrawString(sign, signFont, new SolidBrush(Color.White), signRectangle, drawFormat);
        }

        void Text1ChangedAction(TextBox textBox1, TextBox textBox2, PictureBox picBox, Comic comic)
        {
            picBox.Image.Dispose();
            picBox.Image = comic.DrawImage(bubbleImg);
            Graphics g = Graphics.FromImage(picBox.Image);
            if (comic.numOfBubbles > 0)
            {
                comic.DrawStringWrap(g, textBox1.Text, 0);
                if (comic.numOfBubbles > 1)
                    comic.DrawStringWrap(g, textBox2.Text, 1);
            }
        }
        void Text2ChangedAction(TextBox textBox1, TextBox textBox2, PictureBox picBox, Comic comic)
        {
            picBox.Image.Dispose();
            picBox.Image = comic.DrawImage(bubbleImg);
            Graphics g = Graphics.FromImage(picBox.Image);
            comic.DrawStringWrap(g, textBox1.Text, 0);
            comic.DrawStringWrap(g, textBox2.Text, 1);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Text1ChangedAction(textBox1, textBox2, pictureBox1, comic1);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Text2ChangedAction(textBox1, textBox2, pictureBox1, comic1);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Text1ChangedAction(textBox3, textBox4, pictureBox2, comic2);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Text2ChangedAction(textBox3, textBox4, pictureBox2, comic2);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            Text1ChangedAction(textBox5, textBox6, pictureBox3, comic3);
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            Text2ChangedAction(textBox5, textBox6, pictureBox3, comic3);
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            Text1ChangedAction(textBox7, textBox8, pictureBox4, comic4);
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            Text2ChangedAction(textBox7, textBox8, pictureBox4, comic4);
        }

        #region 选择气泡
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            addMode = AddBubble.addBubble1;
            bubbleWidth = bubbleImg[0].Width;
            bubbleHeight = bubbleImg[0].Height;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            addMode = AddBubble.addBubble2;
            bubbleWidth = bubbleImg[1].Width;
            bubbleHeight = bubbleImg[1].Height;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            addMode = AddBubble.addBubble3;
            bubbleWidth = bubbleImg[2].Width;
            bubbleHeight = bubbleImg[2].Height;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            addMode = AddBubble.addBubble4;
            bubbleWidth = bubbleImg[3].Width;
            bubbleHeight = bubbleImg[3].Height;
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            addMode = AddBubble.addBubble5;
            bubbleWidth = bubbleImg[4].Width;
            bubbleHeight = bubbleImg[4].Height;
        }

        #endregion

        #region 四幅图像的鼠标响应事件
        void MouseMoveAction(PictureBox picBox, Comic comic, MouseEventArgs e)
        {
            int point_X = e.X * picWidth / pictureBox1.Width;
            int point_Y = e.Y * picHeight / pictureBox1.Height;
            if (moveBubble)
            {
                picBox.Image.Dispose();
                picBox.Image = comic.UpdateImage(bubbleImg, movingBubble.bubbleType, point_X, point_Y, movingBubble.width, movingBubble.height);
            }
            else
            {
                if (addMode != AddBubble.none)
                {
                    picBox.Image.Dispose();
                    picBox.Image = comic.UpdateImage(bubbleImg, (int)addMode - 1, point_X, point_Y, bubbleWidth, bubbleHeight);
                }
            }
        }

        void MouseDownAction(PictureBox picBox, Comic comic, MouseEventArgs e, TextBox tBox1, TextBox tBox2)
        {
            if (e.Button == MouseButtons.Right)
                return;
            int point_X = e.X * picWidth / picBox.Width;
            int point_Y = e.Y * picHeight / picBox.Height;
            int bubbleIndex = comic.FindIndex(point_X, point_Y);
            if (bubbleIndex != -1)
            {
                moveBubble = true;
                movingBubble = comic.DeleteBubble(bubbleIndex);
                EnableTextBox(comic.numOfBubbles, tBox1, tBox2);
            }
        }

        void MouseUpAction(PictureBox picBox, Comic comic, MouseEventArgs e, TextBox tBox1, TextBox tBox2)
        {
            if (e.Button == MouseButtons.Right)
            {
                addMode = AddBubble.none;
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (e.X < 5 || e.X > picBox.Width || e.Y < 5 || e.Y > picBox.Height - 5)
                    return;
                int point_X = e.X * picWidth / picBox.Width;
                int point_Y = e.Y * picHeight / picBox.Height;
                if (moveBubble)
                {
                    comic.AddBubble(point_X, point_Y, movingBubble.width, movingBubble.height, movingBubble.bubbleType);
                    EnableTextBox(comic.numOfBubbles, tBox1, tBox2);
                    picBox.Image.Dispose();
                    picBox.Image = comic.DrawImage(bubbleImg);
                    Text1ChangedAction(tBox1, tBox2, picBox, comic);
                    moveBubble = false;
                    addMode = AddBubble.none;
                    return;
                }
                else
                {
                    if (addMode != AddBubble.none)
                    {
                        comic.AddBubble(point_X, point_Y, bubbleWidth, bubbleHeight, (int)addMode - 1);
                        EnableTextBox(comic.numOfBubbles, tBox1, tBox2);
                    }                   
                }
            }
            picBox.Image.Dispose();
            picBox.Image = comic.DrawImage(bubbleImg);
        }       

        void MouseWheelAction(PictureBox picBox, Comic comic, MouseEventArgs e)
        {
            int point_X = e.X * picWidth / picBox.Width;
            int point_Y = e.Y * picHeight / picBox.Height;
            int bubbleIndex = comic.FindIndex(point_X, point_Y);
            if (bubbleIndex != -1)
            {
                int zoom = 20;
                if (e.Delta > 0)
                {
                    zoom++;
                }
                else if (e.Delta < 0)
                {
                    zoom--;
                }
                comic.ChangeBubbleSize(bubbleIndex, (float)zoom / 20);
            }
            picBox.Image.Dispose();
            picBox.Image = comic.DrawImage(bubbleImg);
        }

        void MouseLeaveAction(PictureBox picBox, Comic comic)
        {
            if (addMode == AddBubble.none)
                return;
            picBox.Image = comic.DrawImage(bubbleImg);
            if (moveBubble)
            {
                moveBubble = false;
                addMode = AddBubble.none;
            }
        }

        void EnableTextBox(int num, TextBox tBox1, TextBox tBox2)
        {
            switch (num)
            {
                case 0:
                    //tBox1.Text = "";
                    tBox1.Enabled = false;
                    break;
                case 1:
                    tBox1.Enabled = true;
                    //tBox2.Text = "";
                    tBox2.Enabled = false;
                    break;
                case 2:
                    tBox2.Enabled = true;
                    break;
            }
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveAction(pictureBox1, comic1, e);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownAction(pictureBox1, comic1, e, textBox1, textBox2);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            MouseUpAction(pictureBox1, comic1, e, textBox1, textBox2);
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            MouseWheelAction(pictureBox1, comic1, e);
            textBox1_TextChanged(sender, e);
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            MouseLeaveAction(pictureBox1, comic1);
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.Focus();
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownAction(pictureBox2, comic2, e, textBox3, textBox4);
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            MouseUpAction(pictureBox2, comic2, e, textBox3, textBox4);
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveAction(pictureBox2, comic2, e);
        }

        private void pictureBox2_MouseWheel(object sender, MouseEventArgs e)
        {
            MouseWheelAction(pictureBox2, comic2, e);
            textBox3_TextChanged(sender, e);
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            MouseLeaveAction(pictureBox2, comic2);
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            pictureBox3.Focus();
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownAction(pictureBox3, comic3, e, textBox5, textBox6);
        }

        private void pictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            MouseUpAction(pictureBox3, comic3, e, textBox5, textBox6);
        }

        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveAction(pictureBox3, comic3, e);
        }

        private void pictureBox3_MouseWheel(object sender, MouseEventArgs e)
        {
            MouseWheelAction(pictureBox3, comic3, e);
            textBox5_TextChanged(sender, e);
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            MouseLeaveAction(pictureBox3, comic3);
        }

        private void pictureBox4_MouseEnter(object sender, EventArgs e)
        {
            pictureBox4.Focus();
        }

        private void pictureBox4_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownAction(pictureBox4, comic4, e, textBox7, textBox8);
        }

        private void pictureBox4_MouseUp(object sender, MouseEventArgs e)
        {
            MouseUpAction(pictureBox4, comic4, e, textBox7, textBox8);
        }

        private void pictureBox4_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveAction(pictureBox4, comic4, e);
        }

        private void pictureBox4_MouseWheel(object sender, MouseEventArgs e)
        {
            MouseWheelAction(pictureBox4, comic4, e);
            textBox7_TextChanged(sender, e);
        }

        private void pictureBox4_MouseLeave(object sender, EventArgs e)
        {
            MouseLeaveAction(pictureBox4, comic4);
        }
        #endregion

        #region 菜单栏部分
        private void 关于ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutBox abourBox = new AboutBox();
            abourBox.ShowDialog();
        }

        private void 打开文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = folderBrowserDialog1.SelectedPath;
                ReadImg();
            }
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void shiyoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tutorial tutorial = new Tutorial();
            tutorial.ShowDialog();
        }
        #endregion       

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ConfigClass.GetValue("ShowTutorial") == "Yes")
            {
                Tutorial tutorial = new Tutorial();
                timer1.Dispose();
                tutorial.ShowDialog();
            }
        }
        
    }
}
