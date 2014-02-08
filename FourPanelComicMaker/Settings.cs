using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourPanelComicMaker
{
    public partial class Settings : Form
    {
        FontFamily temp = Form1.myFontFamily;
        Color tempColor = Form1.borderColor;

        public Settings()
        {
            InitializeComponent();
            numericUpDown1.Value = Form1.borderWidth;
            textBox1.Text = Form1.myFontFamily.Name;
            textBox2.Text = Form1.sign;
            textBox3.Text = Form1.borderColor.Name;
            if (Form1.borderWidth == 0)
            {
                textBox3.Enabled = false;
                button3.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                temp = fontDialog1.Font.FontFamily;
                textBox2.Text = temp.Name;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1.borderWidth = (int)numericUpDown1.Value;
            Form1.borderColor = tempColor;
            Form1.myFontFamily = temp;
            Form1.sign = textBox2.Text;
            SaveConfig();
            this.Close();
        }

        private void SaveConfig()
        {
            ConfigClass.SetValue("borderWidth", numericUpDown1.Value.ToString());
            ConfigClass.SetValue("borderColor", tempColor.ToArgb().ToString());
            ConfigClass.SetValue("font", textBox1.Text);
            ConfigClass.SetValue("sign", textBox2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                tempColor = colorDialog1.Color;
                textBox3.Text = tempColor.Name;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value == 0)
            {
                textBox3.Enabled = false;
                button3.Enabled = false;
            }
            else
            {
                textBox3.Enabled = true;
                button3.Enabled = true;
            }
        }
    }
}
