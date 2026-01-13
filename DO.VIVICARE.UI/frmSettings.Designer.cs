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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvElencoDocuments = new System.Windows.Forms.DataGridView();
            this.NomeFileDocument = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VersionInstalledDocument = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HasUpdateDocument = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NomeFileDocumentCompleto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DownloadDocument = new System.Windows.Forms.DataGridViewLinkColumn();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageDocuments = new System.Windows.Forms.TabPage();
            this.tabPageReports = new System.Windows.Forms.TabPage();
            this.dgvElencoReports = new System.Windows.Forms.DataGridView();
            this.NomeFileReport = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VersionInstalledReport = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HasUpdateReport = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NomeFileReportCompleto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DownloadReport = new System.Windows.Forms.DataGridViewLinkColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvElencoDocuments)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPageDocuments.SuspendLayout();
            this.tabPageReports.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvElencoReports)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvElencoDocuments
            // 
            this.dgvElencoDocuments.AllowUserToAddRows = false;
            this.dgvElencoDocuments.AllowUserToDeleteRows = false;
            this.dgvElencoDocuments.AllowUserToResizeRows = false;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvElencoDocuments.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle11;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvElencoDocuments.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dgvElencoDocuments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvElencoDocuments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NomeFileDocument,
            this.VersionInstalledDocument,
            this.HasUpdateDocument,
            this.NomeFileDocumentCompleto,
            this.DownloadDocument});
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvElencoDocuments.DefaultCellStyle = dataGridViewCellStyle13;
            this.dgvElencoDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvElencoDocuments.Location = new System.Drawing.Point(3, 3);
            this.dgvElencoDocuments.Name = "dgvElencoDocuments";
            this.dgvElencoDocuments.ReadOnly = true;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvElencoDocuments.RowHeadersDefaultCellStyle = dataGridViewCellStyle14;
            this.dgvElencoDocuments.RowHeadersWidth = 51;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvElencoDocuments.RowsDefaultCellStyle = dataGridViewCellStyle15;
            this.dgvElencoDocuments.Size = new System.Drawing.Size(647, 248);
            this.dgvElencoDocuments.TabIndex = 6;
            this.dgvElencoDocuments.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvElencoDocuments_CellContentClick);
            // 
            // NomeFileDocument
            // 
            this.NomeFileDocument.HeaderText = "Nome Plugin";
            this.NomeFileDocument.MinimumWidth = 6;
            this.NomeFileDocument.Name = "NomeFileDocument";
            this.NomeFileDocument.ReadOnly = true;
            this.NomeFileDocument.Width = 150;
            // 
            // VersionInstalledDocument
            // 
            this.VersionInstalledDocument.HeaderText = "Versione Installata";
            this.VersionInstalledDocument.MinimumWidth = 6;
            this.VersionInstalledDocument.Name = "VersionInstalledDocument";
            this.VersionInstalledDocument.ReadOnly = true;
            this.VersionInstalledDocument.Width = 100;
            // 
            // HasUpdateDocument
            // 
            this.HasUpdateDocument.HeaderText = "Aggiornamento";
            this.HasUpdateDocument.MinimumWidth = 6;
            this.HasUpdateDocument.Name = "HasUpdateDocument";
            this.HasUpdateDocument.ReadOnly = true;
            this.HasUpdateDocument.Width = 100;
            // 
            // NomeFileDocumentCompleto
            // 
            this.NomeFileDocumentCompleto.HeaderText = "Nome file completo";
            this.NomeFileDocumentCompleto.MinimumWidth = 6;
            this.NomeFileDocumentCompleto.Name = "NomeFileDocumentCompleto";
            this.NomeFileDocumentCompleto.ReadOnly = true;
            this.NomeFileDocumentCompleto.Visible = false;
            this.NomeFileDocumentCompleto.Width = 125;
            // 
            // DownloadDocument
            // 
            this.DownloadDocument.HeaderText = "Azione";
            this.DownloadDocument.MinimumWidth = 6;
            this.DownloadDocument.Name = "DownloadDocument";
            this.DownloadDocument.ReadOnly = true;
            this.DownloadDocument.Width = 80;
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
            dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvElencoReports.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle16;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvElencoReports.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
            this.dgvElencoReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvElencoReports.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NomeFileReport,
            this.VersionInstalledReport,
            this.HasUpdateReport,
            this.NomeFileReportCompleto,
            this.DownloadReport});
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvElencoReports.DefaultCellStyle = dataGridViewCellStyle18;
            this.dgvElencoReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvElencoReports.Location = new System.Drawing.Point(3, 3);
            this.dgvElencoReports.Name = "dgvElencoReports";
            this.dgvElencoReports.ReadOnly = true;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle19.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle19.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle19.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle19.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvElencoReports.RowHeadersDefaultCellStyle = dataGridViewCellStyle19;
            this.dgvElencoReports.RowHeadersWidth = 51;
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvElencoReports.RowsDefaultCellStyle = dataGridViewCellStyle20;
            this.dgvElencoReports.Size = new System.Drawing.Size(647, 248);
            this.dgvElencoReports.TabIndex = 7;
            this.dgvElencoReports.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvElencoReports_CellContentClick);
            // 
            // NomeFileReport
            // 
            this.NomeFileReport.HeaderText = "Nome Plugin";
            this.NomeFileReport.MinimumWidth = 6;
            this.NomeFileReport.Name = "NomeFileReport";
            this.NomeFileReport.ReadOnly = true;
            this.NomeFileReport.Width = 150;
            // 
            // VersionInstalledReport
            // 
            this.VersionInstalledReport.HeaderText = "Versione Installata";
            this.VersionInstalledReport.MinimumWidth = 6;
            this.VersionInstalledReport.Name = "VersionInstalledReport";
            this.VersionInstalledReport.ReadOnly = true;
            this.VersionInstalledReport.Width = 100;
            // 
            // HasUpdateReport
            // 
            this.HasUpdateReport.HeaderText = "Aggiornamento";
            this.HasUpdateReport.MinimumWidth = 6;
            this.HasUpdateReport.Name = "HasUpdateReport";
            this.HasUpdateReport.ReadOnly = true;
            this.HasUpdateReport.Width = 100;
            // 
            // NomeFileReportCompleto
            // 
            this.NomeFileReportCompleto.HeaderText = "Nome file completo";
            this.NomeFileReportCompleto.MinimumWidth = 6;
            this.NomeFileReportCompleto.Name = "NomeFileReportCompleto";
            this.NomeFileReportCompleto.ReadOnly = true;
            this.NomeFileReportCompleto.Visible = false;
            this.NomeFileReportCompleto.Width = 125;
            // 
            // DownloadReport
            // 
            this.DownloadReport.HeaderText = "Azione";
            this.DownloadReport.MinimumWidth = 6;
            this.DownloadReport.Name = "DownloadReport";
            this.DownloadReport.ReadOnly = true;
            this.DownloadReport.Width = 80;
            // 
            // groupBox3
            // 
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
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.DataGridView dgvElencoDocuments;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageDocuments;
        private System.Windows.Forms.TabPage tabPageReports;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeFileDocument;
        private System.Windows.Forms.DataGridViewTextBoxColumn VersionInstalledDocument;
        private System.Windows.Forms.DataGridViewTextBoxColumn HasUpdateDocument;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeFileDocumentCompleto;
        private System.Windows.Forms.DataGridViewLinkColumn DownloadDocument;
        private System.Windows.Forms.DataGridView dgvElencoReports;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeFileReport;
        private System.Windows.Forms.DataGridViewTextBoxColumn VersionInstalledReport;
        private System.Windows.Forms.DataGridViewTextBoxColumn HasUpdateReport;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeFileReportCompleto;
        private System.Windows.Forms.DataGridViewLinkColumn DownloadReport;
    }
}