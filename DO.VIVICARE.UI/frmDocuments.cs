using DO.VIVICARE.Reporter;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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
                case 0: // "LARGE ICONS"
                    lvReport.View = View.LargeIcon;
                    break;
                case 1: // "SMALL ICONS"
                    lvReport.View = View.SmallIcon;
                    break;
                case 2: // "DETAILS"
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
            // Opens the file with Excel separately
            var nome = lvReport.SelectedItems[0];
            if (nome.SubItems[2].Text == "...")
                MessageBox.Show($"You haven't loaded any file for document [{nome.SubItems[0].Text}] yet!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                System.Diagnostics.Process.Start(Path.Combine(Manager.Documents, nome.SubItems[2].Text));
        }

        private void caricaFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Upload new file to archive, overwriting it
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
                    label2.Text = "Please wait...";
                    progressBar1.Value = 0;
                    var progress = new Progress<int>(p =>
                    {
                        progressBar1.Value = progressBar1.Maximum / p;

                    });
                    Cursor.Current = Cursors.WaitCursor;
                    //var ok = await Task.Run(() => ((BaseDocument)nome.Tag).CheckFields(progress));
                    var ok = ((BaseDocument)nome.Tag).CheckFields(progress);
                    Cursor.Current = Cursors.Default;
                    label2.Text = "Completed!";
                    //=================================================================

                    var status = XMLSettings.DocumentStatus.FileOK;
                    var msg = "CHECK VALUE!";
                    if (!ok)
                    {
                        status = XMLSettings.DocumentStatus.FileInError;
                        // Load the generated log file from Path.Combine(Manager.Documents, nome.SubItems[0].Text + ".log")
                        msg += "Error loading file";
                        //foreach (var m in ok.Take(5))  // Show 5 at a time to avoid saturation
                        //{
                        //    msg += "\n" + m.ToString();
                        //}
                        var pippo = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    }

                    File.Copy(nomeFile, Path.Combine(Manager.Documents, nome.SubItems[0].Text + extension), true);
                    var now = DateTime.Now;
                    Manager.Settings.UpdateDocument(nome.SubItems[0].Text, extension, nomeFile, now, now, now, status);
                    if (ok)
                        MessageBox.Show($"File {nome.SubItems[1].Text} loaded successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show(msg, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    LoadDocuments();
                }
            }
        }
        private void apriFileDiorigineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Opens the original file with Excel separately
            var nome = lvReport.SelectedItems[0];
            if (nome.SubItems[3].Text == "...")
                MessageBox.Show($"You haven't loaded any file for document [{nome.SubItems[0].Text}] yet!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                System.Diagnostics.Process.Start(nome.SubItems[3].Text);
        }

        private async void verificaFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Verify file in archive
            var nome = lvReport.SelectedItems[0];
            if (nome.SubItems[2].Text == "...")
            {
                MessageBox.Show($"You haven't loaded any file for document [{nome.SubItems[0].Text}] yet!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nomeFile = Path.Combine(Manager.Documents, nome.SubItems[2].Text);
            var extension = nomeFile.Substring(nomeFile.LastIndexOf('.'));
            ((BaseDocument)nome.Tag).SourceFilePath = nomeFile;

            //=================================================================
            label2.Text = "Please wait...";
            progressBar1.Value = 0;
            var progress = new Progress<int>(p =>
            {
                progressBar1.Value = progressBar1.Maximum / p;

            });
            var res = await Task.Run(() => ((BaseDocument)nome.Tag).CheckFields(progress));
            label2.Text = "Completed!";
            //=================================================================

            var status = XMLSettings.DocumentStatus.FileOK;
            var msg = "CHECK VALUE!";
            if (!res)
            {
                status = XMLSettings.DocumentStatus.FileInError;
                // Load a new form with the list of detected errors (to be taken from the log)
                //foreach (var m in res.Take(5))  // Show 5 at a time to avoid saturation
                //{
                //    msg += "\n" + m.ToString();
                //}
            }

            var now = DateTime.Now;
            Manager.Settings.UpdateDocument(nome.SubItems[0].Text, extension, nomeFile, null, null, now, status);
            if (status == XMLSettings.DocumentStatus.FileOK)
                MessageBox.Show($"File {nome.SubItems[1].Text} verified successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(msg, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            LoadDocuments();
        }

        private void legendaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form childForm = new frmDocumentLegend($"Legend {lvReport.SelectedItems[0].Text}", (BaseDocument)lvReport.SelectedItems[0].Tag);
            childForm.MdiParent = this.MdiParent;
            childForm.Text = $"Legend {lvReport.SelectedItems[0].Text}";
            childForm.Show();
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
                        "Document Name", 100, HorizontalAlignment.Left,
                        "Description", 180, HorizontalAlignment.Left,
                        "File", 120, HorizontalAlignment.Left,
                        "Source File", 200, HorizontalAlignment.Left,
                        "Last Upload", 120, HorizontalAlignment.Right,
                        "Last Modification", 120, HorizontalAlignment.Right,
                        "Last Verification", 120, HorizontalAlignment.Right);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
