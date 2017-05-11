// Copyright 2013 (C) Cody Neuburger  All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace prototype
{
    public partial class Form1 : Form
    {
        #region Variables
        public DataViewer dataViewer;
        public ConnectForm connectForm;
        public Wizard wiz = new Wizard();

        DataTable data = new DataTable("Raw Instances");
        List<string> errorStack = new List<string>();

        string filename = "";
        const int STATE_CLEANSE = 0;
        const int STATE_OPEN = 1;
        const int STATE_SAVE = 2;
        const int STATE_NONE = 3;
        const int STATE_CANCEL = 4;
        const int STATE_REPORT_STRING = 5; //NYI
        const int STATE_ERROR = 6;
        const int STATE_SAVE_ALL = 7;
        const int STATE_SAVE_ERRORS = 8;
        int state = 3;

        StringPair status = new StringPair("Hello!", "");
        int percent = 0;
        #endregion

        public Form1()
        {
            Global.form1 = this;
            InitializeComponent();
            toolStripStatusLabel1.Text = "";
            toolStripStatusLabel2.Text = "";
            toolStripProgressBar1.Enabled = false;

            dataViewer = new DataViewer();
            dataViewer.CreateDatasetsButtonClicked += new EventHandler(CreateDatasets);
            dataViewer.Step2ButtonClicked += new EventHandler(Step2);
            connectForm = new ConnectForm();
            graph1.CoordinatesChanged += new EventHandler(UpdateCoordinates);

            toolStripComboBox1.Items.AddRange(Enum.GetNames(typeof(Graph.DotStyles)));
            toolStripComboBox1.Text = Enum.GetName(typeof(Graph.DotStyles), graph1.DotStyle);
            var dot = graph1.dotSize;
            var scale = graph1.ColorScale;
            toolStripTextBox1.Text = dot.ToString();
            toolStripTextBox2.Text = scale.ToString();
        }
        #region Open File
        private void OpenFile_Click(object sender, EventArgs e)
        {
            //DEPRECATED
            Cursor = Cursors.WaitCursor;
            try
            {
                switch (state)
                {
                    case STATE_NONE:
                        toolStripStatusLabel1.Text = "Opening File... ";
                        toolStripStatusLabel2.Text = "";
                        using (OpenFileDialog open = new OpenFileDialog())
                        {
                            open.Filter = "Comma Seperated (*.csv)|*.csv";
                            open.ShowDialog();
                            if (open.FileName != "")
                            {
                                OpenFileBlock();
                                state = STATE_OPEN;
                                OpenFileWorker.RunWorkerAsync(open.OpenFile());
                                filename = open.SafeFileName;
                            }
                        }
                        break;
                }
            }
            catch
            {
                MessageBox.Show(this, "The file could not be opened because it is being used by another program.", "Warning");
            }
            Cursor = Cursors.Default;
        }
        /// <summary>
        /// Disable GUI elements while a file is opening.
        /// </summary>
        private void OpenFileBlock()
        {
            OpenButton.Enabled = false;
            saveGraphToolStripMenuItem.Enabled = false;
            saveCSVToolStripMenuItem.Enabled = false;
            saveAllToolStripMenuItem.Enabled = false;
            SaveButton.Enabled = false;
        }
        /// <summary>
        /// Enamble GUI elements.
        /// </summary>
        private void OpenFileUnBlock()
        {
            OpenButton.Enabled = true;
            saveGraphToolStripMenuItem.Enabled = true;
            saveCSVToolStripMenuItem.Enabled = true;
            saveAllToolStripMenuItem.Enabled = true;
            SaveButton.Enabled = true;
        }
        #endregion
        #region Load Graph
        private void CreateDatasets(object sender, EventArgs e)
        {
            graph1.LoadData(dataViewer.GetDataTable(), dataViewer.IndependentColumn, dataViewer.DependentColumn);
            graph1.Title = dataViewer.Title;
            graph1.Title2 = dataViewer.Title2;
            graph1.Title3 = dataViewer.Title3;
        }
        private void Step2(object sender, EventArgs e)
        {
            graph1.LoadData(dataViewer.GetStep2Table(), dataViewer.IndependentColumn, dataViewer.DependentColumn, dataViewer.ErrorColumn);
            graph1.Title = dataViewer.Title;
            graph1.Title2 = dataViewer.Title2;
            graph1.Title3 = dataViewer.Title3;
        }
        #endregion
        #region Save
        DataTable saveTable = new DataTable();
        private void SaveFileButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            switch (state)
            {
                case STATE_NONE:
                    using (SaveFileDialog save = new SaveFileDialog())
                    {
                        save.Filter = "Comma Seperated (*.csv)|*.csv";
                        save.FileName = graph1.Title + "_" + filename;
                        save.ShowDialog();
                        if (save.FileName != "")
                        {
                            state = STATE_SAVE;
                            OpenFileBlock();
                            saveTable = dataViewer.GetDataTable();
                            SaveFileWorker.RunWorkerAsync((FileStream)save.OpenFile());
                        }
                    }
                    break;
            }
            Cursor = Cursors.Default;
        }
        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            switch (state)
            {
                case STATE_NONE:
                    using (SaveFileDialog save = new SaveFileDialog())
                    {
                        save.Filter = "Comma Seperated (*.csv)|*.csv";
                        save.FileName = graph1.Title + "_" + filename;
                        save.ShowDialog();
                        if (save.FileName != "")
                        {
                            state = STATE_SAVE_ALL;
                            OpenFileBlock();
                            saveTable = dataViewer.GetFullData();
                            SaveFileWorker.RunWorkerAsync((FileStream)save.OpenFile());
                        }
                    }
                    break;
            }
            Cursor = Cursors.Default;
        }
        private void saveErrorDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DEPRECATED
            Cursor = Cursors.WaitCursor;
            switch (state)
            {
                case STATE_NONE:
                    using (SaveFileDialog save = new SaveFileDialog())
                    {
                        save.Filter = "Comma Seperated (*.csv)|*.csv";
                        save.FileName = "Err_" + filename;
                        save.ShowDialog();
                        if (save.FileName != "")
                        {
                            state = STATE_SAVE_ERRORS;
                            OpenFileBlock();
                            saveTable = dataViewer.GetErrorData();
                            SaveFileWorker.RunWorkerAsync((FileStream)save.OpenFile());
                        }
                    }
                    break;
            }
            Cursor = Cursors.Default;
        }
        private void SaveFileWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                switch (state)
                {
                    case STATE_SAVE:
                        Save(e, saveTable, graph1.Title);
                        break;
                    case STATE_SAVE_ALL:
                        Save(e, saveTable, "All");
                        break;
                    case STATE_SAVE_ERRORS:
                        Save(e, saveTable, "Errors");
                        break;
                }
            }
            catch(Exception ex)
            {
                Global.errorStack.Add("Could not save to file!");
                Global.errorStack.Add(ex.Message);
                state = STATE_ERROR;
            }
        }
        private void Save(DoWorkEventArgs e, DataTable dataTable, string title)
        {

            FileStream outFile = (FileStream)e.Argument;
            StreamWriter writer = new StreamWriter(outFile);
            try
            {
                string stuff = "";
                foreach (DataColumn column in dataTable.Columns)
                {
                    stuff += column.ColumnName + ",";
                }
                writer.WriteLine(stuff);
                status.s1 = "Saving " + title + "...";
                int c = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    c++;
                    stuff = "";
                    ProgressUpdate(SaveFileWorker, dataTable.Rows.Count, c, 500);
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        if (column.DataType == typeof(DateTime))
                        {
                            stuff += ((DateTime)row[column]).ToString("MM/dd/yyyy") + ",";
                        }
                        else
                        {
                            stuff += row[column].ToString() + ",";
                        }
                    }
                    writer.WriteLine(stuff);
                }

                //Done
                ProgressUpdate(SaveFileWorker, dataTable.Rows.Count, data.Rows.Count, 1);
            }
            catch (Exception ex)
            {
                errorStack.Add("Problem Saving File");
                errorStack.Add(ex.Message);
                state = STATE_ERROR;
            }
            finally
            {
                writer.Close();
                outFile.Close();
            }
        }
        private void SaveFileWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                toolStripStatusLabel1.Text = ((StringPair)e.UserState).s1;
                toolStripStatusLabel2.Text = ((StringPair)e.UserState).s2;
                toolStripProgressBar1.Value = e.ProgressPercentage;
            }
            catch
            {
            }
        }
        private void SaveFileWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OpenFileUnBlock();
            Cursor = Cursors.WaitCursor;
            toolStripProgressBar1.Value = 0;
            switch (state)
            {
                case STATE_SAVE_ERRORS:
                case STATE_SAVE_ALL:
                case STATE_SAVE:
                    state = STATE_NONE;
                    toolStripStatusLabel1.Text = "Saved.";
                    toolStripStatusLabel2.Text = "";
                    break;
                case STATE_CANCEL:
                    state = STATE_NONE;
                    toolStripStatusLabel1.Text = "Cancelled.";
                    toolStripStatusLabel2.Text = "";
                    break;
                case STATE_ERROR:
                    state = STATE_NONE;
                    toolStripStatusLabel1.Text = "Error on Save.";
                    toolStripStatusLabel2.Text = "";
                    Global.GenerateErrors();
                    break;
            }
            Cursor = Cursors.Default;
        }
        private void saveGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var bmp = new Bitmap(graph1.Width, graph1.Height))
                {
                    graph1.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    using (SaveFileDialog save = new SaveFileDialog())
                    {
                        save.Filter = "Image File (*.png)|*.png";
                        save.FileName = Global.getSafeFileName("img_" + graph1.Title);
                        save.ShowDialog();
                        if (save.FileName != "")
                        {
                            bmp.Save(save.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorStack.Add("Snapshot Failed.");
                errorStack.Add(ex.Message);
                Global.GenerateErrors();
            }
        }
        #endregion
        #region UI Stuff
        public void ProgressUpdate(BackgroundWorker worker, int n, int i, int delay)
        {
            if (i % delay == 0)
            {
                status.s2 = i + "/" + n;
                percent = (int)((double)i / n * 100);
                worker.ReportProgress(percent, status);
            }
        }
        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataViewer.Show();
            dataViewer.SetDataSource(connectForm);
            SaveButton.Enabled = true;
            dataViewerButton.Enabled = true;
            saveAllToolStripMenuItem.Enabled = true;
            saveCSVToolStripMenuItem.Enabled = true;
        }
        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                graph1.ColorScale = float.Parse(toolStripTextBox2.Text);
            }
            catch
            {
                var scale = graph1.ColorScale;
                toolStripTextBox2.Text = scale.ToString();
            }
        }
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            graph1.DotStyle = (Graph.DotStyles)Enum.Parse(typeof(Graph.DotStyles), toolStripComboBox1.Text);
            graph1.Refresh();
        }
        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                graph1.dotSize = float.Parse(toolStripTextBox1.Text);
            }
            catch
            {
                var dot = graph1.dotSize;
                toolStripTextBox1.Text = dot.ToString();
            }
        }
        private void UpdateCoordinates(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = graph1.coordinates;
        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                toolStripButton8.BackColor = colorDialog1.Color;
                graph1.penColor = colorDialog1.Color;
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void fullGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graph1.FullGraph();
        }
        private void FullButton_Click(object sender, EventArgs e)
        {
            graph1.FullGraph();
        }
        private void MoveButton_Click(object sender, EventArgs e)
        {
            graph1.moveAndResizeMode = !graph1.moveAndResizeMode;
        }
        private void dateCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dateCoordinatesToolStripMenuItem.Checked = !dateCoordinatesToolStripMenuItem.Checked;
            graph1.dateCoordinates = dateCoordinatesToolStripMenuItem.Checked;
        }
        private void onCursorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onCursorToolStripMenuItem.Checked = !onCursorToolStripMenuItem.Checked;
            graph1.displayOnCursor = onCursorToolStripMenuItem.Checked;
        }
        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showGridToolStripMenuItem.Checked = !showGridToolStripMenuItem.Checked;
            graph1.showGrid = showGridToolStripMenuItem.Checked;
        }
        private void minimalGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            minimalGridToolStripMenuItem.Checked = !minimalGridToolStripMenuItem.Checked;
            graph1.minimalGrid = minimalGridToolStripMenuItem.Checked;
        }
        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }
        private void HorizontalShrinkButton_Click(object sender, EventArgs e)
        {
            graph1.ShrinkHorizontal();
        }
        private void HorizonalStretchButton_Click(object sender, EventArgs e)
        {
            graph1.StretchHorizontal();
        }
        private void VerticalShrinkButton_Click(object sender, EventArgs e)
        {
            graph1.ShrinkVertical();
        }
        private void VerticalStretchButton_Click(object sender, EventArgs e)
        {
            graph1.StretchVertical();
        }
        private void showDataTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataViewer.Show();
        }
        #endregion

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wiz.ShowDialog();
        }
    }
}
