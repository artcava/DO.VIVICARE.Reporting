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

        public string Type { get; set; }
         
        public frmDocuments(string t)
        {

            InitializeComponent();
            Type = t;
            Text = "GESTIONE " + t.ToUpper();

            if (Type == "report")
                lvReport.BackColor = Color.SkyBlue;
            else if (Type == "document")
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
            foreach (var f in Manager.GetDocuments(Properties.Settings.Default["UserPathReport"].ToString()))
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
            if (this.Type == "report") e.Cancel = true;

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
        }
    }
}
