using DO.VIVICARE.Reporter;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
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
            try
            {
                cmbChoose.SelectedIndex = 0;

                if (Properties.Settings.Default["UserPathDefault"] != null)
                {
                    foreach (var f in Manager.GetDocuments(Path.Combine(Properties.Settings.Default["UserPathDefault"].ToString(),
                                                            Properties.Settings.Default["UserFolderDocuments"].ToString())))
                    {
                        lvReport.AddRow(0, f.Attribute.Name, f.Attribute.Description, f.Attribute.FileName);
                        lvReport.Items[lvReport.Items.Count - 1].Tag = f.Document;
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
                MessageBox.Show("Devi specificare il Percorso libreria in Strumenti\\Opzioni");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                    ((BaseDocument)nome.Tag).UserPathReport = openFileDialog1.FileName;
                    var res = ((BaseDocument)nome.Tag).CheckFields();
                    if (res.Count != 0)
                    {
                        var msg = "CHECK VALUE!";
                        foreach (var m in res)
                        {
                            msg += "\n" + m.ToString();
                        }
                        MessageBox.Show(msg, "Errore!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    File.Copy(nomeFile, Path.Combine(Properties.Settings.Default["UserPathDefault"].ToString(), nome.SubItems[2].Text), true);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
