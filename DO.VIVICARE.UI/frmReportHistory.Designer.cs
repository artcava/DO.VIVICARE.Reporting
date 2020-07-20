namespace DO.VIVICARE.UI
{
    partial class frmReportHistory
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReportHistory));
            this.lvReport = new System.Windows.Forms.ListView();
            this.btnExit = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openFileExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileTxtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbChoose = new System.Windows.Forms.ComboBox();
            this.imageListGrandi = new System.Windows.Forms.ImageList(this.components);
            this.imageListPiccole = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvReport
            // 
            this.lvReport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvReport.BackColor = System.Drawing.Color.White;
            this.lvReport.HideSelection = false;
            this.lvReport.Location = new System.Drawing.Point(9, 50);
            this.lvReport.Name = "lvReport";
            this.lvReport.Size = new System.Drawing.Size(850, 354);
            this.lvReport.TabIndex = 17;
            this.lvReport.UseCompatibleStateImageBehavior = false;
            this.lvReport.View = System.Windows.Forms.View.Details;
            this.lvReport.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvReport_MouseClick);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Image = global::DO.VIVICARE.UI.Properties.Resources.logout;
            this.btnExit.Location = new System.Drawing.Point(789, 410);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(70, 50);
            this.btnExit.TabIndex = 213;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileExcelToolStripMenuItem,
            this.openFileCSVToolStripMenuItem,
            this.openFileTxtToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(148, 70);
            // 
            // openFileExcelToolStripMenuItem
            // 
            this.openFileExcelToolStripMenuItem.Name = "openFileExcelToolStripMenuItem";
            this.openFileExcelToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openFileExcelToolStripMenuItem.Text = "&Apri File Excel";
            this.openFileExcelToolStripMenuItem.Click += new System.EventHandler(this.openFileExcelToolStripMenuItem_Click);
            // 
            // openFileCSVToolStripMenuItem
            // 
            this.openFileCSVToolStripMenuItem.Name = "openFileCSVToolStripMenuItem";
            this.openFileCSVToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openFileCSVToolStripMenuItem.Text = "Apri File &CSV";
            this.openFileCSVToolStripMenuItem.Click += new System.EventHandler(this.openFileCSVToolStripMenuItem_Click);
            // 
            // openFileTxtToolStripMenuItem
            // 
            this.openFileTxtToolStripMenuItem.Name = "openFileTxtToolStripMenuItem";
            this.openFileTxtToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openFileTxtToolStripMenuItem.Text = "Apri File &Testo";
            this.openFileTxtToolStripMenuItem.Click += new System.EventHandler(this.openFileTxtToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(572, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 216;
            this.label1.Text = "Visualizza";
            // 
            // cmbChoose
            // 
            this.cmbChoose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbChoose.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChoose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbChoose.FormattingEnabled = true;
            this.cmbChoose.Items.AddRange(new object[] {
            "ICONE GRANDI",
            "ICONE PICCOLE",
            "DETTAGLI"});
            this.cmbChoose.Location = new System.Drawing.Point(641, 12);
            this.cmbChoose.Name = "cmbChoose";
            this.cmbChoose.Size = new System.Drawing.Size(218, 21);
            this.cmbChoose.TabIndex = 215;
            // 
            // imageListGrandi
            // 
            this.imageListGrandi.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListGrandi.ImageStream")));
            this.imageListGrandi.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListGrandi.Images.SetKeyName(0, "iconReportLarge.png");
            // 
            // imageListPiccole
            // 
            this.imageListPiccole.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListPiccole.ImageStream")));
            this.imageListPiccole.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListPiccole.Images.SetKeyName(0, "iconReportSmall.png");
            // 
            // frmReportHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 468);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbChoose);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.lvReport);
            this.Name = "frmReportHistory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Storico report";
            this.Load += new System.EventHandler(this.frmReportHistory_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvReport;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openFileExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileTxtToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbChoose;
        private System.Windows.Forms.ImageList imageListGrandi;
        private System.Windows.Forms.ImageList imageListPiccole;
    }
}