using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FourPanelComicMaker
{
    struct Bubble
    {
        public int width;
        public int height;
        public int bubbleType;
    }
    class Comic
    {
        Bitmap originImg, comicImg;
        List<Rectangle> bubblePosition;
        List<Rectangle> textRegion;
        List<int> bubbleType;
        public int numOfBubbles = 0;

        public Comic(Image img)
        {
            originImg = new Bitmap(img);
            bubblePosition = new List<Rectangle>();
            textRegion = new List<Rectangle>();
            bubbleType = new List<int>();
        }

        public Rectangle GetPosition(int index)
        {
            return bubblePosition[index];
        }

        public int GetType(int index)
        {
            return bubbleType[index];
        }

        public Bitmap GetImg()
        {
            return comicImg;
        }

        public void AddBubble(int x, int y, int width, int height, int type)
        {
            if (numOfBubbles == 2)
                return;
            Rectangle bubble = new Rectangle(x - width / 2, y - height / 2, width, height);
            bubblePosition.Add(bubble);
            textRegion.Add(ZoomRectangle(Form1.textRegion[type], (float)width / Form1.bubbleImg[type].Width));
            bubbleType.Add(type);
            numOfBubbles++;
        }

        public Bubble DeleteBubble(int index)
        {
            Bubble backup = new Bubble();
            backup.width = bubblePosition[index].Width;
            backup.height = bubblePosition[index].Height;
            backup.bubbleType = bubbleType[index];
            bubblePosition.RemoveAt(index);
            textRegion.RemoveAt(index);
            bubbleType.RemoveAt(index);
            numOfBubbles--;
            return backup;
        }

        private Rectangle ZoomRectangle(Rectangle origin, float magnification)
        {
            int newX = (int)(origin.X * magnification);
            int newY = (int)(origin.Y * magnification);
            int newWidth = (int)(origin.Width * magnification);
            int newHeight = (int)(origin.Height * magnification);
            Rectangle newRec = new Rectangle(newX, newY, newWidth, newHeight);
            return newRec;
        }

        public void ChangeBubbleSize(int index, float magnification)
        {
            Rectangle temp = bubblePosition[index];
            int newWidth = (int)(temp.Width * magnification);
            int newHeight = (int)(temp.Height * magnification);
            bubblePosition[index] = new Rectangle((temp.Left + temp.Right - newWidth) / 2, (temp.Top +　temp.Bottom - newHeight) / 2, newWidth, newHeight);
            temp = textRegion[index];
            textRegion[index] = ZoomRectangle(temp, magnification);
        }
        public int FindIndex(int x, int y)
        {
            for (int i = 0; i < numOfBubbles; i++)
            {
                if (x > bubblePosition[i].Left && x < bubblePosition[i].Right && y > bubblePosition[i].Top && y < bubblePosition[i].Bottom)
                    return i;
            }
            return -1;
        }
        public Bitmap DrawImage(Bitmap[] bubbleImg)
        {
            comicImg = (Bitmap)originImg.Clone();
            Graphics g = Graphics.FromImage(comicImg);
            for (int i = 0; i < numOfBubbles; i++)
            {
                g.DrawImage(bubbleImg[bubbleType[i]], bubblePosition[i]);
            }
            g.Dispose();
            return comicImg;
        }
        public Bitmap UpdateImage(Bitmap[] bubbleImg, int type, int x, int y, int width, int height)
        {
            comicImg = (Bitmap)originImg.Clone();
            Graphics g = Graphics.FromImage(comicImg);
            for (int i = 0; i < numOfBubbles; i++)
            {
                g.DrawImage(bubbleImg[bubbleType[i]], bubblePosition[i]);
            }
            g.DrawImage(bubbleImg[type], x - width / 2, y - height / 2, width, height);
            g.Dispose();
            return comicImg;
        }

        public void DrawStringWrap(Graphics g, string text, int index)
        {
            float fontSize = 200;
            List<string> textRows;
            double rowHeight;
            int maxRowCount;
            Font myFont;
            do
            {
                myFont = new Font(Form1.myFontFamily, fontSize);
                rowHeight = Math.Ceiling(g.MeasureString("测试", myFont).Height);
                textRows = GetStringRows(g, myFont, text, textRegion[index].Width);
                maxRowCount = (int)(textRegion[index].Height / rowHeight);
                fontSize -= 2;
            } while (maxRowCount < textRows.Count);
            int drawRowCount = (maxRowCount < textRows.Count) ? maxRowCount : textRows.Count;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            for (int i = 0; i < drawRowCount; i++)
            {
                int x = bubblePosition[index].Left + textRegion[index].X;
                int y = bubblePosition[index].Top + textRegion[index].Y + (textRegion[index].Height - (int)(rowHeight * drawRowCount)) / 2;
                int width = textRegion[index].Width;
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
    }
}
