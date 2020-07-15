namespace DO.VIVICARE.UI
{
    partial class frmInputReportParameter
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dgvParameters = new System.Windows.Forms.DataGridView();
            this.ParameterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParameterDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParameterValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParameters)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(481, 210);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(562, 210);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Annulla";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dgvParameters
            // 
            this.dgvParameters.AllowUserToAddRows = false;
            this.dgvParameters.AllowUserToDeleteRows = false;
            this.dgvParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParameterName,
            this.ParameterDescription,
            this.ParameterValue});
            this.dgvParameters.Location = new System.Drawing.Point(12, 12);
            this.dgvParameters.MultiSelect = false;
            this.dgvParameters.Name = "dgvParameters";
            this.dgvParameters.Size = new System.Drawing.Size(623, 183);
            this.dgvParameters.TabIndex = 2;
            // 
            // ParameterName
            // 
            this.ParameterName.HeaderText = "Nome Parametro";
            this.ParameterName.Name = "ParameterName";
            this.ParameterName.Visible = false;
            // 
            // ParameterDescription
            // 
            this.ParameterDescription.HeaderText = "Descrizione Parametro";
            this.ParameterDescription.Name = "ParameterDescription";
            this.ParameterDescription.Width = 300;
            // 
            // ParameterValue
            // 
            this.ParameterValue.HeaderText = "Valore";
            this.ParameterValue.Name = "ParameterValue";
            this.ParameterValue.Width = 250;
            // 
            // frmInputReportParameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 244);
            this.Controls.Add(this.dgvParameters);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Name = "frmInputReportParameter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Parametri";
            ((System.ComponentModel.ISupportInitialize)(this.dgvParameters)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView dgvParameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterValue;
    }
}