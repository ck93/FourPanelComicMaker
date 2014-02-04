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

        public Settings()
        {
            InitializeComponent();
            numericUpDown1.Value = Form1.borderWidth;
            textBox1.Text = Form1.myFontFamily.Name;
            textBox2.Text = Form1.sign;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(fontDialog1.ShowDialog() == DialogResult.OK)
            {
                temp = fontDialog1.Font.FontFamily;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1.borderWidth = (int)numericUpDown1.Value;
            Form1.myFontFamily = temp;
            Form1.sign = textBox2.Text;
            this.Close();
        }
    }
}
