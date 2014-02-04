using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace FourPanelComicMaker
{
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            linkLabel1.Links.Add(0, 31, "http://pan.baidu.com/s/1sjGsFnR");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(sInfo);
        }
    }
}
