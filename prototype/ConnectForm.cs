using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace prototype
{
    public partial class ConnectForm : Form
    {
        public string server 
        { 
            get { return textBoxServer.Text; }
            set { textBoxServer.Text = value; }
        }
        public string login 
        { 
            get { return textBoxLogin.Text; }
            set { textBoxLogin.Text = value; }
        }
        public string password 
        { 
            get { return textBoxPassword.Text; }
            set { textBoxPassword.Text = value; }
        }
        public string database 
        { 
            get { return textBoxDatabase.Text; }
            set { textBoxDatabase.Text = value; }
        }
        public string table 
        { 
            get { return textBoxTable.Text; }
            set { textBoxTable.Text = value; }
        }
        public string connString
        {
            get
            {
                return @"user id=" + login + @";
                    password= " + password + @";
                    server= " + server + @";
                    database= " + database;
            }
        }
        public ConnectForm()
        {
            InitializeComponent();
        }
        public string TestConnection()
        {
            Cursor = Cursors.WaitCursor;
            bool pass = false;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                //add an error column (and test connection)
                string qString = "alter table Faults add Error varchar(20);";
                string qString2 = "alter table Faults add RegStat float;";
                SqlCommand cmd = new SqlCommand(qString, connection);
                try
                {
                    int n = cmd.ExecuteNonQuery();
                    pass = true;
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("Column names in each table must be unique"))
                    {
                        if (ex.Message.Contains("Cannot find the object \"Faults\""))
                        {
                            Global.errorStack.Add("Faults table must be created.\nUse the BDS40 option to setup the table.");
                            Global.errorStack.Add(ex.Message);
                            Global.GenerateErrors();
                            pass = true;
                        }
                        else
                        {
                            Global.errorStack.Add("Failed to Connect!");
                            Global.errorStack.Add(ex.Message);
                            Global.GenerateErrors();
                            pass = false;
                        }
                    }
                    else
                    {
                        cmd.CommandText = qString2;
                        try { int n = cmd.ExecuteNonQuery(); }
                        catch { }
                        pass = true;
                    }
                }
            }
            Cursor = Cursors.Default;
            if (pass) return "Success!";
            else return "Failed to Connect!";
        }
    }
}
