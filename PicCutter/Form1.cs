using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PicCutter
{   
    enum AdjustMode
    {
        none, leftEdge, rightEdge, topEdge, bottomEdge, LUVertex, LDVertex, RUVertex, RDVertex, free, move
    }

    enum AspectRatio
    {
        w1h1, w3h2, w2h3, w16h9
    }

    public partial class Form1 : Form
    {
        Image origin, picBoard;
        Bitmap backup, result;
        string imgName;
        Pen pen = new Pen(Color.White, 2);
        bool started = false;
        bool imgOpened = false;
        bool kwhLocked = false;
        AdjustMode mode = AdjustMode.none;
        Point startPoint;
        int picWidth, picHeight;
        int left, right, top, bottom;
        int bk_left, bk_right, bk_top, bk_bottom;
        float ratio;
        AspectRatio aspectRatio;
        float fixedKwh = 0;

        public Form1()
        {
            InitializeComponent();
            this.BackgroundImage = new Bitmap(Properties.Resources.resPath + "3.jpg");
            menuStrip1.BackgroundImage = new Bitmap(Properties.Resources.resPath + "menuStrip2.jpg");
            pictureBox1.Image = new Bitmap(Properties.Resources.resPath + "initial.jpg");
            groupBox2.Enabled = false;
            groupBox1.Enabled = false;
        }

        private void ShowPic()
        {
            picWidth = origin.Width;
            picHeight = origin.Height;
            label4.Text = picWidth.ToString() + "*" + picHeight.ToString();
            pictureBox1.Width = pictureBox1.Height * picWidth / picHeight;
            this.Width += pictureBox1.Width - 360;
            ratio = (float)picWidth / pictureBox1.Width;
            backup = new Bitmap(origin, pictureBox1.Width, pictureBox1.Height);            
            picBoard = (Image)backup.Clone();
            pictureBox1.Image = picBoard;
            imgOpened = true;
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!imgOpened)
                return;
            int x = e.X;
            int y = e.Y;
            startPoint = new Point(x, y);
            mode = AdjustMode.free;
            if (!started)
            {
                started = true;                
                return;
            }
            int precision = 4;
            if (Math.Abs(x - left) < precision)
            {
                if (y > top - precision && y < bottom + precision)
                {
                    if (y < top + precision)
                        mode = AdjustMode.LUVertex;
                    else if (y > bottom - precision)
                        mode = AdjustMode.LDVertex;
                    else
                        mode = AdjustMode.leftEdge;
                }
            }
            else if (x > left + precision - 1 && x < right - precision + 1)
            {
                if (Math.Abs(y - top) < precision)
                    mode = AdjustMode.topEdge;
                else if (Math.Abs(y - bottom) < precision)
                    mode = AdjustMode.bottomEdge;
                else if (y > top + precision - 1 && y < bottom - precision + 1)
                    mode = AdjustMode.move;
            }
            else if (Math.Abs(x - right) < precision)
            {
                if (y > top - precision && y < bottom + precision)
                {
                    if (y < top + precision)
                        mode = AdjustMode.RUVertex;
                    else if (y > bottom - precision)
                        mode = AdjustMode.RDVertex;
                    else
                        mode = AdjustMode.rightEdge;
                }
            }            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!imgOpened)
                return;
            int x = e.X;
            int y = e.Y;
            if (x < 0)
                x = 0;
            else if (x > pictureBox1.Width)
                x = pictureBox1.Width;
            if (y < 0)
                y = 0;
            else if (y > pictureBox1.Height)
                y = pictureBox1.Height;
            CursorShape(x, y);
            if (mode != AdjustMode.none)
                pictureBox1.Image = picBoard;
            //float kwh = (float)(right - left) / (bottom - top);
            switch(mode)
            {
                case AdjustMode.none:
                    break;
                case AdjustMode.free:
                    if (kwhLocked)
                    {
                        int tempY;
                        if(y < startPoint.Y)
                            tempY = startPoint.Y - (int)(Math.Abs(x - startPoint.X) / fixedKwh);
                        else
                            tempY = startPoint.Y + (int)(Math.Abs(x - startPoint.X) / fixedKwh);
                        if (tempY > pictureBox1.Height)
                        {
                            tempY = pictureBox1.Height;
                            x = startPoint.X + (int)((tempY - startPoint.Y) * fixedKwh);
                        }
                        else if (tempY < 0)
                        {
                            tempY = 0;
                            x = startPoint.X + (int)((tempY - startPoint.Y) * fixedKwh);
                        }
                        DrawRegion(startPoint, new Point(x, tempY));
                    }
                    else
                        DrawRegion(startPoint, RevisePosi(e.Location));
                    break;
                case AdjustMode.move:
                    int shiftX = x - startPoint.X;
                    int shiftY = y - startPoint.Y;
                    if (left + shiftX < 0)
                        shiftX = 0 - left;
                    else if (right + shiftX > pictureBox1.Width)
                        shiftX = pictureBox1.Width - right;
                    if (top + shiftY < 0)
                        shiftY = 0 - top;
                    else if (bottom + shiftY > pictureBox1.Height)
                        shiftY = pictureBox1.Height - bottom;
                    DrawRegion(RevisePosi(new Point(left + shiftX, top + shiftY)), RevisePosi(new Point(right + shiftX, bottom + shiftY)));
                    startPoint = new Point(x, y);
                    break;
                case AdjustMode.leftEdge:
                    x = Math.Min(x, right - 5);
                    if (kwhLocked)
                        top = bk_top + (int)((x - bk_left) / fixedKwh);
                    DrawRegion(RevisePosi(new Point(x, top)), new Point(right, bottom));
                    break;
                case AdjustMode.rightEdge:
                    x = Math.Max(x, left + 5);
                    if (kwhLocked)
                        bottom = bk_bottom - (int)((bk_right - x) / fixedKwh);
                    DrawRegion(RevisePosi(new Point(x, top)), new Point(left, bottom));
                    break;
                case AdjustMode.topEdge:
                    y = Math.Min(y, bottom - 5);
                    if (kwhLocked)
                        right = bk_right - (int)((y - bk_top) * fixedKwh);
                    DrawRegion(RevisePosi(new Point(left, y)), new Point(right, bottom));
                    break;
                case AdjustMode.bottomEdge:
                    y = Math.Max(y, top + 5);
                    if (kwhLocked)
                        left = bk_left + (int)((bk_bottom - y) * fixedKwh);
                    DrawRegion(RevisePosi(new Point(left, y)), new Point(right, top));
                    break;
                case AdjustMode.LUVertex:
                    x = Math.Min(x, right - 5);
                    if (kwhLocked)
                        y = bk_top + (int)((x - bk_left) / fixedKwh);
                    else
                        y = Math.Min(y, bottom - 5);
                    DrawRegion(RevisePosi(new Point(x, y)), new Point(right, bottom));
                    break;
                case AdjustMode.RUVertex:
                    x = Math.Max(x, left + 5);
                    if (kwhLocked)
                        y = bk_top + (int)((bk_right - x) / fixedKwh);
                    else
                        y = Math.Min(y, bottom - 5);
                    DrawRegion(RevisePosi(new Point(x, y)), new Point(left, bottom));
                    break;
                case AdjustMode.LDVertex:
                    x = Math.Min(x, right - 5);
                    if (kwhLocked)
                        y = bk_bottom - (int)((x - bk_left) / fixedKwh);
                    else
                        y = Math.Max(y, top + 5);
                    DrawRegion(RevisePosi(new Point(x, y)), new Point(right, top));
                    break;
                case AdjustMode.RDVertex:
                    x = Math.Max(x, left + 5);
                    if (kwhLocked)
                        y = bk_bottom - (int)((bk_right - x) / fixedKwh);
                    else
                        y = Math.Max(y, top + 5);
                    DrawRegion(RevisePosi(new Point(x, y)), new Point(left, top));
                    break;
            }

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!imgOpened)
                return;
            DrawRegion(new Point(right, bottom), new Point(left, top));
            mode = AdjustMode.none;
            BackupLRTB();
        }

        private void BackupLRTB()
        {
            bk_left = left;
            bk_right = right;
            bk_top = top;
            bk_bottom = bottom;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] filename = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            origin = new Bitmap(filename[0]);
            ShowPic();
        }

        private Point RevisePosi(Point p)
        {
            int x = p.X;
            int y = p.Y;
            int maxX = pictureBox1.Width;
            int maxY = pictureBox1.Height;
            if (x > 0)
                x = Math.Min(x, maxX);
            else
                x = 0;
            if (y > 0)
                y = Math.Min(y, maxY);
            else
                y = 0;
            return new Point(x, y);
        }

        private void DrawRec()
        {
            picBoard = (Image)backup.Clone();
            Graphics g = Graphics.FromImage(picBoard);
            int radius = 4;
            int diameter = 8;
            g.DrawRectangle(pen, left, top, right - left, bottom - top);
            g.DrawEllipse(pen, left - radius, top - radius, diameter, diameter);
            g.DrawEllipse(pen, right - radius, top - radius, diameter, diameter);
            g.DrawEllipse(pen, left - radius, bottom - radius, diameter, diameter);
            g.DrawEllipse(pen, right - radius, bottom - radius, diameter, diameter);
            g.Dispose();
        }

        private void DrawRegion(Point p1, Point p2)
        {            
            int x = (p1.X < p2.X) ? p1.X : p2.X;
            int y = (p1.Y < p2.Y) ? p1.Y : p2.Y;
            int width = Math.Abs(p1.X - p2.X);
            int height = Math.Abs(p1.Y - p2.Y);            
            left = x;
            right = x + width;
            top = y;
            bottom = y + height;
            pictureBox1.Image.Dispose();
            DrawRec();
            pictureBox1.Image = picBoard;
            textBox1.Text = ((int)(width * ratio)).ToString();
            textBox2.Text = ((int)(height * ratio)).ToString();
        }

        private void CursorShape(int x, int y)
        {           
            this.Cursor = Cursors.Arrow;
            int precision = 4;
            if (Math.Abs(x - left) < precision)
            {
                if (y > top - precision && y < bottom + precision)
                {
                    if (y < top + precision)
                        this.Cursor = Cursors.SizeNWSE;
                    else if (y > bottom - precision)
                        this.Cursor = Cursors.SizeNESW;
                    else
                        this.Cursor = Cursors.SizeWE;
                }
            }
            else if (x > left + precision - 1 && x < right - precision + 1)
            {
                if (Math.Abs(y - top) < precision)
                    this.Cursor = Cursors.SizeNS;
                else if (Math.Abs(y - bottom) < precision)
                    this.Cursor = Cursors.SizeNS;
                else if (y > top + precision - 1 && y < bottom - precision + 1)
                    this.Cursor = Cursors.Cross;
            }
            else if (Math.Abs(x - right) < precision)
            {
                if (y > top - precision && y < bottom + precision)
                {
                    if (y < top + precision)
                        this.Cursor = Cursors.SizeNESW;
                    else if (y > bottom - precision)
                        this.Cursor = Cursors.SizeNWSE;
                    else
                        this.Cursor = Cursors.SizeWE;
                }
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            打开ToolStripMenuItem_Click(sender, e);
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "图像文件|*.jpg;*.bmp;*.png";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imgName = openFileDialog1.SafeFileName;
                origin = new Bitmap(openFileDialog1.FileName);
                ShowPic();
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!imgOpened)
            {
                MessageBox.Show("请先打开一张图片！");
                return;
            }
            else if (!started)
            {
                MessageBox.Show("请先选择截取区域！");
                return;
            }
            saveFileDialog1.FileName = imgName.Split('.')[0] + "_副本." + imgName.Split('.')[1];
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CutImg();
                result.Save(saveFileDialog1.FileName);
            }
        }

        private void CutImg()
        {
            int x = left * picWidth / pictureBox1.Width;
            int y = top * picWidth / pictureBox1.Width;
            int width = (int)((right - left) * ratio);
            int height = (int)((bottom - top) * ratio);
            result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            Graphics g = Graphics.FromImage(result);           
            g.DrawImage(origin, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers.CompareTo(Keys.Control) == 0)
            {
                if (e.KeyCode == Keys.S)
                    保存ToolStripMenuItem_Click(sender, e);
                else if (e.KeyCode == Keys.O)
                    打开ToolStripMenuItem_Click(sender, e);
            }                            
        }

        private void SetAccuRegion()
        {
            float width = Convert.ToInt32(textBox1.Text) / ratio;
            float height = Convert.ToInt32(textBox2.Text) / ratio;
            width = Math.Min(width, pictureBox1.Width);
            height = Math.Min(height, pictureBox1.Height);
            left = (int)(pictureBox1.Width - width) / 2;
            top = (int)(pictureBox1.Height - height) / 2;
            right = left + (int)width;
            bottom = top + (int)height;
            DrawRegion(new Point(left, top), new Point(right, bottom));
            textBox1.Text = ((int)(width * ratio)).ToString();
            textBox2.Text = ((int)(height * ratio)).ToString();
            if (!started)
                started = true;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!imgOpened)
                return;
            if (e.KeyCode == Keys.Enter)
            {
                if (kwhLocked)
                {
                    int height = (int)(Convert.ToInt32(textBox1.Text) / fixedKwh);
                    if (height > picHeight)
                    {
                        height = picHeight;
                        textBox1.Text = ((int)(height * fixedKwh)).ToString();                        
                    }
                    textBox2.Text = height.ToString();
                }
                SetAccuRegion();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (!imgOpened)
                return;
            if (e.KeyCode == Keys.Enter)
            {
                if (kwhLocked)
                {
                    int width = (int)(Convert.ToInt32(textBox2.Text) * fixedKwh);
                    if (width > picWidth)
                    {
                        width = picWidth;
                        textBox2.Text = ((int)(width / fixedKwh)).ToString();
                    }
                    textBox1.Text = width.ToString();
                }
                SetAccuRegion();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                kwhLocked = true;
                fixedKwh = (float)(right - left) / (bottom - top);
                BackupLRTB();
            }
            else
                kwhLocked = false;
        }

        private void DrawSizeText()
        {
            Font yahei = new Font("微软雅黑", 10, FontStyle.Bold);
            Graphics g = panel1.CreateGraphics();
            g.DrawString("1:1", yahei, Brushes.White, 0, 2);
            g = panel4.CreateGraphics();
            g.DrawString("3:2", yahei, Brushes.White, 3, 2);
            g = panel6.CreateGraphics();
            g.DrawString("2:3", new Font("微软雅黑", 8, FontStyle.Bold), Brushes.White, 0, 4);
            g = panel8.CreateGraphics();
            g.DrawString("16:9", yahei, Brushes.White, 0, 0);                
            g.Dispose();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawSizeText();
        }

        private void ChangePanelColor(Panel panel)
        {
            switch (aspectRatio)
            {
                case AspectRatio.w1h1:
                    //panel9.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel10.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel11.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel12.BackColor = Color.FromKnownColor(KnownColor.Control);
                    break;
                case AspectRatio.w3h2:
                    panel9.BackColor = Color.FromKnownColor(KnownColor.Control);
                    //panel10.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel11.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel12.BackColor = Color.FromKnownColor(KnownColor.Control);
                    break;
                case AspectRatio.w2h3:
                    panel9.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel10.BackColor = Color.FromKnownColor(KnownColor.Control);
                    //panel11.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel12.BackColor = Color.FromKnownColor(KnownColor.Control);
                    break;
                case AspectRatio.w16h9:
                    panel9.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel10.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel11.BackColor = Color.FromKnownColor(KnownColor.Control);
                    //panel12.BackColor = Color.FromKnownColor(KnownColor.Control);
                    break;
                default:
                    panel9.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel10.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel11.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel12.BackColor = Color.FromKnownColor(KnownColor.Control);
                    break;
            }
            panel.BackColor = Color.LightGray;
        }

        private void panel9_MouseEnter(object sender, EventArgs e)
        {
            if (fixedKwh != 1)
                ChangePanelColor(panel9);
        }

        private void panel10_MouseEnter(object sender, EventArgs e)
        {
            if (fixedKwh != 2)
                ChangePanelColor(panel10);
        }

        private void panel11_MouseEnter(object sender, EventArgs e)
        {
            if (fixedKwh != 3)
                ChangePanelColor(panel11);
        }

        private void panel12_MouseEnter(object sender, EventArgs e)
        {
            if (fixedKwh != 3)
                ChangePanelColor(panel12);
        }

        private void DrawFixedKwhRegion(float kwh)
        {
            float height = 800;
            while (height > picHeight)
            {
                height -= 100;
            }
            float width = height * kwh;
            left = (pictureBox1.Width - (int)(width / ratio)) / 2;
            right = left + (int)(width / ratio);
            top = (pictureBox1.Height - (int)(height / ratio)) / 2;
            bottom = top + (int)(height / ratio);
            BackupLRTB();
            pictureBox1.Image.Dispose();
            DrawRec();
            pictureBox1.Image = picBoard;
            textBox1.Text = ((int)((right - left) * ratio)).ToString();
            textBox2.Text = ((int)((bottom - top) * ratio)).ToString();
            if (!started)
                started = true;
        }

        private void panel9_Click(object sender, EventArgs e)
        {
            aspectRatio = AspectRatio.w1h1;
            fixedKwh = 1f;            
            panel9.BackColor = Color.DimGray;
            panel10.BackColor = Color.FromKnownColor(KnownColor.Control);
            panel11.BackColor = Color.FromKnownColor(KnownColor.Control);
            panel12.BackColor = Color.FromKnownColor(KnownColor.Control);            
            DrawFixedKwhRegion(fixedKwh);
            checkBox1.Checked = true;
        }

        private void panel10_Click(object sender, EventArgs e)
        {
            aspectRatio = AspectRatio.w3h2;
            fixedKwh = 1.5f;
            panel10.BackColor = Color.DimGray;
            panel9.BackColor = Color.FromKnownColor(KnownColor.Control);
            panel11.BackColor = Color.FromKnownColor(KnownColor.Control);
            panel12.BackColor = Color.FromKnownColor(KnownColor.Control);           
            DrawFixedKwhRegion(fixedKwh);
            checkBox1.Checked = true;
        }

        private void panel11_Click(object sender, EventArgs e)
        {
            aspectRatio = AspectRatio.w2h3;
            fixedKwh = 2 / 3f;
            panel11.BackColor = Color.DimGray;
            panel9.BackColor = Color.FromKnownColor(KnownColor.Control);
            panel10.BackColor = Color.FromKnownColor(KnownColor.Control);
            panel12.BackColor = Color.FromKnownColor(KnownColor.Control);            
            DrawFixedKwhRegion(fixedKwh);
            checkBox1.Checked = true;
        }

        private void panel12_Click(object sender, EventArgs e)
        {
            aspectRatio = AspectRatio.w16h9;
            fixedKwh = 16 / 9f;
            panel12.BackColor = Color.DimGray;
            panel9.BackColor = Color.FromKnownColor(KnownColor.Control);
            panel10.BackColor = Color.FromKnownColor(KnownColor.Control);
            panel11.BackColor = Color.FromKnownColor(KnownColor.Control);            
            DrawFixedKwhRegion(fixedKwh);            
            checkBox1.Checked = true;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            panel9_Click(sender, e);
        }

        private void label8_Click(object sender, EventArgs e)
        {
            panel10_Click(sender, e);
        }

        private void label9_Click(object sender, EventArgs e)
        {
            panel11_Click(sender, e);
        }

        private void label10_Click(object sender, EventArgs e)
        {
            panel12_Click(sender, e);
        }

        

    }
}
