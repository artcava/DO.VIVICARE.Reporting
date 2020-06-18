namespace DO.VIVICARE.UI
{
    partial class frmDocuments
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDocuments));
            this.lvReport = new System.Windows.Forms.ListView();
            this.cmbChoose = new System.Windows.Forms.ComboBox();
            this.imageListGrandi = new System.Windows.Forms.ImageList(this.components);
            this.imageListPiccole = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.apriFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.caricaFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnExit = new System.Windows.Forms.Button();
            this.apriFileDiorigineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verificaFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvReport
            // 
            this.lvReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvReport.BackColor = System.Drawing.Color.White;
            this.lvReport.HideSelection = false;
            this.lvReport.Location = new System.Drawing.Point(16, 89);
            this.lvReport.Margin = new System.Windows.Forms.Padding(4);
            this.lvReport.Name = "lvReport";
            this.lvReport.Size = new System.Drawing.Size(1337, 550);
            this.lvReport.TabIndex = 14;
            this.lvReport.UseCompatibleStateImageBehavior = false;
            this.lvReport.View = System.Windows.Forms.View.Details;
            this.lvReport.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvReport_MouseClick);
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
            this.cmbChoose.Location = new System.Drawing.Point(1040, 23);
            this.cmbChoose.Margin = new System.Windows.Forms.Padding(4);
            this.cmbChoose.Name = "cmbChoose";
            this.cmbChoose.Size = new System.Drawing.Size(289, 24);
            this.cmbChoose.TabIndex = 13;
            this.cmbChoose.SelectionChangeCommitted += new System.EventHandler(this.cmbChoose_SelectionChangeCommitted);
            // 
            // imageListGrandi
            // 
            this.imageListGrandi.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListGrandi.ImageStream")));
            this.imageListGrandi.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListGrandi.Images.SetKeyName(0, "iconLarge.png");
            // 
            // imageListPiccole
            // 
            this.imageListPiccole.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListPiccole.ImageStream")));
            this.imageListPiccole.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListPiccole.Images.SetKeyName(0, "iconSmall.png");
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbChoose);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1339, 66);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(633, 28);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 17);
            this.label2.TabIndex = 16;
            this.label2.Text = "Visualizza";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(49, 23);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Maximum = 1000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(576, 28);
            this.progressBar1.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(924, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "Visualizza";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.apriFileToolStripMenuItem,
            this.caricaFileToolStripMenuItem,
            this.apriFileDiorigineToolStripMenuItem,
            this.verificaFileToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(211, 128);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // apriFileToolStripMenuItem
            // 
            this.apriFileToolStripMenuItem.Name = "apriFileToolStripMenuItem";
            this.apriFileToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.apriFileToolStripMenuItem.Text = "&Apri File";
            this.apriFileToolStripMenuItem.Click += new System.EventHandler(this.apriFileToolStripMenuItem_Click);
            // 
            // caricaFileToolStripMenuItem
            // 
            this.caricaFileToolStripMenuItem.Name = "caricaFileToolStripMenuItem";
            this.caricaFileToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.caricaFileToolStripMenuItem.Text = "&Carica File";
            this.caricaFileToolStripMenuItem.Click += new System.EventHandler(this.caricaFileToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Image = global::DO.VIVICARE.UI.Properties.Resources.logout;
            this.btnExit.Location = new System.Drawing.Point(1261, 647);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(93, 62);
            this.btnExit.TabIndex = 211;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // apriFileDiorigineToolStripMenuItem
            // 
            this.apriFileDiorigineToolStripMenuItem.Name = "apriFileDiorigineToolStripMenuItem";
            this.apriFileDiorigineToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.apriFileDiorigineToolStripMenuItem.Text = "Apri File di &Origine";
            this.apriFileDiorigineToolStripMenuItem.Click += new System.EventHandler(this.apriFileDiorigineToolStripMenuItem_Click);
            // 
            // verificaFileToolStripMenuItem
            // 
            this.verificaFileToolStripMenuItem.Name = "verificaFileToolStripMenuItem";
            this.verificaFileToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.verificaFileToolStripMenuItem.Text = "&Verifica File";
            this.verificaFileToolStripMenuItem.Click += new System.EventHandler(this.verificaFileToolStripMenuItem_Click);
            // 
            // frmDocuments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1371, 718);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lvReport);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmDocuments";
            this.Text = "DOCUMENTI";
            this.Load += new System.EventHandler(this.frmDocuments_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView lvReport;
        private System.Windows.Forms.ComboBox cmbChoose;
        private System.Windows.Forms.ImageList imageListGrandi;
        private System.Windows.Forms.ImageList imageListPiccole;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem apriFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem caricaFileToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem apriFileDiorigineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verificaFileToolStripMenuItem;
    }
}

