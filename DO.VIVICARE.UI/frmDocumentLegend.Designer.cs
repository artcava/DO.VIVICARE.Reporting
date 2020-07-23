namespace DO.VIVICARE.UI
{
    partial class frmDocumentLegend
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
            this.lvReport = new System.Windows.Forms.ListView();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvReport
            // 
            this.lvReport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvReport.BackColor = System.Drawing.Color.White;
            this.lvReport.HideSelection = false;
            this.lvReport.Location = new System.Drawing.Point(12, 13);
            this.lvReport.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lvReport.Name = "lvReport";
            this.lvReport.Size = new System.Drawing.Size(1132, 484);
            this.lvReport.TabIndex = 17;
            this.lvReport.UseCompatibleStateImageBehavior = false;
            this.lvReport.View = System.Windows.Forms.View.Details;
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Image = global::DO.VIVICARE.UI.Properties.Resources.logout;
            this.btnExit.Location = new System.Drawing.Point(1052, 505);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(93, 62);
            this.btnExit.TabIndex = 213;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // frmDocumentLegend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1161, 576);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.lvReport);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmDocumentLegend";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Storico report";
            this.Load += new System.EventHandler(this.frmReportHistory_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvReport;
        private System.Windows.Forms.Button btnExit;
    }
}