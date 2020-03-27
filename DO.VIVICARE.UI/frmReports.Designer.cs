namespace DO.VIVICARE.UI
{
    partial class frmReports
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReports));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbChoose = new System.Windows.Forms.ComboBox();
            this.lvReport = new System.Windows.Forms.ListView();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.imageListGrandi = new System.Windows.Forms.ImageList(this.components);
            this.imageListPiccole = new System.Windows.Forms.ImageList(this.components);
            this.dgvElenco = new System.Windows.Forms.DataGridView();
            this.NomeFileDocument = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NomeFileDocumentCompleto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Altro1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Altro2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Altro3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvElenco)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbChoose);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1269, 54);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(976, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 14;
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
            this.cmbChoose.Location = new System.Drawing.Point(1045, 19);
            this.cmbChoose.Name = "cmbChoose";
            this.cmbChoose.Size = new System.Drawing.Size(218, 21);
            this.cmbChoose.TabIndex = 13;
            this.cmbChoose.SelectionChangeCommitted += new System.EventHandler(this.cmbChoose_SelectionChangeCommitted);
            // 
            // lvReport
            // 
            this.lvReport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvReport.BackColor = System.Drawing.Color.White;
            this.lvReport.HideSelection = false;
            this.lvReport.Location = new System.Drawing.Point(12, 72);
            this.lvReport.Name = "lvReport";
            this.lvReport.Size = new System.Drawing.Size(1272, 447);
            this.lvReport.TabIndex = 16;
            this.lvReport.UseCompatibleStateImageBehavior = false;
            this.lvReport.Click += new System.EventHandler(this.lvReport_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Image = global::DO.VIVICARE.UI.Properties.Resources.logout;
            this.btnExit.Location = new System.Drawing.Point(1214, 681);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(70, 50);
            this.btnExit.TabIndex = 212;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Image = global::DO.VIVICARE.UI.Properties.Resources.execute;
            this.btnExecute.Location = new System.Drawing.Point(1138, 681);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(70, 50);
            this.btnExecute.TabIndex = 211;
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
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
            // dgvElenco
            // 
            this.dgvElenco.AllowUserToAddRows = false;
            this.dgvElenco.AllowUserToDeleteRows = false;
            this.dgvElenco.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvElenco.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvElenco.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvElenco.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvElenco.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvElenco.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NomeFileDocument,
            this.NomeFileDocumentCompleto,
            this.Altro1,
            this.Altro2,
            this.Altro3});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvElenco.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvElenco.Location = new System.Drawing.Point(12, 525);
            this.dgvElenco.Name = "dgvElenco";
            this.dgvElenco.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvElenco.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvElenco.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvElenco.Size = new System.Drawing.Size(1272, 150);
            this.dgvElenco.TabIndex = 214;
            // 
            // NomeFileDocument
            // 
            this.NomeFileDocument.HeaderText = "Nome file";
            this.NomeFileDocument.Name = "NomeFileDocument";
            this.NomeFileDocument.ReadOnly = true;
            // 
            // NomeFileDocumentCompleto
            // 
            this.NomeFileDocumentCompleto.HeaderText = "Nome file completo";
            this.NomeFileDocumentCompleto.Name = "NomeFileDocumentCompleto";
            this.NomeFileDocumentCompleto.ReadOnly = true;
            this.NomeFileDocumentCompleto.Visible = false;
            // 
            // Altro1
            // 
            this.Altro1.HeaderText = "Altro1";
            this.Altro1.Name = "Altro1";
            this.Altro1.ReadOnly = true;
            // 
            // Altro2
            // 
            this.Altro2.HeaderText = "Altro2";
            this.Altro2.Name = "Altro2";
            this.Altro2.ReadOnly = true;
            // 
            // Altro3
            // 
            this.Altro3.HeaderText = "Altro3";
            this.Altro3.Name = "Altro3";
            this.Altro3.ReadOnly = true;
            // 
            // frmReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1293, 739);
            this.Controls.Add(this.dgvElenco);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lvReport);
            this.Name = "frmReports";
            this.Text = "REPORT";
            this.Load += new System.EventHandler(this.frmReports_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvElenco)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbChoose;
        private System.Windows.Forms.ListView lvReport;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.ImageList imageListGrandi;
        private System.Windows.Forms.ImageList imageListPiccole;
        private System.Windows.Forms.DataGridView dgvElenco;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeFileDocument;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeFileDocumentCompleto;
        private System.Windows.Forms.DataGridViewTextBoxColumn Altro1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Altro2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Altro3;
    }
}