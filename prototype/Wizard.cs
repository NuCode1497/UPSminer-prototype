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
    public partial class Wizard : Form
    {
        ConnectForm c;
        public Wizard()
        {
            InitializeComponent();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked) FromFilePanel.Enabled = true;
            else FromFilePanel.Enabled = false;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked) ConnectDBPanel.Enabled = true;
            else ConnectDBPanel.Enabled = false;
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            c = new ConnectForm();
            c.server = textBoxServer.Text;
            c.login = textBoxLogin.Text;
            c.password = textBoxPassword.Text;
            c.table = textBoxTable.Text;
            c.database = textBoxDatabase.Text;
            TestConnectionLabel.Text = c.TestConnection();
            Cursor = Cursors.Default;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            TestButton_Click(sender, e);
            Global.form1.dataViewer.SetDataSource(c);
            tabControl1.SelectTab(1);
        }
    }
}
