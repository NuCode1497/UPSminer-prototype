// Copyright 2012 (C) Cody Neuburger  All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace prototype
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
            linkLabel1.Links.Add(0, 50, "http://student.cse.fau.edu/~cneuburg/EmersonPrototype/readmeDBWEB.htm");
            linkLabel2.Links.Add(0, 4, "http://student.cse.fau.edu/~cneuburg/EmersonPrototype/readme.pdf");
            linkLabel3.Links.Add(0, 50, "http://student.cse.fau.edu/~cneuburg/EmersonPrototype/readme.htm");
            linkLabel4.Links.Add(0, 4, "http://student.cse.fau.edu/~cneuburg/EmersonPrototype/readmeDB.pdf");
            toolTip1.SetToolTip(this.linkLabel1, linkLabel1.Links[0].LinkData.ToString());
            toolTip2.SetToolTip(this.linkLabel2, linkLabel2.Links[0].LinkData.ToString());
            toolTip3.SetToolTip(this.linkLabel3, linkLabel3.Links[0].LinkData.ToString());
            toolTip4.SetToolTip(this.linkLabel4, linkLabel4.Links[0].LinkData.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }
}
