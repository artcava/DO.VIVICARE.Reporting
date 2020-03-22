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
            this.SuspendLayout();
            // 
            // lvReport
            // 
            this.lvReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvReport.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lvReport.HideSelection = false;
            this.lvReport.Location = new System.Drawing.Point(12, 39);
            this.lvReport.Name = "lvReport";
            this.lvReport.Size = new System.Drawing.Size(776, 399);
            this.lvReport.TabIndex = 14;
            this.lvReport.UseCompatibleStateImageBehavior = false;
            // 
            // cmbChoose
            // 
            this.cmbChoose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbChoose.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChoose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbChoose.FormattingEnabled = true;
            this.cmbChoose.Items.AddRange(new object[] {
            "ICONE GRANDI",
            "ICONE PICCOLE"});
            this.cmbChoose.Location = new System.Drawing.Point(510, 12);
            this.cmbChoose.Name = "cmbChoose";
            this.cmbChoose.Size = new System.Drawing.Size(278, 21);
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
            // frmDocuments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cmbChoose);
            this.Controls.Add(this.lvReport);
            this.Name = "frmDocuments";
            this.Text = "DOCUMENTI";
            this.Load += new System.EventHandler(this.frmDocuments_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView lvReport;
        private System.Windows.Forms.ComboBox cmbChoose;
        private System.Windows.Forms.ImageList imageListGrandi;
        private System.Windows.Forms.ImageList imageListPiccole;
    }
}

