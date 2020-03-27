namespace DO.VIVICARE.UI
{
    partial class frmSettings
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvElencoDocuments = new System.Windows.Forms.DataGridView();
            this.NomeFileDocument = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NomeFileDocumentCompleto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DownloadDocument = new System.Windows.Forms.DataGridViewLinkColumn();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageDocuments = new System.Windows.Forms.TabPage();
            this.tabPageReports = new System.Windows.Forms.TabPage();
            this.dgvElencoReports = new System.Windows.Forms.DataGridView();
            this.btnChoose = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.NomeFileReport = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NomeFileReportCompleto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DownloadReport = new System.Windows.Forms.DataGridViewLinkColumn();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvElencoDocuments)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPageDocuments.SuspendLayout();
            this.tabPageReports.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvElencoReports)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvElencoDocuments
            // 
            this.dgvElencoDocuments.AllowUserToAddRows = false;
            this.dgvElencoDocuments.AllowUserToDeleteRows = false;
            this.dgvElencoDocuments.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvElencoDocuments.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvElencoDocuments.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvElencoDocuments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvElencoDocuments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NomeFileDocument,
            this.NomeFileDocumentCompleto,
            this.DownloadDocument});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvElencoDocuments.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvElencoDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvElencoDocuments.Location = new System.Drawing.Point(3, 3);
            this.dgvElencoDocuments.Name = "dgvElencoDocuments";
            this.dgvElencoDocuments.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvElencoDocuments.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvElencoDocuments.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvElencoDocuments.Size = new System.Drawing.Size(647, 248);
            this.dgvElencoDocuments.TabIndex = 6;
            this.dgvElencoDocuments.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvElencoDocuments_CellContentClick);
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
            // DownloadDocument
            // 
            this.DownloadDocument.HeaderText = "Download";
            this.DownloadDocument.Name = "DownloadDocument";
            this.DownloadDocument.ReadOnly = true;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageDocuments);
            this.tabControl.Controls.Add(this.tabPageReports);
            this.tabControl.Location = new System.Drawing.Point(12, 62);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(661, 280);
            this.tabControl.TabIndex = 212;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPageDocuments
            // 
            this.tabPageDocuments.Controls.Add(this.dgvElencoDocuments);
            this.tabPageDocuments.Location = new System.Drawing.Point(4, 22);
            this.tabPageDocuments.Name = "tabPageDocuments";
            this.tabPageDocuments.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDocuments.Size = new System.Drawing.Size(653, 254);
            this.tabPageDocuments.TabIndex = 0;
            this.tabPageDocuments.Text = "DOCUMENTI";
            this.tabPageDocuments.UseVisualStyleBackColor = true;
            // 
            // tabPageReports
            // 
            this.tabPageReports.Controls.Add(this.dgvElencoReports);
            this.tabPageReports.Location = new System.Drawing.Point(4, 22);
            this.tabPageReports.Name = "tabPageReports";
            this.tabPageReports.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageReports.Size = new System.Drawing.Size(653, 254);
            this.tabPageReports.TabIndex = 1;
            this.tabPageReports.Text = "REPORT";
            this.tabPageReports.UseVisualStyleBackColor = true;
            // 
            // dgvElencoReports
            // 
            this.dgvElencoReports.AllowUserToAddRows = false;
            this.dgvElencoReports.AllowUserToDeleteRows = false;
            this.dgvElencoReports.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvElencoReports.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvElencoReports.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvElencoReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvElencoReports.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NomeFileReport,
            this.NomeFileReportCompleto,
            this.DownloadReport});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvElencoReports.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvElencoReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvElencoReports.Location = new System.Drawing.Point(3, 3);
            this.dgvElencoReports.Name = "dgvElencoReports";
            this.dgvElencoReports.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvElencoReports.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvElencoReports.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvElencoReports.Size = new System.Drawing.Size(647, 248);
            this.dgvElencoReports.TabIndex = 7;
            this.dgvElencoReports.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvElencoReports_CellContentClick);
            // 
            // btnChoose
            // 
            this.btnChoose.Location = new System.Drawing.Point(610, 14);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(35, 23);
            this.btnChoose.TabIndex = 215;
            this.btnChoose.Text = "...";
            this.btnChoose.UseVisualStyleBackColor = true;
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(111, 15);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(493, 20);
            this.txtPath.TabIndex = 214;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 213;
            this.label5.Text = "Percorso libreria";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.btnChoose);
            this.groupBox3.Controls.Add(this.txtPath);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(661, 44);
            this.groupBox3.TabIndex = 216;
            this.groupBox3.TabStop = false;
            // 
            // lblResult
            // 
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(391, 63);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(278, 18);
            this.lblResult.TabIndex = 8;
            this.lblResult.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // NomeFileReport
            // 
            this.NomeFileReport.HeaderText = "Nome file";
            this.NomeFileReport.Name = "NomeFileReport";
            this.NomeFileReport.ReadOnly = true;
            // 
            // NomeFileReportCompleto
            // 
            this.NomeFileReportCompleto.HeaderText = "Nome file completo";
            this.NomeFileReportCompleto.Name = "NomeFileReportCompleto";
            this.NomeFileReportCompleto.ReadOnly = true;
            this.NomeFileReportCompleto.Visible = false;
            // 
            // DownloadReport
            // 
            this.DownloadReport.HeaderText = "Download";
            this.DownloadReport.Name = "DownloadReport";
            this.DownloadReport.ReadOnly = true;
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Image = global::DO.VIVICARE.UI.Properties.Resources.logout;
            this.btnExit.Location = new System.Drawing.Point(603, 348);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(70, 50);
            this.btnExit.TabIndex = 210;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Image = global::DO.VIVICARE.UI.Properties.Resources.ok;
            this.btnConfirm.Location = new System.Drawing.Point(12, 348);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(70, 50);
            this.btnConfirm.TabIndex = 209;
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 409);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnConfirm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IMPOSTAZIONI";
            this.Shown += new System.EventHandler(this.frmSettings_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvElencoDocuments)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPageDocuments.ResumeLayout(false);
            this.tabPageReports.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvElencoReports)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.DataGridView dgvElencoDocuments;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageDocuments;
        private System.Windows.Forms.TabPage tabPageReports;
        private System.Windows.Forms.Button btnChoose;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeFileDocument;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeFileDocumentCompleto;
        private System.Windows.Forms.DataGridViewLinkColumn DownloadDocument;
        private System.Windows.Forms.DataGridView dgvElencoReports;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeFileReport;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeFileReportCompleto;
        private System.Windows.Forms.DataGridViewLinkColumn DownloadReport;
    }
}