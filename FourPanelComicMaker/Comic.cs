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
        List<int> bubbleType;
        public int numOfBubbles = 0;

        public Comic(Image img)
        {
            originImg = new Bitmap(img);
            bubblePosition = new List<Rectangle>();
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
            Rectangle r = new Rectangle(x - width / 2, y - height / 2, width, height);
            bubblePosition.Add(r);
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
            bubbleType.RemoveAt(index);
            numOfBubbles--;
            return backup;
        }
        public void ChangeBubbleSize(int index, float magnification)
        {
            Rectangle temp = bubblePosition[index];
            int newWidth = (int)(temp.Width * magnification);
            int newHeight = (int)(temp.Height * magnification);
            bubblePosition[index] = new Rectangle((temp.Left + temp.Right - newWidth) / 2, (temp.Top +　temp.Bottom - newHeight) / 2, newWidth, newHeight);          
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
        
    }
}
