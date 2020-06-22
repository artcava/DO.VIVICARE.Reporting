using DO.VIVICARE.Reporter;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            LoadDocuments();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
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

        #region ToolStripMenuItem_Click
        private void apriFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE CON EXCEL SEPARATAMENTE
            var nome = lvReport.SelectedItems[0];
            if (nome.SubItems[2].Text == "...")
                MessageBox.Show($"Non hai ancora caricato nessun file per il documento [{nome.SubItems[0].Text}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                System.Diagnostics.Process.Start(Path.Combine(Manager.Documents, nome.SubItems[2].Text));
        }

        private async void caricaFileToolStripMenuItem_Click(object sender, EventArgs e)
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
                    var extension = nomeFile.Substring(nomeFile.LastIndexOf('.'));
                    ((BaseDocument)nome.Tag).SourceFilePath = nomeFile;

                    //=================================================================
                    label2.Text = "Attendere...";
                    progressBar1.Value = 0;
                    var progress = new Progress<int>(p =>
                    {
                        progressBar1.Value = progressBar1.Maximum / p;

                    });
                    var ok = await Task.Run(() => ((BaseDocument)nome.Tag).CheckFields(progress));
                    label2.Text = "Completato!";
                    //=================================================================

                    var status = XMLSettings.DocumentStatus.FileOK;
                    var msg = "CHECK VALUE!";
                    if (!ok)
                    {
                        status = XMLSettings.DocumentStatus.FileInError;
                        //===> Caricare in qualche modo il file di log generato in Path.Combine(Manager.Documents, nome.SubItems[0].Text + ".log")
                        msg += "Errore nel caricamento del file";
                        //foreach (var m in ok.Take(5))  //===> qua magari fagliene vedere 5 alla volta.. così non saturi tutto
                        //{
                        //    msg += "\n" + m.ToString();
                        //}
                    }

                    File.Copy(nomeFile, Path.Combine(Manager.Documents, nome.SubItems[0].Text + extension), true);
                    var now = DateTime.Now;
                    Manager.Settings.UpdateDocument(nome.SubItems[0].Text, extension, nomeFile, now, now, now, status);
                    if (ok)
                        MessageBox.Show($"File {nome.SubItems[1].Text} caricato correttamente!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show(msg, "Errore!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    LoadDocuments();
                }
            }
        }
        private void apriFileDiorigineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE CON EXCEL SEPARATAMENTE
            var nome = lvReport.SelectedItems[0];
            if (nome.SubItems[3].Text == "...")
                MessageBox.Show($"Non hai ancora caricato nessun file per il documento [{nome.SubItems[0].Text}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                System.Diagnostics.Process.Start(nome.SubItems[3].Text);
        }

        private async void verificaFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // VERIFICA FILE IN ARCHIVIO..
            var nome = lvReport.SelectedItems[0];
            if (nome.SubItems[2].Text == "...")
            {
                MessageBox.Show($"Non hai ancora caricato nessun file per il documento [{nome.SubItems[0].Text}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nomeFile = Path.Combine(Manager.Documents, nome.SubItems[2].Text);
            var extension = nomeFile.Substring(nomeFile.LastIndexOf('.'));
            ((BaseDocument)nome.Tag).SourceFilePath = nomeFile;

            //=================================================================
            label2.Text = "Attendere...";
            progressBar1.Value = 0;
            var progress = new Progress<int>(p =>
            {
                progressBar1.Value = progressBar1.Maximum / p;

            });
            var res = await Task.Run(() => ((BaseDocument)nome.Tag).CheckFields(progress));
            label2.Text = "Completato!";
            //=================================================================

            var status = XMLSettings.DocumentStatus.FileOK;
            var msg = "CHECK VALUE!";
            if (res.Count != 0)
            {
                status = XMLSettings.DocumentStatus.FileInError;
                foreach (var m in res.Take(5))  //===> qua magari fagliene vedere 5 alla volta.. così non saturi tutto
                {
                    msg += "\n" + m.ToString();
                }
            }

            var now = DateTime.Now;
            Manager.Settings.UpdateDocument(nome.SubItems[0].Text, extension, nomeFile, null, null, now, status);
            if (status == XMLSettings.DocumentStatus.FileOK)
                MessageBox.Show($"File {nome.SubItems[1].Text} vericato correttamente!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(msg, "Errore!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            LoadDocuments();
        }
        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadDocuments()
        {
            try
            {
                lvReport.Clear();
                cmbChoose.SelectedIndex = 2;

                foreach (var f in Manager.GetDocuments())
                {
                    var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Document, f.Attribute.Name);
                    lvReport.AddRow(0, f.Attribute.Name, f.Attribute.Description, list[1] == null ? "..." : f.Attribute.Name + list[1], list[2] ?? "...", list[3] ?? "...", list[4] ?? "...", list[5] ?? "...");
                    //lvReport.Items[3].ImageKey = ""; (XMLSettings.DocumentStatus)((int)(list[6]??"0"))
                    lvReport.Items[lvReport.Items.Count - 1].Tag = f.Document;
                }

                lvReport.SmallImageList = imageListPiccole;
                lvReport.LargeImageList = imageListGrandi;
                lvReport.MountHeaders(
                        "Nome Documento", 100, HorizontalAlignment.Left,
                        "Descrizione", 180, HorizontalAlignment.Left,
                        "File", 120, HorizontalAlignment.Left,
                        "File di origine", 200, HorizontalAlignment.Left,
                        "Ultimo caricamento", 120, HorizontalAlignment.Right,
                        "Ultima modifica", 120, HorizontalAlignment.Right,
                        "Ultima verifica", 120, HorizontalAlignment.Right);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
