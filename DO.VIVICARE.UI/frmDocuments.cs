using DO.VIVICARE.Reporter;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class frmDocuments : Form
    { 
        public frmDocuments()
        {
            InitializeComponent();
            lvReport.BackColor = Color.DarkSeaGreen;
        }

        private void cmbChoose_SelectionChangeCommitted(object sender, EventArgs e)
        {
            switch (cmbChoose.SelectedIndex)
            {
                case 0: // "ICONE GRANDI"
                    lvReport.View = View.LargeIcon;
                    break;
                case 1: // "ICONE PICCOLE"
                    lvReport.View = View.SmallIcon;
                    break;
                case 2: // "DETTAGLI"
                    lvReport.View = View.Details;
                    break;
            }
        }

        private void frmDocuments_Load(object sender, EventArgs e)
        {
            cmbChoose.SelectedIndex = 0;
            //FileInfo assemblyFile = new FileInfo(Assembly.GetExecutingAssembly().Location);

            foreach (var f in Manager.GetDocuments(Path.Combine(Properties.Settings.Default["UserPathDefault"].ToString(),
                                                    Properties.Settings.Default["UserFolderDocuments"].ToString())))
            {
                lvReport.AddRow(0, f.Name, "param1", "param2", "param3", "param4");
            }
            
            lvReport.SmallImageList = imageListPiccole;
            lvReport.LargeImageList = imageListGrandi;
            lvReport.MountHeaders(
                    "File", 180, HorizontalAlignment.Left,
                    "Altro1", 120, HorizontalAlignment.Left,
                    "Altro2", 120, HorizontalAlignment.Left,
                    "Altro3", 120, HorizontalAlignment.Left,
                    "Altro4", 120, HorizontalAlignment.Left);
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lvReport.SelectedItems.Count == 0)
                e.Cancel = true;
        }

        private void lvReport_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pt = lvReport.PointToScreen(e.Location);
                contextMenuStrip1.Show(pt);
            }
        }

        private void apriFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE CON EXCEL SEPARATAMENTE
        }

        private void caricaFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // UPLOAD NUOVO FILE IN ARCHIVIO.. SOVRASCRIVENDOLO
            var nome = lvReport.SelectedItems[0];
            openFileDialog1.Title = "Excel File";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Excel File|*.xlsx;*.xls";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var nomeFile = openFileDialog1.FileName;
                if (nomeFile.Trim() != "")
                {
                    File.Copy(nomeFile, Path.Combine(Properties.Settings.Default["UserPathDefault"].ToString(), nome.Text + Path.GetExtension(nomeFile)), true);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
