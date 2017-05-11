using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace prototype
{
    public partial class TableFilter : UserControl
    {
        //static options
        public static bool suppressWarning = false;

        public EventHandler boxTextChanged;
        private string key;
        private DataView tableView;

        public int state;
        public const int STATE_REFRESH = 0;
        public const int STATE_READY = 1;
        public TableFilter()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Setup a row filter for a data table.
        /// </summary>
        /// <param name="column">The column to filter.</param>
        /// <param name="table">The table to filter.</param>
        public void Setup(string column, ref DataTable table)
        {
            this.tableView = table.DefaultView;
            key = column;
            label1.Text = key;
            comboBox1.Name = key;
            Name = key;
            comboBox1.Text = "-Refresh-";
            comboBox1.SelectedIndexChanged += comboBox1_TextUpdate;
        }
        public void RefreshList()
        {
            switch (state)
            {
                case STATE_READY:
                    break;
                case STATE_REFRESH:
                    GetList();
                    state = STATE_READY;
                    break;
            }
        }
        private void GetList()
        {
            try
            {
                var values =
                    from row in tableView.ToTable().AsEnumerable()
                    select row.Field<object>(key);
                List<object> vals = new List<object>(values.Distinct());
                vals.Sort();
                comboBox1.Items.AddRange(vals.ToArray()); //drop down list
            }
            catch// (Exception ex)
            {
                //good place to show warning to refresh again
            }
        }
        #region Events
        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            if (boxTextChanged != null) boxTextChanged(sender, e);
        }
        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            RefreshList();
        }
        #endregion
    }
}
