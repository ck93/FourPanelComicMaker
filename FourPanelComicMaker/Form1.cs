﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace FourPanelComicMaker
{
    enum AddBubble
    {
        none, addBubble1, addBubble2, addBubble3, addBubble4
    }
    public partial class Form1 : Form
    {
        int picWidth = 500;
        int picHeight = 666;
        int bubbleWidth = 210;
        int bubbleHeight = 180;
        static public int borderWidth = 10;
        static public Color borderColor = Color.White;
        static public FontFamily myFontFamily = new FontFamily("微软雅黑");
        static public string sign = "Designed by";
        Comic comic1, comic2, comic3, comic4;
        Bitmap[] bubbleImg;
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
            bubbleImg = new Bitmap[4];
            bubbleImg[0] = new Bitmap(Properties.Resources.bubble1);
            bubbleImg[1] = new Bitmap(Properties.Resources.bubble2);
            bubbleImg[2] = new Bitmap(Properties.Resources.bubble3);
            bubbleImg[3] = new Bitmap(Properties.Resources.bubble4);
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
                //sameSize[0] = true;
                origin2 = ResizeImg(origin1, origin2);
            }
            if (origin3.Width != picWidth || origin3.Height != picHeight)
            {
                //sameSize[1] = true;
                origin3 = ResizeImg(origin1, origin3);
            }
            if (origin4.Width != picWidth || origin4.Height != picHeight)
            {
                //sameSize[2] = true;
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

        private void DrawStringWrap(Graphics g, string text, Rectangle recangle, int bubbleType)
        {
            float fontSize = 80;
            List<string> textRows;
            double rowHeight;
            int maxRowCount;
            Font myFont;
            do
            {
                myFont = new Font(myFontFamily, fontSize);
                textRows = GetStringRows(g, myFont, text, recangle.Width * 2 / 3);
                rowHeight = Math.Ceiling(g.MeasureString("测试", myFont).Height);
                switch (bubbleType)
                {
                    case 0:
                        maxRowCount = (int)(recangle.Height * 5 / (rowHeight * 8));
                        break;
                    case 1:
                        maxRowCount = (int)(recangle.Height * 3 / (rowHeight * 4));
                        break;
                    case 2:
                        maxRowCount = (int)(recangle.Height * 3 / (rowHeight * 4));
                        break;
                    case 3:
                        maxRowCount = (int)(recangle.Height * 3 / (rowHeight * 4));
                        break;
                    default:
                        maxRowCount = 1;
                        break;
                }                
                fontSize -= 2;
            } while (maxRowCount < textRows.Count);
            int drawRowCount = (maxRowCount < textRows.Count) ? maxRowCount : textRows.Count;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            for (int i = 0; i < drawRowCount; i++)
            {
                int x, y, width;
                switch (bubbleType)
                {
                    case 0:
                        x = recangle.Left + recangle.Width / 6;
                        y = recangle.Top + recangle.Height / 4;
                        width = recangle.Width * 3 / 4;
                        break;
                    case 1:
                        x = recangle.Left + recangle.Width / 6;
                        y = recangle.Top + recangle.Height / 4;
                        width = recangle.Width * 3 / 4;
                        break;
                    case 2:
                        x = recangle.Left + recangle.Width / 7;
                        y = recangle.Top + recangle.Height / 5;
                        width = recangle.Width * 3 / 4;
                        break;
                    case 3:
                        x = recangle.Left + recangle.Width / 9;
                        y = recangle.Top + recangle.Height / 6;
                        width = recangle.Width * 3 / 4;
                        break;
                    default:
                        x = 0;
                        y = 0;
                        width = 0;
                        break;
                }
                Rectangle fontRectanle = new Rectangle(x, y + (int)(rowHeight * i), width, (int)rowHeight);
                g.DrawString(textRows[i], myFont, new SolidBrush(Color.Black), fontRectanle, sf);
            }
        }

        private List<string> GetStringRows(Graphics graphic, Font font, string text, int width)
        {
            int RowBeginIndex = 0;
            int rowEndIndex = 0;
            int textLength = text.Length;
            List<string> textRows = new List<string>();
            for (int index = 0; index < textLength; index++)
            {
                rowEndIndex = index;
                if (index == textLength - 1)
                {
                    if (graphic.MeasureString(text.Substring(RowBeginIndex), font).Width <= width)
                        textRows.Add(text.Substring(RowBeginIndex));
                    else
                    {
                        textRows.Add(text.Substring(RowBeginIndex, rowEndIndex - RowBeginIndex));
                        textRows.Add(text.Substring(rowEndIndex));
                    }
                }
                else if (rowEndIndex + 1 < text.Length && text.Substring(rowEndIndex, 2) == "\r\n")
                {
                    textRows.Add(text.Substring(RowBeginIndex, rowEndIndex - RowBeginIndex));
                    rowEndIndex = index += 2;
                    RowBeginIndex = rowEndIndex;
                }
                else if (graphic.MeasureString(text.Substring(RowBeginIndex, rowEndIndex - RowBeginIndex + 1), font).Width > width)
                {
                    textRows.Add(text.Substring(RowBeginIndex, rowEndIndex - RowBeginIndex));
                    RowBeginIndex = rowEndIndex;
                }
            }
            return textRows;
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
                DrawStringWrap(g, textBox1.Text, comic1.GetPosition(0), comic1.GetType(0));
                if (comic1.numOfBubbles > 1)
                    DrawStringWrap(g, textBox2.Text, comic1.GetPosition(1), comic1.GetType(1));
            }
            g = Graphics.FromImage(pictureBox2.Image);
            if (comic2.numOfBubbles > 0)
            {
                DrawStringWrap(g, textBox3.Text, comic2.GetPosition(0), comic2.GetType(0));
                if (comic2.numOfBubbles > 1)
                    DrawStringWrap(g, textBox4.Text, comic2.GetPosition(1), comic2.GetType(1));
            }
            g = Graphics.FromImage(pictureBox3.Image);
            if (comic3.numOfBubbles > 0)
            {
                DrawStringWrap(g, textBox5.Text, comic3.GetPosition(0), comic3.GetType(0));
                if (comic3.numOfBubbles > 1)
                    DrawStringWrap(g, textBox6.Text, comic3.GetPosition(1), comic3.GetType(1));
            }
            g = Graphics.FromImage(pictureBox4.Image);
            if (comic4.numOfBubbles > 0)
            {
                DrawStringWrap(g, textBox7.Text, comic4.GetPosition(0), comic4.GetType(0));
                if (comic4.numOfBubbles > 1)
                    DrawStringWrap(g, textBox8.Text, comic4.GetPosition(1), comic4.GetType(1));
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
                DrawStringWrap(g, textBox1.Text, comic.GetPosition(0), comic.GetType(0));
                if (comic.numOfBubbles > 1)
                    DrawStringWrap(g, textBox2.Text, comic.GetPosition(1), comic.GetType(1));
            }
        }
        void Text2ChangedAction(TextBox textBox1, TextBox textBox2, PictureBox picBox, Comic comic)
        {
            picBox.Image.Dispose();
            picBox.Image = comic.DrawImage(bubbleImg);
            Graphics g = Graphics.FromImage(picBox.Image);
            DrawStringWrap(g, textBox1.Text, comic.GetPosition(0), comic.GetType(0));
            DrawStringWrap(g, textBox2.Text, comic.GetPosition(1), comic.GetType(1));
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
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            addMode = AddBubble.addBubble2;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            addMode = AddBubble.addBubble3;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            addMode = AddBubble.addBubble4;
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
                    moveBubble = false;
                    addMode = AddBubble.none;
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

        void EnableTextBox(int num, TextBox tBox1, TextBox tBox2)
        {
            switch(num)
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
            if (!File.Exists(@".\使用说明.txt"))
            {
                MessageBox.Show("文件缺失，请自行下载使用说明！");
                return;
            }
            Process.Start(@".\使用说明.txt");
        }
        #endregion

        

        

        
    }
}
