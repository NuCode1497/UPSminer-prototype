namespace prototype
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelFILLER = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.SaveFileWorker = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.saveGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dateCoordinatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onCursorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimalGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDataTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.OpenButton = new System.Windows.Forms.ToolStripButton();
            this.SaveButton = new System.Windows.Forms.ToolStripButton();
            this.dataViewerButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MoveButton = new System.Windows.Forms.ToolStripButton();
            this.HorizontalShrinkButton = new System.Windows.Forms.ToolStripButton();
            this.HorizonalStretchButton = new System.Windows.Forms.ToolStripButton();
            this.VerticalShrinkButton = new System.Windows.Forms.ToolStripButton();
            this.VerticalStretchButton = new System.Windows.Forms.ToolStripButton();
            this.FullButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.ZoomLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.OpenFileWorker = new System.ComponentModel.BackgroundWorker();
            this.graph1 = new prototype.Graph();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabelFILLER,
            this.toolStripStatusLabel3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 568);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(864, 22);
            this.statusStrip1.TabIndex = 20;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(150, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // toolStripStatusLabelFILLER
            // 
            this.toolStripStatusLabelFILLER.Name = "toolStripStatusLabelFILLER";
            this.toolStripStatusLabelFILLER.Size = new System.Drawing.Size(343, 17);
            this.toolStripStatusLabelFILLER.Spring = true;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            // 
            // SaveFileWorker
            // 
            this.SaveFileWorker.WorkerReportsProgress = true;
            this.SaveFileWorker.WorkerSupportsCancellation = true;
            this.SaveFileWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.SaveFileWorker_DoWork);
            this.SaveFileWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.SaveFileWorker_ProgressChanged);
            this.SaveFileWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.SaveFileWorker_RunWorkerCompleted);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(864, 24);
            this.menuStrip1.TabIndex = 24;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToDatabaseToolStripMenuItem,
            this.newProjectToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.saveProjectAsToolStripMenuItem,
            this.toolStripSeparator6,
            this.saveGraphToolStripMenuItem,
            this.saveCSVToolStripMenuItem,
            this.saveAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // connectToDatabaseToolStripMenuItem
            // 
            this.connectToDatabaseToolStripMenuItem.Name = "connectToDatabaseToolStripMenuItem";
            this.connectToDatabaseToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.connectToDatabaseToolStripMenuItem.Text = "Connect to Database...";
            this.connectToDatabaseToolStripMenuItem.Click += new System.EventHandler(this.ConnectToolStripMenuItem_Click);
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Enabled = false;
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.newProjectToolStripMenuItem.Text = "New Project...";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Enabled = false;
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.openProjectToolStripMenuItem.Text = "Open Project...";
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Enabled = false;
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveProjectToolStripMenuItem.Text = "Save Project";
            // 
            // saveProjectAsToolStripMenuItem
            // 
            this.saveProjectAsToolStripMenuItem.Enabled = false;
            this.saveProjectAsToolStripMenuItem.Name = "saveProjectAsToolStripMenuItem";
            this.saveProjectAsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveProjectAsToolStripMenuItem.Text = "Save Project As...";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(190, 6);
            // 
            // saveGraphToolStripMenuItem
            // 
            this.saveGraphToolStripMenuItem.Name = "saveGraphToolStripMenuItem";
            this.saveGraphToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveGraphToolStripMenuItem.Text = "Save Screenshot";
            this.saveGraphToolStripMenuItem.Click += new System.EventHandler(this.saveGraphToolStripMenuItem_Click);
            // 
            // saveCSVToolStripMenuItem
            // 
            this.saveCSVToolStripMenuItem.Enabled = false;
            this.saveCSVToolStripMenuItem.Name = "saveCSVToolStripMenuItem";
            this.saveCSVToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveCSVToolStripMenuItem.Text = "Save Graph Data";
            this.saveCSVToolStripMenuItem.Click += new System.EventHandler(this.SaveFileButton_Click);
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Enabled = false;
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveAllToolStripMenuItem.Text = "Save All Data";
            this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(190, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullGraphToolStripMenuItem,
            this.dateCoordinatesToolStripMenuItem,
            this.onCursorToolStripMenuItem,
            this.showGridToolStripMenuItem,
            this.minimalGridToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // fullGraphToolStripMenuItem
            // 
            this.fullGraphToolStripMenuItem.Name = "fullGraphToolStripMenuItem";
            this.fullGraphToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.fullGraphToolStripMenuItem.Text = "Full Graph";
            this.fullGraphToolStripMenuItem.Click += new System.EventHandler(this.fullGraphToolStripMenuItem_Click);
            // 
            // dateCoordinatesToolStripMenuItem
            // 
            this.dateCoordinatesToolStripMenuItem.Checked = true;
            this.dateCoordinatesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dateCoordinatesToolStripMenuItem.Name = "dateCoordinatesToolStripMenuItem";
            this.dateCoordinatesToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.dateCoordinatesToolStripMenuItem.Text = "Date Coordinates";
            this.dateCoordinatesToolStripMenuItem.Click += new System.EventHandler(this.dateCoordinatesToolStripMenuItem_Click);
            // 
            // onCursorToolStripMenuItem
            // 
            this.onCursorToolStripMenuItem.Checked = true;
            this.onCursorToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.onCursorToolStripMenuItem.Name = "onCursorToolStripMenuItem";
            this.onCursorToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.onCursorToolStripMenuItem.Text = "On Cursor";
            this.onCursorToolStripMenuItem.Click += new System.EventHandler(this.onCursorToolStripMenuItem_Click);
            // 
            // showGridToolStripMenuItem
            // 
            this.showGridToolStripMenuItem.Checked = true;
            this.showGridToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGridToolStripMenuItem.Name = "showGridToolStripMenuItem";
            this.showGridToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.showGridToolStripMenuItem.Text = "Show Grid";
            this.showGridToolStripMenuItem.Click += new System.EventHandler(this.showGridToolStripMenuItem_Click);
            // 
            // minimalGridToolStripMenuItem
            // 
            this.minimalGridToolStripMenuItem.Name = "minimalGridToolStripMenuItem";
            this.minimalGridToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.minimalGridToolStripMenuItem.Text = "Minimal Grid";
            this.minimalGridToolStripMenuItem.Click += new System.EventHandler(this.minimalGridToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showDataTableToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // showDataTableToolStripMenuItem
            // 
            this.showDataTableToolStripMenuItem.Name = "showDataTableToolStripMenuItem";
            this.showDataTableToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.showDataTableToolStripMenuItem.Text = "Show Data Table";
            this.showDataTableToolStripMenuItem.Click += new System.EventHandler(this.showDataTableToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenButton,
            this.SaveButton,
            this.dataViewerButton,
            this.toolStripSeparator3,
            this.MoveButton,
            this.HorizontalShrinkButton,
            this.HorizonalStretchButton,
            this.VerticalShrinkButton,
            this.VerticalStretchButton,
            this.FullButton,
            this.toolStripSeparator5,
            this.toolStripLabel1,
            this.ZoomLabel,
            this.toolStripSeparator2,
            this.toolStripLabel3,
            this.toolStripButton8,
            this.toolStripLabel2,
            this.toolStripComboBox1,
            this.toolStripLabel4,
            this.toolStripTextBox1,
            this.toolStripSeparator4,
            this.toolStripLabel5,
            this.toolStripTextBox2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(864, 25);
            this.toolStrip1.TabIndex = 25;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // OpenButton
            // 
            this.OpenButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OpenButton.Image = global::prototype.Properties.Resources.open;
            this.OpenButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(23, 22);
            this.OpenButton.Text = "Open File";
            this.OpenButton.Click += new System.EventHandler(this.ConnectToolStripMenuItem_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveButton.Enabled = false;
            this.SaveButton.Image = global::prototype.Properties.Resources.save;
            this.SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(23, 22);
            this.SaveButton.Text = "Save File";
            this.SaveButton.Click += new System.EventHandler(this.SaveFileButton_Click);
            // 
            // dataViewerButton
            // 
            this.dataViewerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.dataViewerButton.Enabled = false;
            this.dataViewerButton.Image = ((System.Drawing.Image)(resources.GetObject("dataViewerButton.Image")));
            this.dataViewerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.dataViewerButton.Name = "dataViewerButton";
            this.dataViewerButton.Size = new System.Drawing.Size(23, 22);
            this.dataViewerButton.Text = "Table View";
            this.dataViewerButton.Click += new System.EventHandler(this.showDataTableToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // MoveButton
            // 
            this.MoveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveButton.Image = global::prototype.Properties.Resources.Cross;
            this.MoveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MoveButton.Name = "MoveButton";
            this.MoveButton.Size = new System.Drawing.Size(23, 22);
            this.MoveButton.Text = "Toggle Move";
            this.MoveButton.Click += new System.EventHandler(this.MoveButton_Click);
            // 
            // HorizontalShrinkButton
            // 
            this.HorizontalShrinkButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.HorizontalShrinkButton.Image = global::prototype.Properties.Resources.Horizontal_In;
            this.HorizontalShrinkButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.HorizontalShrinkButton.Name = "HorizontalShrinkButton";
            this.HorizontalShrinkButton.Size = new System.Drawing.Size(23, 22);
            this.HorizontalShrinkButton.Text = "Horizontal Shrink";
            this.HorizontalShrinkButton.Click += new System.EventHandler(this.HorizontalShrinkButton_Click);
            // 
            // HorizonalStretchButton
            // 
            this.HorizonalStretchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.HorizonalStretchButton.Image = global::prototype.Properties.Resources.Horizontal_Out;
            this.HorizonalStretchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.HorizonalStretchButton.Name = "HorizonalStretchButton";
            this.HorizonalStretchButton.Size = new System.Drawing.Size(23, 22);
            this.HorizonalStretchButton.Text = "Horizontal Stretch";
            this.HorizonalStretchButton.Click += new System.EventHandler(this.HorizonalStretchButton_Click);
            // 
            // VerticalShrinkButton
            // 
            this.VerticalShrinkButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.VerticalShrinkButton.Image = global::prototype.Properties.Resources.Vertical_In;
            this.VerticalShrinkButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.VerticalShrinkButton.Name = "VerticalShrinkButton";
            this.VerticalShrinkButton.Size = new System.Drawing.Size(23, 22);
            this.VerticalShrinkButton.Text = "Vertical Shrink";
            this.VerticalShrinkButton.Click += new System.EventHandler(this.VerticalShrinkButton_Click);
            // 
            // VerticalStretchButton
            // 
            this.VerticalStretchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.VerticalStretchButton.Image = global::prototype.Properties.Resources.Vertical_Out;
            this.VerticalStretchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.VerticalStretchButton.Name = "VerticalStretchButton";
            this.VerticalStretchButton.Size = new System.Drawing.Size(23, 22);
            this.VerticalStretchButton.Text = "Vertical Stretch";
            this.VerticalStretchButton.Click += new System.EventHandler(this.VerticalStretchButton_Click);
            // 
            // FullButton
            // 
            this.FullButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.FullButton.Image = global::prototype.Properties.Resources.Full;
            this.FullButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FullButton.Name = "FullButton";
            this.FullButton.Size = new System.Drawing.Size(23, 22);
            this.FullButton.Text = "Show Full Graph";
            this.FullButton.Click += new System.EventHandler(this.FullButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(42, 22);
            this.toolStripLabel1.Text = "Zoom:";
            // 
            // ZoomLabel
            // 
            this.ZoomLabel.Name = "ZoomLabel";
            this.ZoomLabel.Size = new System.Drawing.Size(35, 22);
            this.ZoomLabel.Text = "100%";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(39, 22);
            this.toolStripLabel3.Text = "Color:";
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.BackColor = System.Drawing.Color.Green;
            this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton8.Text = "Line Color";
            this.toolStripButton8.Click += new System.EventHandler(this.toolStripButton8_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(54, 22);
            this.toolStripLabel2.Text = "Dot Style";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(100, 25);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(49, 22);
            this.toolStripLabel4.Text = "Dot Size";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(40, 25);
            this.toolStripTextBox1.TextChanged += new System.EventHandler(this.toolStripTextBox1_TextChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(66, 22);
            this.toolStripLabel5.Text = "Color Scale";
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(40, 25);
            this.toolStripTextBox2.TextChanged += new System.EventHandler(this.toolStripTextBox2_TextChanged);
            // 
            // graph1
            // 
            this.graph1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graph1.Location = new System.Drawing.Point(12, 52);
            this.graph1.MinimumSize = new System.Drawing.Size(222, 205);
            this.graph1.Name = "graph1";
            this.graph1.Size = new System.Drawing.Size(840, 513);
            this.graph1.TabIndex = 26;
            this.graph1.Title = "Title";
            this.graph1.Title2 = "Title2";
            this.graph1.Title3 = "Title3";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 590);
            this.Controls.Add(this.graph1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(621, 369);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Prototype - Step 1 - Emerson - FAU";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.ComponentModel.BackgroundWorker SaveFileWorker;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dateCoordinatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onCursorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimalGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton OpenButton;
        private System.Windows.Forms.ToolStripButton SaveButton;
        private System.Windows.Forms.ToolStripButton MoveButton;
        private System.Windows.Forms.ToolStripButton HorizontalShrinkButton;
        private System.Windows.Forms.ToolStripButton HorizonalStretchButton;
        private System.Windows.Forms.ToolStripButton VerticalShrinkButton;
        private System.Windows.Forms.ToolStripButton VerticalStretchButton;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel ZoomLabel;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripButton toolStripButton8;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private Graph graph1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton FullButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDataTableToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker OpenFileWorker;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton dataViewerButton;
        private System.Windows.Forms.ToolStripMenuItem saveGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFILLER;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToDatabaseToolStripMenuItem;
    }
}

