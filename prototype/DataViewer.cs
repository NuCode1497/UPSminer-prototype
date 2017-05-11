// Copyright 2012 (C) Cody Neuburger  All rights reserved.
// This Control was written in a generic way to provide a user interface and functionality 
// to organize a mashed data table into a data set usable by a Graph object.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;

namespace prototype
{
    public partial class DataViewer : Form
    {
        #region public
        //Properties
        public EventHandler IndependentAxisCheckChanged;
        public EventHandler DependentAxisCheckChanged;
        public EventHandler CreateDatasetsButtonClicked;
        public EventHandler Step2ButtonClicked;
        public string IndependentColumn
        {
            get
            {
                try
                {
                    return checkedListBoxInd.CheckedItems[0].ToString();
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                checkedListBoxInd.SetItemChecked(checkedListBoxInd.FindStringExact(value), true);
            }
        }
        public SqlDbType IndependentColumnType
        {
            get
            {
                return (SqlDbType)(int)SchemaTable.Rows[0]["ProviderType"];
            }
        }
        public int IndependentColumnSize
        {
            get
            {
                return (int)SchemaTable.Rows[0]["ColumnSize"];
            }
        }
        public string DependentColumn
        {
            get
            {
                try
                {
                    return checkedListBoxDep.CheckedItems[0].ToString();
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                checkedListBoxDep.SetItemChecked(checkedListBoxDep.FindStringExact(value), true);
            }
        }
        public SqlDbType DependentColumnType
        {
            get
            {
                return (SqlDbType)(int)SchemaTable.Rows[1]["ProviderType"];
            }
        }
        public int DependentColumnSize
        {
            get
            {
                return (int)SchemaTable.Rows[1]["ColumnSize"];
            }
        }
        public string ErrorColumn
        {
            get
            {
                try
                {
                    switch (state)
                    {
                        case STATE_DB_NONE:
                        case STATE_DB_CANCEL:
                        case STATE_DB_CLEANSE:
                        case STATE_DB_ERROR:
                            return "RegStat";
                        default:
                            Global.NYI("State " + state + " has not been set up for the ErrorColumn property.");
                            throw new Exception("State " + state + " has not been set up for the ErrorColumn property.");
                    }
                }
                catch
                {
                    return "";
                }
            }
        }
        public string Title = "";
        public string Title2 = "";
        public string Title3 = "";
        public ConnectForm connectForm;

        public DataViewer()
        {
            InitializeComponent();
        }
        #region Interface Input
        public void SetDataSource(ConnectForm connectForm)
        {
            buttonRefresh.Show();
            this.connectForm = connectForm;
            connectForm.ShowDialog(this);
            LoadDBConnection();

            dataTable = new DataTable("Graph Data");
            errorTable = new DataTable("Error Data");
            step2Table = new DataTable();
            Thresholds = new DataTable();
            dataView = new DataView(dataTable);
            errorView = new DataView(errorTable);
            step2View = new DataView(step2Table);
            dataGridViewStep2.DataSource = step2View;
            dataGridViewError.DataSource = errorView;
            dataGridViewGraph.DataSource = dataView;
            dataGridViewThresh.DataSource = null;

            filterBin.Clear();
            EnableButtons();

            //HACK
            try
            {
                checkedListBoxKeys.SetItemChecked(10, true);
                checkedListBoxKeys.SetItemChecked(6, true);
                checkedListBoxKeys.SetItemChecked(5, true);
                checkedListBoxInd.SetItemChecked(1, true);
                checkedListBoxDep.SetItemChecked(3, true);
                SetupFilters();
                buttonRefresh_Click(this, EventArgs.Empty);
                //1293309 6 11
                filterBin[0].comboBox1.Text = "1293309";
                filterBin[1].comboBox1.Text = "6";
                filterBin[2].comboBox1.Text = "11";
                OnFilterSelectionChangedDB(this, EventArgs.Empty);
                tabControl1.SelectTab(1);
            }
            catch
            {
            }
        }
        #endregion
        #region Interface Output
        /// <summary>
        /// Gets a DataTable of all rows in this DataViewer's DataSource that match 
        /// the filter criteria in order of primary key (or lacking one, order of addition.)
        /// </summary>
        /// <param name="filterExpression">The criteria to use to filter the rows.</param>
        /// <returns>A filtered DataTable object.</returns>
        public DataTable GetDataTable(string filterExpression)
        {
            dataView.RowFilter = filterExpression;
            return GetDataTable();
        }
        /// <summary>
        /// Gets a DataTable of all rows in this DataViewer's DataSource that match 
        /// the filter specified by the input DataTable's first row.
        /// </summary>
        /// <param name="referenceTable">The table to use to filter rows.</param>
        /// <returns>A filtered DataTable object.</returns>
        public DataTable GetDataTable(DataTable referenceTable)
        {
            if (referenceTable.Columns.Count != Signatures.Columns.Count)
            {
                throw new Exception(" Column count doesn't match. ");
            }

            string query = "";
            foreach (DataColumn column in referenceTable.Columns)
            {
                if (query != "") query += " AND ";
                query += column.ColumnName + " = " + referenceTable.Rows[0][column].ToString();
            }
            return GetDataTable(query);
        }
        /// <summary>
        /// Gets a DataTable of all rows in this DataViewer's DataSource that match 
        /// the filter specified by the input list. The input list is specified in a
        /// columnName, cellData, columnName, cellData, ... pattern.
        /// </summary>
        /// <param name="dataPairs">The array of values to specify a filter.</param>
        /// <returns>A filtered DataTable object.</returns>
        public DataTable GetDataTable(params object[] dataPairs)
        {
            if (Signatures.Columns.Count * 2 != dataPairs.Count())
            {
                throw new Exception(" Column count doesn't match. ");
            }
            DataTable referenceTable = new DataTable("Signature");
            List<object> values = new List<object>();
            try
            {
                for (int i = 0; i < dataPairs.Count(); i++)
                {
                    if (i % 2 == 0)
                    {
                        referenceTable.Columns.Add(new DataColumn((string)dataPairs[i], dataPairs[i + 1].GetType()));
                    }
                    else
                    {
                        values.Add(dataPairs[i]);
                    }
                }
                referenceTable.Rows.Add(values);
            }
            catch
            {
                throw new Exception("Bad parameter format.");
            }
            return GetDataTable(referenceTable);
        }
        /// <summary>
        /// Gets the current filtered data table 
        /// </summary>
        /// <returns>A filtered DataTable using the current view.</returns>
        public DataTable GetDataTable()
        {
            dataView.Sort = IndependentColumn;
            return dataView.ToTable();
        }
        /// <summary>
        /// Gets the step 2 data table
        /// </summary>
        /// <returns></returns>
        public DataTable GetStep2Table()
        {
            return step2Table.DefaultView.ToTable();
        }
        public DataTable GetFullData()
        {
            switch (state)
            {
                case STATE_DB_NONE:
                    SelectAll();
                    return dataTable.Copy();
                default:
                    throw new Exception("Please wait for the Data Viewer to finish processing.");
            }
        }
        public DataTable GetErrorData()
        {
            return errorTable.Copy();
        }
        #endregion
        #endregion
        #region private variables
        //Under the Hood
        private DataTable dataTable;
        private DataTable errorTable;
        private DataTable step2Table;
        private DataTable step2ProcTable;
        private DataView dataView;
        private DataView errorView;
        private DataView step2View;
        private DataTable ColumnsTypes = new DataTable();
        private DataTable SchemaTable = new DataTable();
        private DataTable Signatures = new DataTable();
        private DataTable Thresholds = new DataTable();
        StringPair status = new StringPair("Hello!", "");
        List<TableFilter> filters = new List<TableFilter>();
        List<TableFilter> filterBin = new List<TableFilter>();
        int percent = 0;
        bool isCleansed = false;
        //Database
        private string server { get { return connectForm.server; } }
        private string login { get { return connectForm.login; } }
        private string password { get { return connectForm.password; } }
        private string database { get { return connectForm.database; } }
        private string table { get { return connectForm.table; } }
        private string connString { get { return connectForm.connString; } }

        string[] RESTORE_keys;
        string RESTORE_ind = "The chances of a column having this name are astornomicallifornially low!";
        string RESTORE_dep = "The chances of a column having this name are astronmonomonomically low!";
        string[] RESTORE_keyValues;
        string sqlSigWhereClause;

        private int state = 0;
        const int STATE_DB_NONE = 5;
        const int STATE_DB_CANCEL = 6;
        const int STATE_DB_ERROR = 7;
        const int STATE_DB_CLEANSE = 8;
        #endregion
        #region Connect To Database
        //This stuff is called when using a database connection and pressing refresh
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            //save choices
            RESTORE_keys = new string[checkedListBoxKeys.CheckedItems.Count];
            checkedListBoxKeys.CheckedItems.CopyTo(RESTORE_keys, 0);
            try { RESTORE_ind = IndependentColumn; }
            catch { }
            try { RESTORE_dep = DependentColumn; }
            catch { }
            RESTORE_keyValues = new string[RESTORE_keys.Length];
            for (int i = 0; i < RESTORE_keys.Length; i++)
            {
                TableFilter filter = (TableFilter)flowLayoutPanel1.Controls[i];
                ComboBox box = filter.comboBox1;
                RESTORE_keyValues[i] = box.Text;
            }

            //reload keys
            //clear tables
            LoadDBConnection();

            //Refresh filters the next time they are opened.
            foreach (TableFilter filter in filterBin)
            {
                filter.state = TableFilter.STATE_REFRESH;
            }

            //load tables
        }
        private void LoadDBConnection()
        {
            try
            {
                state = STATE_DB_NONE;
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = "Testing Connection...";
                toolStripStatusLabel2.Text = "";
                Text = "Data Viewer";

                ResetEverything();
                connectForm.TestConnection();
                SetupKeyListsDB();
                SetSignaturesDB();
            }
            catch { }
        }
        private void ResetEverything()
        {
            checkedListBoxDep.Items.Clear();
            checkedListBoxInd.Items.Clear();
            checkedListBoxKeys.Items.Clear();
            checkedListBoxThresh.Items.Clear();
            flowLayoutPanel1.Controls.Clear();
            filters.Clear();
            Global.errorStack.Clear();
        }
        private void SetupKeyListsDB()
        {
            try
            {
                //get column names
                string select1 = "SELECT COLUMN_NAME, DATA_TYPE FROM " + database +
                    ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" +
                    table.Split('.').Last() + "'";
                SqlDataAdapter query = new SqlDataAdapter(select1, connString);
                query.SelectCommand.CommandTimeout = 0;
                ColumnsTypes.Clear();
                query.Fill(ColumnsTypes);
                var columns =
                    from row in ColumnsTypes.AsEnumerable()
                    select row.Field<object>("COLUMN_NAME");
                //fill out key, ind, thresh lists
                checkedListBoxKeys.Items.AddRange(columns.ToArray());
                checkedListBoxInd.Items.AddRange(columns.ToArray());
                checkedListBoxThresh.Items.AddRange(columns.ToArray());
                //fill out dep list
                var numberColumns =
                    from row in ColumnsTypes.AsEnumerable()
                    where row.Field<object>("DATA_TYPE").ToString() == "float"
                    select row.Field<object>("COLUMN_NAME");
                checkedListBoxDep.Items.AddRange(numberColumns.ToArray());

                EnableButtons();
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("There was a problem filling out key lists!");
                Global.errorStack.Add(ex.Message);
                Global.GenerateErrors();
                toolStripStatusLabel1.Text = "Failure generating Keys!";
                toolStripStatusLabel2.Text = ex.Message;
                throw ex;
            }

            //restore key check marks
            try
            {
                for (int i = 0; i < RESTORE_keys.Length; i++)
                {
                    checkedListBoxKeys.SetItemChecked(checkedListBoxKeys.FindString(RESTORE_keys[i]), true);
                }
            }
            catch { }
            try
            {
                if (RESTORE_ind != "")
                {
                    checkedListBoxInd.SetItemChecked(checkedListBoxInd.FindString(RESTORE_ind), true);
                }
            }
            catch { }
            try
            {
                if (RESTORE_ind != "")
                {
                    checkedListBoxDep.SetItemChecked(checkedListBoxDep.FindString(RESTORE_dep), true);
                }
            }
            catch { }

            SetupFilters();
        }
        private void SetSignaturesDB()
        {
            //find all possible signatures for the given key choices
            //store them in the Signature table

            try
            {
                //create a parameter array
                string[] columns = new string[checkedListBoxKeys.CheckedItems.Count];
                if (columns.Count() == 0) return;
                checkedListBoxKeys.CheckedItems.CopyTo(columns, 0);
                //generate a query string
                string select1 = "select distinct ";
                for (int i = 0; i < columns.Length - 1; i++)
                {
                    select1 += columns[i] + ", ";
                }
                select1 += columns[columns.Length - 1] + " from " + table;
                SqlDataAdapter query = new SqlDataAdapter(select1, connString);
                query.SelectCommand.CommandTimeout = 0;
                Signatures.Clear();
                query.Fill(Signatures);
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("There was a problem while getting signatures!");
                Global.errorStack.Add(ex.Message);
                Global.GenerateErrors();
                throw ex;
            }
        }
        #endregion
        #region Setup Table Filters
        //This stuff is called when a key checkbox is checked
        //sets up a table filter for the key
        private void SetupFilters()
        {
            switch (state)
            {
                case STATE_DB_NONE:
                    SetupKeysDB();
                    flowLayoutPanel1.Controls.Clear();
                    flowLayoutPanel1.Controls.AddRange(filters.ToArray());
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = "Ready.";
                    toolStripStatusLabel2.Text = "";
                    break;
            }
            EnableButtons();
        }
        private void SetupKeysDB()
        {
            //called when a key checkbox is checked
            //sets up a table filter for the key
            try
            {
                filters.Clear();
                int n = checkedListBoxKeys.CheckedItems.Count;
                for (int c = 0; c < n; c++)
                {
                    string key = checkedListBoxKeys.CheckedItems[c].ToString();

                    //check if this filter was already created once
                    TableFilter filter = filterBin.Find(delegate(TableFilter tf) { return tf.Name == key; });
                    if (filter == null)
                    {
                        //the filter does not call the query until the drop down menu is opened
                        filter = new TableFilter();
                        filter.Setup(key, ref Signatures);
                        filter.boxTextChanged += OnFilterSelectionChangedDB;
                        filterBin.Add(filter);
                    }
                    filters.Add(filter);
                }
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("Error while Setting Up Keys");
                Global.errorStack.Add(ex.Message);
                state = STATE_DB_ERROR;
            }
        }
        #endregion
        #region Setup Threshold Table
        private void checkedListBoxThresh_SelectedValueChanged(object sender, EventArgs e)
        {
            tabControl2.SelectTab(tabThresholds);
            switch (state)
            {
                case STATE_DB_NONE:
                    SetThresholdTableDB();
                    EnableButtons();
                    break;
            }
        }
        private void SetThresholdTableDB()
        {
            try
            {
                string select1;
                SqlDataAdapter query;
                string[] columnsT = new string[checkedListBoxThresh.CheckedItems.Count];
                if (columnsT.Count() == 0) return;
                if (dataTable.Columns.Count == 0) return;
                checkedListBoxThresh.CheckedItems.CopyTo(columnsT, 0);
                select1 = "select distinct ";
                for (int c = 0; c < checkedListBoxThresh.CheckedItems.Count; c++)
                {
                    string key = checkedListBoxThresh.CheckedItems[c].ToString();
                    select1 += key + ",";
                }
                select1 = select1.Substring(0, select1.Length - 1) + " from " + table + " ";
                select1 += sqlSigWhereClause;
                query = new SqlDataAdapter(select1, connString);
                query.SelectCommand.CommandTimeout = 0;
                Thresholds = new DataTable("Thresholds");
                query.Fill(Thresholds); //send query
                Thresholds.Columns.Add("Low " + DependentColumn + " Threshold",
                    dataTable.Columns[DependentColumn].DataType);
                Thresholds.Columns.Add("High " + DependentColumn + " Threshold",
                    dataTable.Columns[DependentColumn].DataType);
                dataGridViewThresh.DataSource = Thresholds;
            }
            catch(Exception ex)
            {
                Global.errorStack.Add("There was a problem creating the Threshold table!");
                Global.errorStack.Add(ex.Message);
                Global.GenerateErrors();
            }
        }
        #endregion
        #region Load Tables
        //This stuff is called when a filter value is chosen
        private void OnFilterSelectionChangedDB(object sender, EventArgs e)
        {
            try
            {
                genSqlSigWhereClause();
                FilterViewDB();
                EnableButtons();
            }
            catch
            {
            }
        }
        private void FilterViewDB()
        {
            try
            {
                string select1 = "";
                string[] columns = new string[checkedListBoxKeys.CheckedItems.Count];
                if (columns.Count() == 0) return;
                checkedListBoxKeys.CheckedItems.CopyTo(columns, 0);

                //depending on the currently focused tab, modify the select statement
                //and fill the appropriate table.
                SqlDataAdapter query;
                switch (tabControl2.SelectedTab.Text)
                {
                    case "Graph Data":
                        select1 = "select distinct ";
                        select1 += IndependentColumn + ", ";
                        select1 += DependentColumn + " from BatteryReadings, Faults ";
                        select1 += sqlSigWhereClause;
                        select1 += " and Faults.ReadingID = BatteryReadings.ReadingID";
                        select1 += " and (error is null or error = '')";
                        select1 += " order by " + IndependentColumn; //sort
                        query = new SqlDataAdapter(select1, connString);
                        query.SelectCommand.CommandTimeout = 0;
                        dataTable.Clear();
                        query.Fill(dataTable); //send query
                        HideColumns(dataGridViewGraph);
                        dataGridViewGraph.Columns[IndependentColumn].Visible = true;
                        dataGridViewGraph.Columns[DependentColumn].Visible = true;
                        break;
                    case "Thresholds":
                        if(Thresholds.Columns.Count == 0)
                            SetThresholdTableDB();
                        break;
                    case "Error Data":
                        select1 = "select distinct ";
                        select1 += IndependentColumn + ", ";
                        select1 += DependentColumn + ", ";
                        select1 += "error from BatteryReadings, Faults ";
                        select1 += sqlSigWhereClause;
                        select1 += " and Faults.ReadingID = BatteryReadings.ReadingID";
                        select1 += " and error is not null and error <> ''";
                        select1 += " order by " + IndependentColumn; //sort
                        query = new SqlDataAdapter(select1, connString);
                        query.SelectCommand.CommandTimeout = 0;
                        errorTable.Clear();
                        query.Fill(errorTable); //send query
                        HideColumns(dataGridViewError);
                        try
                        {
                            dataGridViewError.Columns[IndependentColumn].Visible = true;
                            dataGridViewError.Columns[DependentColumn].Visible = true;
                            dataGridViewError.Columns["error"].Visible = true;
                        }
                        catch { }
                        break;
                    case "Step 2":
                        select1 = "select distinct ";
                        select1 += IndependentColumn + ", ";
                        select1 += DependentColumn + ", ";
                        select1 += "RegStat from BatteryReadings, Faults ";
                        select1 += sqlSigWhereClause;
                        select1 += " and Faults.ReadingID = BatteryReadings.ReadingID";
                        select1 += " and RegStat is not null and RegStat > -1";
                        select1 += " order by " + IndependentColumn; //sort
                        query = new SqlDataAdapter(select1, connString);
                        query.SelectCommand.CommandTimeout = 0;
                        step2Table.Clear();
                        query.Fill(step2Table); //send query
                        dataGridViewStep2.DataSource = step2Table;
                        HideColumns(dataGridViewStep2);
                        try
                        {
                            dataGridViewStep2.Columns[IndependentColumn].Visible = true;
                            dataGridViewStep2.Columns[DependentColumn].Visible = true;
                            dataGridViewStep2.Columns["RegStat"].Visible = true;
                        }
                        catch { }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("There was a problem generating the data table!");
                Global.errorStack.Add(ex.Message);
            }
        }
        private void genSqlSigWhereClause()
        {
            sqlSigWhereClause = " where ";
            foreach (object item in flowLayoutPanel1.Controls)
            {
                string filterValue = ((TableFilter)item).comboBox1.Text;
                string filterColumn = ((TableFilter)item).comboBox1.Name;
                if (filterValue == "-Refresh-" || filterValue == "")
                    throw new Exception("Choose key values.");
                sqlSigWhereClause += filterColumn + " = '" + filterValue + "' and ";
            }
            sqlSigWhereClause = sqlSigWhereClause.Substring(0, sqlSigWhereClause.Length - 5);
        }
        #endregion
        #region BDS40
        private void buttonBDS40_Click(object sender, EventArgs e)
        {
            try
            {
                switch (state)
                {
                    case STATE_DB_NONE:
                        BlockControls();
                        //buttonBDS40.Enabled = true;
                        tabControl2.SelectTab("Console");
                        buttonBDS40.Text = "Cancel BDS40";
                        state = STATE_DB_CLEANSE;
                        BDS40Worker.RunWorkerAsync();
                        break;
                    case STATE_DB_CANCEL:
                        break;
                    case STATE_DB_CLEANSE:
                        CleanseWorker.CancelAsync();
                        state = STATE_DB_CANCEL;
                        break;
                }
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("Could not cleanse.");
                Global.errorStack.Add(ex.Message);
                Global.GenerateErrors();
            }
        }
        private void BDS40Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                switch (state)
                {
                    case STATE_DB_CLEANSE:
                        BDS40DetectFaults();
                        break;
                }
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("Error while Cleansing Data");
                Global.errorStack.Add(ex.Message);
                switch (state)
                {
                    case STATE_DB_CLEANSE:
                        state = STATE_DB_ERROR;
                        break;
                }
            }
        }
        private void BDS40DetectFaults()
        {
            if(checkBoxBDS40Setup.Checked) FaultsSetup();
            if (checkBoxF1.Checked) BDS40_F1();
            if (checkBoxF2.Checked) BDS40_F2();
            if (checkBoxF3.Checked) BDS40_F3();
            if (checkBoxF4.Checked) BDS40_F4();
            if (checkBoxF5.Checked) BDS40_F5();
            if (checkBoxF6.Checked) BDS40_F6();
            if (checkBoxF7.Checked) BDS40_F7();
            if (checkBoxF10.Checked) BDS40_F10();

            status.s1 = "BDS40 done!";
            status.s2 = "";
            BDS40Worker.ReportProgress(0, status);
        }
        private void FaultsSetup()
        {
            string cmd1 =
                "create table Faults (ReadingID bigint unique foreign key references " +
                table + "(ReadingID) not null,  BDS40_ID bigint primary key, " +
                "Error varchar(20), RegStat float, BDS40_F1 float, BDS40_F2 float, " +
                "BDS40_F3 float, BDS40_F4 float, BDS40_F5 float, BDS40_F6 float, " +
                "BDS40_F7 float, BDS40_F10 float) ";
            InstantUpdate(1, 18, "Creating Faults table...");
            trySqlCommand(cmd1);
            InstantUpdate(2, 18, "Clearing Faults table...");
            trySqlCommand("delete from Faults");

            string cmd2 =
                "insert into Faults " +
                "(ReadingID, BDS40_ID) " +
                "select " +
                "A.ReadingID, B.Rank " +
                "from " + table + " A " +
                "LEFT OUTER JOIN " +
                "(" +
                    "select rank() over " +
                        "(order by StringTag, StringNumber, " +
                        "CellNumber, ReadingDate, ReadingID) " +
                    "as Rank, ReadingID " +
                    "from " + table +
                ") B " +
                "on A.ReadingID = B.ReadingID ";
            InstantUpdate(3, 18, "Filling Faults table...");
            trySqlCommand(cmd2);
        }
        private void InstantUpdate(int c, int n, string description)
        {
            percent = (int)((double)c / n * 100);
            Invoke(new Action(() => DVConsole.AppendText("\n" + description)));
            Invoke(new Action(() => toolStripProgressBar1.Value = percent));
            Invoke(new Action(() => toolStripStatusLabel1.Text = description));
            Invoke(new Action(() => toolStripStatusLabel2.Text = c + "/" + n));
        }
        private void trySqlCommand(string command, params object[] values)
        {
            StopwatchWorkerStart();
            string response = "Failed to Connect!";
            using(SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(command, connection);
                    cmd.CommandTimeout = 0;
                    for (int i = 0; i < values.Count(); i = i + 2)
                    {
                        cmd.Parameters.AddWithValue((string)values[i], values[i+1]);
                    }
                    int rows = cmd.ExecuteNonQuery();
                    response = "(" + rows + " rows affected.)";
                }
                catch(Exception e)
                {
                    response = e.Message + "\nThe statement has been terminated.";
                }
            }
            StopwatchWorker.CancelAsync();
            Invoke(new Action(() => DVConsole.AppendText("\n" + response + "\nTime: " + 
                toolStripStatusLabel3.Text)));
        }
        private void BDS40Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UnBlockControls();
            buttonBDS40.Text = "BDS40 Faults";
            switch (state)
            {
                case STATE_DB_NONE:
                    break;
                case STATE_DB_ERROR:
                    state = STATE_DB_NONE;
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = "Error.";
                    toolStripStatusLabel2.Text = "";
                    Global.GenerateErrors();
                    break;
                case STATE_DB_CLEANSE:
                    state = STATE_DB_NONE;
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = "Ready.";
                    toolStripStatusLabel2.Text = "";
                    Title3 = "Step 1: BDS40 Faults";
                    break;
                case STATE_DB_CANCEL:
                    state = STATE_DB_NONE;
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = "Cancelled.";
                    toolStripStatusLabel2.Text = "";
                    break;
            }
        }
        private void BDS40_F1()
        {
            //Fault 1 readings occur when two adjacent batteries have 
            //CellResistance == NULL and CellVoltage <= 0.5

            //Probability of F1 if V is less than 5
            float P_V_LT5 = 0.38f;
            //Probability of F1 if R is null
            float P_R_NULL = 0.12f;

            //Update database with F1 findings
            string update1 = "update Faults set BDS40_F1 = (BDS40_F1 + @BDS40_F1) " +
                "where ReadingID in (select ReadingID " +
                "from " + table + " where CellVoltage <= 0.5)";
            InstantUpdate(4, 18, "Fault1: V <= 0.5...");
            trySqlCommand(update1, "@BDS40_F1", P_V_LT5);

            string update2 = "update Faults set BDS40_F1 = (BDS40_F1 + @BDS40_F1) " +
                "where ReadingID in (select ReadingID " +
                "from " + table + " where CellResistance is null)";
            InstantUpdate(5, 18, "Fault1: R is null...");
            trySqlCommand(update2, "@BDS40_F1", P_R_NULL);

            string update3 =
                "update Faults set BDS40_F1 = BDS40_F1 + .38 " +
                "from " + table + " A, " + table + " B, Faults C " +
                "where  " +
                    "A.StringTag = B.StringTag  " +
                "and A.StringNumber = B.StringNumber " +
                "and A.CellNumber = (B.CellNumber - 1)  " +
                "and A.ReadingDate = B.ReadingDate " +
                "and A.CellVoltage <= 0.5  " +
                "and B.CellVoltage <= 0.5 " +
                "and A.ReadingID = C.ReadingID ";

            InstantUpdate(6, 18, "Fault1: Next V <= 0.5...");
            trySqlCommand(update3, "@BDS40_F1", P_V_LT5);

            string update4 =
                "update Faults set BDS40_F1 = BDS40_F1 + .38 " +
                "from " + table + " A, " + table + " B, Faults C " +
                "where  " +
                    "A.StringTag = B.StringTag  " +
                "and A.StringNumber = B.StringNumber " +
                "and A.CellNumber = (B.CellNumber - 1)  " +
                "and A.ReadingDate = B.ReadingDate " +
                "and A.CellResistance is null  " +
                "and B.CellResistance is null " +
                "and A.ReadingID = C.ReadingID ";
            InstantUpdate(7, 18, "Fault1: Next R is null...");
            trySqlCommand(update4, "@BDS40_F1", P_R_NULL);
        }
        private void BDS40_F2()
        {
            //Fault 2 readings occur when
            //CellVoltage > 17.4 and CellResistance == NULL

            //Probability of F2 if voltage is greater than 17.4
            float P_V_GT17 = 0.75f;
            //Probability of F2 if resistance is null
            float P_R_NULL = 0.25f;

            //Update database with F2 findings
            string update1 = "update Faults set BDS40_F2 = BDS40_F2 + @BDS40_F2 " +
                "where ReadingID in (select ReadingID " +
                "from " + table + " where CellVoltage > 17.4)";
            InstantUpdate(8, 18, "Fault2: V > 17.4...");
            trySqlCommand(update1, "@BDS40_F2", P_V_GT17);

            string update2 = "update Faults set BDS40_F2 = BDS40_F2 + @BDS40_F2 " +
                "where ReadingID in (select ReadingID " +
                "from " + table + " where CellResistance is null)";
            InstantUpdate(9, 18, "Fault2: R is null...");
            trySqlCommand(update2, "@BDS40_F2", P_R_NULL);
        }
        private void BDS40_F3()
        {
            //Fault 3 readings occur when
            //CellResistance == 65535 and CellVoltage < 13.1

            //Probability of F3 if resistance is maxed
            float P_R_MAX = .75f;
            //Probabilty of F3 if Voltage is less than 13.1
            float P_V_LT13 = .25f;

            //Update database with F3 findings
            string update1 = "update Faults set BDS40_F3 = (BDS40_F3 + @BDS40_F3) " +
                "where ReadingID in (select ReadingID " +
                "from " + table + " where CellVoltage < 13.1)";
            InstantUpdate(9, 18, "Fault3: V < 13.1...");
            trySqlCommand(update1, "@BDS40_F3", P_V_LT13);

            string update2 = "update Faults set BDS40_F3 = (BDS40_F3 + @BDS40_F3) " +
                "where ReadingID in (select ReadingID " +
                "from " + table + " where CellResistance = 65535)";
            InstantUpdate(10, 18, "Fault3: R is maxed...");
            trySqlCommand(update2, "@BDS40_F3", P_R_MAX);
        }
        private void BDS40_F4()
        {
            //Fault 4 readings occur when a group of cells have maxed resistance
            //A group is defined as 10% of the number of cells on a string

            //Probability of F4 if Resistance is maxed
            float P_R_MAX = 1f;

            //Update database with F4 findings
            InstantUpdate(11, 18, "Fault4: Drop view BDS40F4_GSizes...");
            trySqlCommand("drop view BDS40F4_GSizes");

            string view1 = 
                "create view BDS40F4_GSizes as " +
                "select distinct StringTag, StringNumber, " +
                "(max(CellNumber) / 10) as GSize " +
                "from " + table + " " +
                "group by StringTag, StringNumber; ";
            InstantUpdate(11, 18, "Fault4: Creating view BDS40F4_GSizes...");
            trySqlCommand(view1);

            string update1 =
                "update Faults set BDS40_F4 = @BDS40_F4 " +
                "from Faults A, " +
                "( " +
                    "select A.ReadingID, count(distinct B.ReadingID) as NumAdjCells " +
                    "from " + table + " A, " + table + " B, BDS40F5_GSizes C " +
                    "where " +
                        "A.StringTag = B.StringTag " +
                    "and A.StringTag = C.StringTag " +
                    "and A.StringNumber = B.StringNumber " +
                    "and A.StringNumber = C.StringNumber " +
                    "and A.ReadingDate = B.ReadingDate " +
                    "and B.CellNumber > A.CellNumber " +
                    "and B.CellNumber - A.CellNumber < C.GSize " +
                    "and A.CellResistance = B.CellResistance " +
                    "and A.CellResistance > 50000 " +
                    "group by A.ReadingID, C.GSize " +
                    "having count(distinct B.ReadingID) = C.GSize - 1 " +
                ") B " +
                "where A.ReadingID = B.ReadingID ";

            InstantUpdate(12, 18, "Fault4: Find maxed groups...");
            trySqlCommand(update1, "@BDS40_F4", P_R_MAX);
        }
        private void BDS40_F5()
        {
            //Fault 5 readings occur when two adjacent groups of cells have maxed resistance
            //A group is defined as 10% of the number of cells on a string

            //Probability of F5 if resistance is maxed
            float P_R_MAX = 1f;

            InstantUpdate(13, 18, "Fault5: Drop view BDS40F5_GSizes...");
            trySqlCommand("drop view BDS40F5_GSizes");

            //Update database with F5 findings
            string view1 = 
                "create view BDS40F5_GSizes as " +
                "select distinct StringTag, StringNumber, " +
                "(max(CellNumber) / 20) as GSize " +
                "from " + table + " " +
                "group by StringTag, StringNumber; ";
            InstantUpdate(13, 18, "Fault5: Creating view BDS40F5_GSizes...");
            trySqlCommand(view1);

            string update1 =
                "update Faults set BDS40_F5 = @BDS40_F5 " +
                "from Faults A, " +
                "( " +
                    "select A.ReadingID, count(distinct B.ReadingID) as NumAdjCells " +
                    "from " + table + " A, " + table + " B, BDS40F5_GSizes C " +
                    "where " +
                        "A.StringTag = B.StringTag " +
                    "and A.StringTag = C.StringTag " +
                    "and A.StringNumber = B.StringNumber " +
                    "and A.StringNumber = C.StringNumber " +
                    "and A.ReadingDate = B.ReadingDate " +
                    "and B.CellNumber > A.CellNumber " +
                    "and B.CellNumber - A.CellNumber < C.GSize " +
                    "and A.CellResistance = B.CellResistance " +
                    "and A.CellResistance > 50000 " +
                    "group by A.ReadingID, C.GSize " +
                    "having count(distinct B.ReadingID) = C.GSize - 1 " +
                ") B " +
                "where A.ReadingID = B.ReadingID ";

            InstantUpdate(14, 18, "Fault5: Find maxed groups...");
            trySqlCommand(update1, "@BDS40_F5", P_R_MAX);
        }
        private void BDS40_F6()
        {
            //Probability of F6 if reading frequency changes
            float P_FREQ1 = 1;

            InstantUpdate(15, 18, "Fault6: Drop view TSLRs...");
            trySqlCommand("drop view TSLRs");

            //Update database with F6 findings
            string view1 =
                "create view TSLRs as " +
                "(select C.BDS40_ID as ID, DATEDIFF(day, A.ReadingDate, B.ReadingDate) as TSLR  " +
                "from " + table + " A, " + table + " B, Faults C, Faults D " +
                "where  " +
                    "A.StringTag = B.StringTag " +
                "and A.StringNumber = B.StringNumber " +
                "and A.CellNumber = B.CellNumber " +
                "and A.ReadingID = C.ReadingID " +
                "and B.ReadingID = D.ReadingID " +
                "and C.BDS40_ID = D.BDS40_ID - 1) ";

            InstantUpdate(15, 18, "Fault6: Creating view TSLRs...");
            trySqlCommand(view1);

            string update1 =
                "update Faults set BDS40_F6 = @BDS40_F6 " +
                "from TSLRs A, TSLRs B, Faults C " +
                "where abs(A.TSLR - B.TSLR) > 0 " +
                "and A.ID = B.ID - 1 " +
                "and C.BDS40_ID = A.ID ";


            InstantUpdate(16, 18, "Fault6: Find inconsistent reading frequencies...");
            trySqlCommand(update1, "@BDS40_F6", P_FREQ1);
        }
        private void BDS40_F7()
        {
            //Probability of F7 if criteria met
            float P_MUX = 1;

            //Update database with F7 findings
            string update1 =
                "update Faults set BDS40_F7 = @BDS40_F7 " +
                "from Faults A, " +
                "( " +
                    "select  " +
                      "A.ReadingID as A_ID " +
                    ", B.ReadingID as B_ID " +
                    ", C.ReadingID as C_ID " +
                    ", D.ReadingID as D_ID " +
                    "from " +
                    table + " A " +
                    "," + table + " B " +
                    "," + table + " C " +
                    "," + table + " D " +
                    "where  " +
                        "A.StringTag = B.StringTag " +
                    "and A.StringTag = C.StringTag " +
                    "and A.StringTag = D.StringTag " +
                    "and A.StringNumber = B.StringNumber " +
                    "and A.StringNumber = C.StringNumber " +
                    "and A.StringNumber = D.StringNumber " +
                    "and A.ReadingDate = B.ReadingDate " +
                    "and A.ReadingDate = C.ReadingDate " +
                    "and A.ReadingDate = D.ReadingDate " +
                    "and A.CellNumber = B.CellNumber - 1 " +
                    "and A.CellNumber = C.CellNumber - 2 " +
                    "and A.CellNumber = D.CellNumber - 3 " +
                    "and A.CellVoltage < B.CellVoltage * .97 " +
                    "and A.CelLVoltage < C.CellVoltage * .99 " +
                    "and A.CellVoltage < D.CellVoltage * .97 " +
                    "and C.CellVoltage < B.CellVoltage * .99 " +
                    "and C.CellVoltage < D.CellVoltage * .97 " +
                    "and D.CellVoltage < B.CellVoltage * .99 " +
                ") B " +
                "where  " +
                   "A.ReadingID = B.A_ID " +
                "or A.ReadingID = B.B_ID " +
                "or A.ReadingID = B.C_ID " +
                "or A.ReadingID = B.D_ID ";
            InstantUpdate(17, 18, "Fault7: Find MUX failures...");
            trySqlCommand(update1, "@BDS40_F7", P_MUX);
        }
        private void BDS40_F10()
        {
            //F10 is flagged if StringTag is not a number

            string update1 =
                "update Faults set BDS40_F10 = @BDS40_F10 " +
                "where ReadingID in " +
                "(select ReadingID from " + table + " " +
                "where ISNUMERIC(StringTag) <> 1) ";
            InstantUpdate(18, 18, "Fault10: Find rows with bad syntax...");
            trySqlCommand(update1, "@BDS40_F10", 1);
        }
        #endregion
        #region Cleanse
        private void CleanseButton_Click(object sender, EventArgs e)
        {
            try
            {
                switch (state)
                {
                    case STATE_DB_NONE:
                        BlockControls();
                        CleanseButton.Enabled = true;
                        CleanseButton.Text = "Cancel Cleanse";
                        state = STATE_DB_CLEANSE;
                        CleanseSetupDB();
                        break;
                    case STATE_DB_CANCEL:
                        break;
                    case STATE_DB_CLEANSE:
                        CleanseWorker.CancelAsync();
                        state = STATE_DB_CANCEL;
                        break;
                }
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("Could not cleanse.");
                Global.errorStack.Add(ex.Message);
                Global.GenerateErrors();
            }
        }
        private void CleanseSetupDB()
        {
            status = new StringPair("Retrieving View...", "");

            SelectAll();
            HideColumns(dataGridViewGraph);
            CleanseWorker.RunWorkerAsync();
        }
        private void SelectAll()
        {
            //get new data for the main tab, including error and signature columns
            string select1;
            select1 = "";
            string[] columns = new string[checkedListBoxKeys.CheckedItems.Count];
            if (columns.Count() == 0) throw new Exception("There are no keys selected!");
            checkedListBoxKeys.CheckedItems.CopyTo(columns, 0);
            string[] columnsT = new string[checkedListBoxThresh.CheckedItems.Count];
            checkedListBoxThresh.CheckedItems.CopyTo(columnsT, 0);
            //generate a query string
            //select relevant columns of rows matching the given signature
            select1 = "select distinct ";
            select1 += IndependentColumn + ", ";
            select1 += DependentColumn + ", ";
            for (int c = 0; c < checkedListBoxThresh.CheckedItems.Count; c++)
            {
                string Tkey = checkedListBoxThresh.CheckedItems[c].ToString();
                select1 += Tkey + ", ";
            }
            select1 += "Faults.error from Faults, BatteryReadings";
            select1 += " where Faults.ReadingID = BatteryReadings.ReadingID and ";
            TableFilter filter;
            for (int i = 0; i < columns.Length; i++)
            {
                filter = (TableFilter)flowLayoutPanel1.Controls[i];
                ComboBox box = filter.comboBox1;
                string filterValue = box.Text;
                if (filterValue == "-Refresh-" ||
                    filterValue == "")
                    throw new Exception("Choose key values.");
                string filterColumn = box.Name;
                select1 += filterColumn + " = '" + filterValue + "' and ";
            }
            select1 = select1.Substring(0, select1.Length - 5);
            select1 += " order by " + IndependentColumn; //sort
            using(SqlDataAdapter query = new SqlDataAdapter(select1, connString))
            {
                query.SelectCommand.CommandTimeout = 0;
                dataTable.Clear();
                query.Fill(dataTable); //send query
            }
        }
        private void HideColumns(DataGridView gridView)
        {
            try
            {
                foreach (DataGridViewColumn column in gridView.Columns)
                {
                    if (column.Name == IndependentColumn ||
                        column.Name == DependentColumn)
                        continue;
                    column.Visible = false;
                }
            }
            catch { }
        }
        private void CleanseWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                switch (state)
                {
                    case STATE_DB_CLEANSE:
                        CleanseDB();
                        break;
                }
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("Error while Cleansing Data");
                Global.errorStack.Add(ex.Message);
                switch (state)
                {
                    case STATE_DB_CLEANSE:
                        state = STATE_DB_ERROR;
                        break;
                }
            }
        }
        private void CleanseDB()
        {
            #region status
            status = new StringPair("Cleansing Data...", "");
            int n = dataView.Count;
            int c = 0;
            ProgressUpdate(CleanseWorker, n, c, 1);
            #endregion
            #region Cleanse
            float prevDepValue = 0;
            //for-loop checks first value, so make prevDepValue different
            try { prevDepValue = float.Parse(dataView[0][DependentColumn].ToString()) - 1; }
            catch { throw new Exception("Dependant axis must be a float"); }
            foreach (DataRowView rowView in dataView)
            {
                DataRow row = rowView.Row;
                row["Error"] = "";
                #region Thresholds
                if (CleanseWorker.CancellationPending) return;
                if (checkBoxThresh.Checked)
                {
                    try
                    {
                        foreach (DataRow threshRow in Thresholds.Rows)
                        {
                            //this may be faster with hash tables here
                            foreach (DataColumn threshCol in Thresholds.Columns)
                            {
                                //skip the threshold values columns
                                if (threshCol.ColumnName.Contains(DependentColumn)) continue;
                                //skip threshRow if it doesn't match
                                if (threshRow[threshCol].ToString() != row[threshCol.ToString()].ToString()) goto NextThreshRow;
                            }
                            //If it gets to this point, then the threshold matches
                            //check and flag for threshold error
                            if (float.Parse(row[DependentColumn].ToString()) > float.Parse(threshRow["High " + DependentColumn + " Threshold"].ToString()))
                            {
                                row["Error"] = "Above Threshold";
                                goto NextDataRow;
                            }
                            else if (float.Parse(row[DependentColumn].ToString()) < float.Parse(threshRow["Low " + DependentColumn + " Threshold"].ToString()))
                            {
                                row["Error"] = "Below Threshold";
                                goto NextDataRow;
                            }
                        NextThreshRow: ;
                        }
                    }
                    catch
                    {
                        Global.errorStack.Add("Error while Cleansing Data");
                        Global.errorStack.Add("Make sure all thresholds are set and try again.");
                        state = STATE_DB_ERROR;
                        return;
                    }
                }
                #endregion
                #region Repeated Data
                if (CleanseWorker.CancellationPending) return;
                if (checkBoxRepeat.Checked)
                {
                    float depValue = float.Parse(row[DependentColumn].ToString());
                    if (depValue == prevDepValue)
                    {
                        row["Error"] = "Repeat";
                        goto NextDataRow;
                    }
                    prevDepValue = depValue;
                }
                #endregion
            NextDataRow: ;
                #region status
                c++;
                ProgressUpdate(CleanseWorker, n, c, 3);
                if (CleanseWorker.CancellationPending) return;
                #endregion
            }
            #region Box and Whisker Outliers
            if (checkBoxOutliers.Checked)
            {
                status = new StringPair("Sorting...", "");
                ProgressUpdate(CleanseWorker, 1, 1, 1);
                if (CleanseWorker.CancellationPending) return;
                dataView.Sort = DependentColumn;
                OutliersDB(dataView);
            }
            #endregion
            #endregion
            #region Update database
            status = new StringPair("Updating Database...", "");
            ProgressUpdate(CleanseWorker, 1, 1, 1);
            try
            {
                GetSchemaTable();
                UpdateDBCleanse();
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("Problem updating error column in database!");
                Global.errorStack.Add(ex.Message);
                state = STATE_DB_ERROR;
            }
            #endregion
        }
        private void UpdateDBCleanse()
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                //this block creates an update query that finds and updates the error column
                //in the database where they were updated in the local dataTable
                string update1 = "update Faults set error = @error ";
                update1 += " from Faults, BatteryReadings ";
                update1 += sqlSigWhereClause;
                update1 += " and Faults.ReadingID = BatteryReadings.ReadingID";
                update1 += " and " + IndependentColumn + " = @" + IndependentColumn;
                SqlCommand updateCommand = new SqlCommand(update1, connection);
                updateCommand.Parameters.Add("@error", SqlDbType.VarChar, 20, "error");
                updateCommand.Parameters.Add("@" + IndependentColumn, IndependentColumnType, IndependentColumnSize, IndependentColumn); //IWASHERE
                SqlDataAdapter updateq = new SqlDataAdapter();
                updateCommand.CommandTimeout = 0;
                updateq.UpdateCommand = updateCommand;
                updateq.Update(dataTable); //send query
            }
        }
        private void GetSchemaTable()
        {
            if (IndependentColumn == "" || DependentColumn == "")
            {
                throw new Exception("Choose keys for Independant and Dependant axis");
            }
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                string qstring = "SET FMTONLY ON; select " + IndependentColumn + ", " + DependentColumn + " from " + table + "; SET FMTONLY OFF";
                using (SqlCommand cmd = new SqlCommand(qstring, connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    SchemaTable = reader.GetSchemaTable();
                }
            }
        }
        private void OutliersDB(DataView view)
        {
            try
            {
                #region status
                status = new StringPair("Outlier Test...", "");
                int n = view.Count;
                int c = 0;
                ProgressUpdate(CleanseWorker, n, c, 1);
                #endregion
                //Find the quartiles and the inner quartile range
                float Q2 = float.Parse(view[(n + 1) / 2][DependentColumn].ToString());
                float Q1 = float.Parse(view[(n + 1) / 4][DependentColumn].ToString());
                float Q3 = float.Parse(view[(n + 1) / 4 + (n + 1) / 2][DependentColumn].ToString());
                float IQR = Q3 - Q1;
                //Remove values outside extreme test.
                float top = Q3 + 3 * IQR;
                float bottom = Q1 - 3 * IQR;
                foreach (DataRowView rowView in view)
                {
                    DataRow row = rowView.Row;
                    float value = float.Parse(row[DependentColumn].ToString());
                    if (value > top)
                    {
                        row["Error"] = "High Outlier";
                    }
                    if (value < bottom)
                    {
                        row["Error"] = "Low Outlier";
                    }
                    c++;
                    ProgressUpdate(CleanseWorker, n, c, 3);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Specified cast is not valid"))
                {
                    throw new Exception("Dependant axis value must be a float.");
                }
                throw new Exception("Not enough rows to complete outlier test.\nCheck that you have the correct axis and keys chosen.");
            }
        }
        private void Outliers(DataView view)
        {
            try
            {
                int n = view.Count;
                //Find the quartiles and the inner quartile range
                float Q2 = (float)view[(n + 1) / 2][DependentColumn];
                float Q1 = (float)view[(n + 1) / 4][DependentColumn];
                float Q3 = (float)view[(n + 1) / 4 + (n + 1) / 2][DependentColumn];
                float IQR = Q3 - Q1;
                //Remove values outside extreme test.
                float top = Q3 + 3 * IQR;
                float bottom = Q1 - 3 * IQR;
                foreach (DataRowView rowView in view)
                {
                    DataRow row = rowView.Row;
                    float value = (float)row[DependentColumn];
                    if (value > top)
                    {
                        row["Error"] = "High Outlier";
                        errorTable.ImportRow(row);
                    }
                    if (value < bottom)
                    {
                        row["Error"] = "Low Outlier";
                        errorTable.ImportRow(row);
                    }
                }
            }
            catch
            {
                throw new Exception("Not enough rows to complete outlier test.\nCheck that you have the correct axis and keys chosen.");
            }
        }
        private void CleanseWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UnBlockControls();
            CleanseButton.Text = "Cleanse";
            switch (state)
            {
                case STATE_DB_NONE:
                    break;
                case STATE_DB_ERROR:
                    state = STATE_DB_NONE;
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = "Error.";
                    toolStripStatusLabel2.Text = "";
                    Global.GenerateErrors();
                    FilterViewDB();
                    break;
                case STATE_DB_CLEANSE:
                    state = STATE_DB_NONE;
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = "Ready.";
                    toolStripStatusLabel2.Text = "";
                    Title3 = "Step 1: Cleansed Data";
                    Step2Button.Enabled = true;
                    FilterViewDB();
                    break;
                case STATE_DB_CANCEL:
                    state = STATE_DB_NONE;
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = "Cancelled.";
                    toolStripStatusLabel2.Text = "";
                    FilterViewDB();
                    break;
            }
        }
        #endregion
        #region Step 2
        private void button2_Click(object sender, EventArgs e)
        {
            switch (state)
            {
                case STATE_DB_NONE:
                    BlockControls();
                    Step2Button.Enabled = true;
                    Step2Button.Text = "Cancel Step2";
                    state = STATE_DB_CLEANSE;
                    Step2SetupDB();
                    break;
                case STATE_DB_CLEANSE:
                    Step2Worker.CancelAsync();
                    state = STATE_DB_CANCEL;
                    break;
            }
        }
        private void Step2SetupDB()
        {
            try
            {
                status = new StringPair("Retrieving View...", "");

                //get new data for the step2 tab, including signature columns
                string select1;
                select1 = "";
                string[] columns = new string[checkedListBoxKeys.CheckedItems.Count];
                if (columns.Count() == 0) return;
                checkedListBoxKeys.CheckedItems.CopyTo(columns, 0);
                //generate a query string
                //select relevant columns of rows matching the given signature
                select1 = "select distinct ";
                select1 += IndependentColumn + ", ";
                select1 += DependentColumn + ", ";
                select1 += "RegStat from Faults, BatteryReadings";
                select1 += " where Faults.ReadingID = BatteryReadings.ReadingID and ";
                TableFilter filter;
                for (int i = 0; i < columns.Length; i++)
                {
                    filter = (TableFilter)flowLayoutPanel1.Controls[i];
                    ComboBox box = filter.comboBox1;
                    string filterValue = box.Text;
                    if (filterValue == "-Refresh-" ||
                        filterValue == "")
                        throw new Exception("Choose key values.");
                    string filterColumn = box.Name;
                    select1 += filterColumn + " = '" + filterValue + "' and ";
                }
                select1 += "error = ''";
                select1 += " order by " + IndependentColumn; //sort
                SqlDataAdapter query = new SqlDataAdapter(select1, connString);
                query.SelectCommand.CommandTimeout = 0;
                step2Table.Clear();
                query.Fill(step2Table); //send query

                HideColumns(dataGridViewStep2);
                try { dataGridViewStep2.Columns["RegStat"].Visible = true; }
                catch { }

                Step2Worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("There was a problem with Step 2!");
                Global.errorStack.Add(ex.Message);
            }
        }
        private void Step2Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (state)
            {
                case STATE_DB_CLEANSE:
                    step2ProcTable = step2Table.DefaultView.ToTable();
                    step2ProcTable.Columns.Remove("RegStat");
                    SaveTable("a.csv", step2ProcTable);
                    RunStep2Process();
                    break;
            }
            switch (state)
            {
                case STATE_DB_CLEANSE:
                    Step2UpdateDB();
                    break;
            }
        }
        private void RunStep2Process()
        {
            try
            {
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = "Running Step 2...";
                toolStripStatusLabel2.Text = "";
                var p = new Process();
                p.StartInfo.FileName = Application.StartupPath + @"\step_2_redistributable\step_2_prototype.exe";
                p.StartInfo.Arguments = "-i a.csv -x \"" + IndependentColumn + "\" -y \"" +
                    DependentColumn + "\" ";
                if (dataTable.Columns[IndependentColumn].DataType == typeof(DateTime))
                    p.StartInfo.Arguments += "-d \"" + IndependentColumn + "\" -D \"%Y-%m-%d %H:%M:%S\"";
                p.StartInfo.Arguments += " -o \"" + Application.StartupPath + "\\b.csv\"";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.Start();
                p.WaitForExit();

                switch (state)
                {
                    case STATE_DB_CLEANSE:
                        step2ProcTable = step2Table.Clone();
                        step2ProcTable.Columns["RegStat"].ColumnName = DependentColumn + "_error";
                        OpenTable("b.csv", ref step2ProcTable);
                        step2ProcTable.Columns[DependentColumn + "_error"].ColumnName = "RegStat";
                        break;
                }
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("Error during Step 2");
                Global.errorStack.Add(ex.Message + "\nMake sure the Step 2 redistributable package \nis placed in the same folder as prototype.exe.");
                switch (state)
                {
                    case STATE_DB_CLEANSE:
                        state = STATE_DB_ERROR;
                        break;
                }
            }
        }
        private void Step2UpdateDB()
        {
            status = new StringPair("Updating Database...", "");
            ProgressUpdate(Step2Worker, 1, 1, 1);

            try
            {
                GetSchemaTable();

                for (int i = 0; i < step2Table.Rows.Count; i++)
                {
                    step2Table.Rows[i]["RegStat"] = step2ProcTable.Rows[i]["RegStat"];
                }

                //debugDumpC();
                //debugDumpD();

                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();
                    //step2Table only has a subset of the selected jar. I need to clear the RegStat
                    //column for every row of the jar before updating the subset.
                    string update0 = "update Faults set RegStat = -1 ";
                    update0 += " from Faults, BatteryReadings ";
                    update0 += sqlSigWhereClause;
                    update0 += " and Faults.ReadingID = BatteryReadings.ReadingID";
                    SqlCommand updateCommand0 = new SqlCommand(update0, connection);
                    updateCommand0.ExecuteNonQuery();

                    //this block creates an update query that finds and updates the error column
                    //in the database where they were updated in the local dataTable
                    string update1 = "update Faults set RegStat = @RegStat ";
                    update1 += " from Faults, BatteryReadings ";
                    update1 += sqlSigWhereClause;
                    update1 += " and Faults.ReadingID = BatteryReadings.ReadingID";
                    update1 += " and " + IndependentColumn + " = @" + IndependentColumn;
                    SqlCommand updateCommand = new SqlCommand(update1, connection);
                    updateCommand.Parameters.Add("@RegStat", SqlDbType.Float, 5, "RegStat");
                    updateCommand.Parameters.Add("@" + IndependentColumn, IndependentColumnType, IndependentColumnSize, IndependentColumn); //IWASHERE
                    SqlDataAdapter updateq = new SqlDataAdapter();
                    updateCommand.CommandTimeout = 0;
                    updateq.UpdateCommand = updateCommand;
                    updateq.Update(step2Table); //send query
                }
            }
            catch (Exception ex)
            {
                Global.errorStack.Add("Problem updating RegStat column in database!");
                Global.errorStack.Add(ex.Message);
                state = STATE_DB_ERROR;
            }
        }
        private void debugDumpC()
        {
            DataTable dump = step2Table.DefaultView.ToTable();
            dump.Columns.Remove("RegStat");
            SaveTable("c.csv", dump); //dump the to-be-uploaded table for debugging
        }
        private void debugDumpD()
        {
            DataTable debugTable = new DataTable();
            string select1 = "select ";
            foreach (object item in flowLayoutPanel1.Controls)
            {
                string filterColumn = ((TableFilter)item).comboBox1.Name;
                select1 += filterColumn + ", ";
            }
            select1 += IndependentColumn + ", " + DependentColumn;
            select1 += " from " + table + " " + sqlSigWhereClause;
            select1 += "order by " + IndependentColumn;

            SqlDataAdapter selectq = new SqlDataAdapter(select1, connString);
            selectq.SelectCommand.CommandTimeout = 0;
            selectq.Fill(debugTable);
            SaveTable("d.csv", debugTable);
        }
        public void SaveTable(string filename, DataTable sdt)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create)))
            {
                string stuff = "";
                foreach (DataColumn column in sdt.Columns)
                {
                    stuff += column.ColumnName + ",";
                }
                writer.WriteLine(stuff);
                int c = 0;
                foreach (DataRow row in sdt.Rows)
                {
                    c++;
                    stuff = "";
                    foreach (DataColumn column in sdt.Columns)
                    {
                        if (column.DataType == typeof(DateTime))
                        {
                            //stuff += ((DateTime)row[column]).ToString("MM/dd/yyyy") + ",";
                            stuff += ((DateTime)row[column]).ToString("yyyy-MM-dd HH:mm:ss") + ",";
                        }
                        else
                        {
                            stuff += row[column].ToString() + ",";
                        }
                    }
                    writer.WriteLine(stuff);
                }
            }
        }
        DataTable OpenTable(string filename)
        {
            DataTable data = new DataTable();
            List<string> lines = new List<string>();
            using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    //skip blank lines
                    if (line == "") continue;
                    //delete blank columns
                    line = line.Replace(",,", ",");
                    //fix n/a values to 0
                    line = line.Replace("n/a", "0");
                    lines.Add(line);
                }
            }
            List<string> headers = lines[0].Split(',').ToList(); //headers to list
            List<string> lineOne = lines[1].Split(',').ToList(); //look at first row for data types
            status = new StringPair("Opening File...", "");

            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];
                object item;
                data.Columns.Add(header, Global.parseForType(lineOne[i], out item));
            }
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                try
                {
                    List<string> row = line.Split(',').ToList();
                    DataRow dataRow = data.NewRow();
                    int j = 0;
                    foreach (DataColumn column in data.Columns)
                    {
                        object o;
                        Type T = Global.parseForType(row[j], out o);
                        dataRow.SetField(column, o);
                        j++;
                    }
                    data.Rows.Add(dataRow);
                    //Bad data rows will be omitted because they will throw exceptions with dataRow.SetField
                }
                catch { }
            }
            return data;
        }
        void OpenTable(string filename, ref DataTable data)
        {
            List<string> lines = new List<string>();
            using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    //skip blank lines
                    if (line == "") continue;
                    //delete blank columns
                    line = line.Replace(",,", ",");
                    //fix n/a values to 0
                    line = line.Replace("n/a", "0");
                    lines.Add(line);
                }
            }
            List<string> headers = lines[0].Split(',').ToList(); //headers to list
            status = new StringPair("Opening File...", "");

            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];
                if (!data.Columns.Contains(header))
                    throw new Exception("The given DataTable does not have a column" +
                        " named " + header + "!");
            }
            foreach (string line in lines)
            {
                try
                {
                    data.Rows.Add(line.Split(',').ToArray());
                }
                catch { }
            }
        }
        private void Step2Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UnBlockControls();
            Step2Button.Text = "Run Step 2";
            switch (state)
            {
                case STATE_DB_CLEANSE:
                    state = STATE_DB_NONE;
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = "Ready.";
                    toolStripStatusLabel2.Text = "";
                    Title3 = "Step 2: Error Detection";
                    FilterViewDB();
                    break;
                case STATE_DB_ERROR:
                    state = STATE_DB_NONE;
                    Global.GenerateErrors();
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = "Error.";
                    toolStripStatusLabel2.Text = "";
                    FilterViewDB();
                    break;
                case STATE_DB_CANCEL:
                    state = STATE_DB_NONE;
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = "Cancelled.";
                    toolStripStatusLabel2.Text = "";
                    FilterViewDB();
                    break;
            }
        }
        #endregion
        #region UI Stuff
        Stopwatch timer = new Stopwatch();
        private void StopwatchWorkerStart()
        {
            timer.Restart();
            if (!StopwatchWorker.IsBusy) StopwatchWorker.RunWorkerAsync();
        }
        private void StopwatchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!StopwatchWorker.CancellationPending)
            {
                string t = String.Format("{0:00}:{1:00}:{2:00}",
                    timer.Elapsed.Hours, timer.Elapsed.Minutes, timer.Elapsed.Seconds);
                Invoke(new Action(() => toolStripStatusLabel3.Text = t));
            }
        }
        private void StopwatchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer.Stop();
        }
        private void radioAll_CheckedChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }
        private void radioSelected_CheckedChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }
        private void dataGridViewThresh_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewThresh.Rows[e.RowIndex].ErrorText = String.Empty;
        }
        private void dataGridViewThresh_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string headerText =
                dataGridViewThresh.Columns[e.ColumnIndex].HeaderText;

            if (!headerText.Contains(DependentColumn)) return;

            float trash;
            if (e.FormattedValue.ToString() != "" && !float.TryParse(e.FormattedValue.ToString(), out trash))
            {
                dataGridViewThresh.Rows[e.RowIndex].ErrorText =
                    "Must be a Number";
                e.Cancel = true;
            }
        }
        private void BlockControls()
        {
            groupBoxThresh.Enabled = false;
            flowLayoutPanel1.Enabled = false;
            tableLayoutPanelKeys.Enabled = false;
            Step2Button.Enabled = false;
            Graph2Button.Enabled = false;
            groupBoxCOpt1.Enabled = false;
            Graph2Button.Enabled = false;
            GraphButton.Enabled = false;
            Step2Button.Enabled = false;
            CleanseButton.Enabled = false;
            buttonBDS40.Enabled = false;
        }
        private void UnBlockControls()
        {
            buttonBDS40.Enabled = true;
            CleanseButton.Enabled = true;
            groupBoxThresh.Enabled = true;
            flowLayoutPanel1.Enabled = true;
            tableLayoutPanelKeys.Enabled = true;
            groupBoxCOpt1.Enabled = true;
            if (step2Table.Rows.Count > 0) Graph2Button.Enabled = true;
            GraphButton.Enabled = true;
            Step2Button.Enabled = true;
        }
        private void EnableButtons()
        {
            bool enable = false;
            if (checkedListBoxKeys.CheckedItems.Count == 0)
            {
                label1.Text = "Choose Keys.";
                enable = false;
            }
            else if(checkedListBoxInd.CheckedItems.Count == 1 && 
                checkedListBoxDep.CheckedItems.Count == 1)
            {
                try
                {
                    enable = true;
                    if ( flowLayoutPanel1.Controls.Count > 0)
                    {
                        Title = "";
                        foreach (TableFilter filter in flowLayoutPanel1.Controls)
                        {
                            if (filter.comboBox1.Text == "-All-" ||
                                filter.comboBox1.Text == "-Refresh-" ||
                                filter.comboBox1.Text == "")
                            {
                                label1.Text = "Choose Key Values.";
                                enable = false;
                                break;
                            }
                            Title += filter.comboBox1.Text + ", ";
                        }
                        Title = Title.Substring(0, Title.Length - 2);
                        Title = "(" + Title + ") ";
                        Title2 = DependentColumn + " vs. " + IndependentColumn;
                        Text = "Data Viewer - " + Title + " - " + Title2 + " - " + Title3;
                    }
                }
                catch
                {
                    enable = false;
                }
            }
            else
            {
                label1.Text = "Choose X and Y columns.";
                enable = false;
                return;
            }
            if (enable)
            {
                label1.Text = "Ready to graph!";
                GraphButton.Enabled = true;
                if (step2Table.Rows.Count > 0) Graph2Button.Enabled = true;
                buttonBDS40.Enabled = true;
                CleanseButton.Enabled = true;
                Step2Button.Enabled = true;
                checkBoxRepeat.Enabled = true;
                checkBoxRepeat.Checked = true;
                checkBoxOutliers.Enabled = true;
                checkBoxOutliers.Checked = true;
                checkBoxThresh.Enabled = true;
            }
            else
            {
                Text = "Data Viewer";
                GraphButton.Enabled = false;
                buttonBDS40.Enabled = false;
                Graph2Button.Enabled = false;
                CleanseButton.Enabled = false;
                Step2Button.Enabled = false;
                checkBoxRepeat.Enabled = false;
                checkBoxRepeat.Checked = false;
                checkBoxOutliers.Enabled = false;
                checkBoxOutliers.Checked = false;
                checkBoxThresh.Enabled = false;
            }
        }
        private void Graph2Button_Click(object sender, EventArgs e)
        {
            Title3 = "Step 2: Regression Statistic";
            tabControl2.SelectTab("tabStep2");
            if (Step2ButtonClicked != null) Step2ButtonClicked(sender, e);
        }
        private void GraphButton_Click(object sender, EventArgs e)
        {
            Title3 = "Step 1: Cleansed Data";
            tabControl2.SelectTab("tabGraphData");
            if (CreateDatasetsButtonClicked != null) CreateDatasetsButtonClicked(sender, e);
        }
        private void DataViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void checkBoxThresh_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxThresh.Checked) isCleansed = false;
        }
        private void checkBoxRepeat_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRepeat.Checked) isCleansed = false;
        }
        private void checkBoxOutliers_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOutliers.Checked) isCleansed = false;
        }
        private void checkedListBoxDep_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //causes the checked box list to behave like radio buttons
            //when a box is checked, it is removed from the other lists

            //This checkbox may only choose one value  
            if (e.NewValue == CheckState.Checked)
            {
                try
                {
                    //try to uncheck whatever was checked before
                    checkedListBoxInd.Items.Add(checkedListBoxDep.CheckedItems[0]);
                    checkedListBoxKeys.Items.Add(checkedListBoxDep.CheckedItems[0]);
                    checkedListBoxDep.SetItemChecked(checkedListBoxDep.CheckedIndices[0], false);
                }
                catch { }

                object item = checkedListBoxDep.Items[e.Index];
                checkedListBoxInd.Items.Remove(item);
                checkedListBoxKeys.Items.Remove(item);
            }
            else if (checkedListBoxDep.SelectedIndex == e.Index)
            {
                //prevent unchecking
                e.NewValue = CheckState.Checked;
            }
        }
        private void checkedListBoxInd_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //causes the checked box list to behave like radio buttons
            //when a box is checked, it is removed from the other lists

            //This checkbox may only choose one value  
            if (e.NewValue == CheckState.Checked)
            {
                try
                {
                    //try to uncheck whatever was checked before
                    AddItemtoDepCheckList(checkedListBoxInd.CheckedItems[0]);
                    checkedListBoxKeys.Items.Add(checkedListBoxInd.CheckedItems[0]);
                    checkedListBoxInd.SetItemChecked(checkedListBoxInd.CheckedIndices[0], false);
                }
                catch { }

                object item = checkedListBoxInd.Items[e.Index];
                checkedListBoxDep.Items.Remove(item);
                checkedListBoxKeys.Items.Remove(item);
            }
            else if (checkedListBoxInd.SelectedIndex == e.Index)
            {
                e.NewValue = CheckState.Checked;
            }
        }
        private void AddItemtoDepCheckList(object item)
        {
            switch (state)
            {
                case STATE_DB_NONE:
                    if (getColumnDataType((string)item) == "float")
                    {
                        checkedListBoxDep.Items.Add(item, false);
                    }
                    break;
            }
        }
        private string getColumnDataType(string column)
        {
            try
            {
                var result =
                    from row in ColumnsTypes.AsEnumerable()
                    where row.Field<object>("COLUMN_NAME").ToString() == column
                    select row.Field<object>("DATA_TYPE");
                return result.ToArray()[0].ToString();
            }
            catch
            {
                return "COL_NOT_FOUND";
            }
        }
        public void ProgressUpdate(BackgroundWorker worker, int n, int i, int delay)
        {
            if (i % delay == 0)
            {
                status.s2 = i + "/" + n;
                percent = (int)((double)i / n * 100);
                worker.ReportProgress(percent, status);
            }
        }
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
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
        private void tabControl2_Selected(object sender, TabControlEventArgs e)
        {
            switch (state)
            {
                case STATE_DB_NONE:
                    FilterViewDB();
                    break;
            }
        }
        private void checkedListBoxKeys_MouseLeave(object sender, EventArgs e)
        {
            SetupFilters();
        }
        private void checkedListBoxInd_MouseLeave(object sender, EventArgs e)
        {
            if (IndependentAxisCheckChanged != null) IndependentAxisCheckChanged(sender, e);
            isCleansed = false;
            EnableButtons();
        }
        private void checkedListBoxDep_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                if (DependentAxisCheckChanged != null) DependentAxisCheckChanged(sender, e);
                isCleansed = false;
                EnableButtons();
                switch (state)
                {
                    case STATE_DB_NONE:
                        FilterViewDB();
                        break;
                }
            }
            catch { }
        }
        #endregion
    }
}