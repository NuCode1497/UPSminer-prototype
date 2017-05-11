namespace prototype
{
    partial class Graph
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.Title2Label = new System.Windows.Forms.Label();
            this.gridPanel = new prototype.DoubleBufferedPanel();
            this.gridYLabelPanel = new prototype.DoubleBufferedPanel();
            this.gridXLabelPanel = new prototype.DoubleBufferedPanel();
            this.Title3Label = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.TitleLabel);
            this.flowLayoutPanel1.Controls.Add(this.Title2Label);
            this.flowLayoutPanel1.Controls.Add(this.Title3Label);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(611, 18);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Location = new System.Drawing.Point(3, 0);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(27, 13);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "Title";
            // 
            // Title2Label
            // 
            this.Title2Label.AutoSize = true;
            this.Title2Label.Location = new System.Drawing.Point(36, 0);
            this.Title2Label.Name = "Title2Label";
            this.Title2Label.Size = new System.Drawing.Size(33, 13);
            this.Title2Label.TabIndex = 1;
            this.Title2Label.Text = "Title2";
            // 
            // gridPanel
            // 
            this.gridPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPanel.AutoScroll = true;
            this.gridPanel.AutoSize = true;
            this.gridPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(73)))));
            this.gridPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridPanel.Location = new System.Drawing.Point(64, 18);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(547, 288);
            this.gridPanel.TabIndex = 8;
            this.gridPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gridPanel_Paint);
            this.gridPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridPanel_MouseDown);
            this.gridPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridPanel_MouseMove);
            this.gridPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridPanel_MouseUp);
            this.gridPanel.Resize += new System.EventHandler(this.gridPanel_Resize);
            // 
            // gridYLabelPanel
            // 
            this.gridYLabelPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gridYLabelPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridYLabelPanel.Location = new System.Drawing.Point(0, 18);
            this.gridYLabelPanel.Name = "gridYLabelPanel";
            this.gridYLabelPanel.Size = new System.Drawing.Size(64, 288);
            this.gridYLabelPanel.TabIndex = 9;
            this.gridYLabelPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gridYLabelPanel_Paint);
            this.gridYLabelPanel.Resize += new System.EventHandler(this.gridYLabelPanel_Resize);
            // 
            // gridXLabelPanel
            // 
            this.gridXLabelPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridXLabelPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridXLabelPanel.Location = new System.Drawing.Point(64, 306);
            this.gridXLabelPanel.Name = "gridXLabelPanel";
            this.gridXLabelPanel.Size = new System.Drawing.Size(547, 79);
            this.gridXLabelPanel.TabIndex = 10;
            this.gridXLabelPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gridXLabelPanel_Paint);
            this.gridXLabelPanel.Resize += new System.EventHandler(this.gridXLabelPanel_Resize);
            // 
            // Title3Label
            // 
            this.Title3Label.AutoSize = true;
            this.Title3Label.Location = new System.Drawing.Point(75, 0);
            this.Title3Label.Name = "Title3Label";
            this.Title3Label.Size = new System.Drawing.Size(33, 13);
            this.Title3Label.TabIndex = 2;
            this.Title3Label.Text = "Title3";
            // 
            // Graph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.gridPanel);
            this.Controls.Add(this.gridYLabelPanel);
            this.Controls.Add(this.gridXLabelPanel);
            this.MinimumSize = new System.Drawing.Size(222, 205);
            this.Name = "Graph";
            this.Size = new System.Drawing.Size(611, 385);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DoubleBufferedPanel gridPanel;
        private DoubleBufferedPanel gridYLabelPanel;
        private DoubleBufferedPanel gridXLabelPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label Title2Label;
        private System.Windows.Forms.Label Title3Label;
    }
}
