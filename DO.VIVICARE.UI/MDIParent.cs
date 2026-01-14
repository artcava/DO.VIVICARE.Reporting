using System;
using System.Deployment.Application;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class MDIParent : Form
    {
        private int childFormNumber = 0;

        public MDIParent()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Finestra " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "File di testo (*.txt)|*.txt|Tutti i file (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "File di testo (*.txt)|*.txt|Tutti i file (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSettings f = new frmSettings();
            f.ShowDialog();
        }

        private void archivioToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                if (childForm.GetType() == typeof(frmReports))
                {

                    childForm.Focus();
                    return;
                }
            }
            frmReports f = new frmReports() { MdiParent = this };
            f.Show();
        }

        private void documentiToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                if (childForm.GetType() == typeof(frmDocuments))
                {
                    childForm.Focus();
                    return;
                }
            }
            frmDocuments f = new frmDocuments() { MdiParent = this };
            f.Show();
        }

        private void documentiToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private async void MDIParent_Load(object sender, EventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
                Text = $"Reporting [{ApplicationDeployment.CurrentDeployment.CurrentVersion}]";

            // Check for application updates
            await CheckForApplicationUpdatesAsync();
        }

        /// <summary>
        /// Controlla se è disponibile una nuova versione dell'applicazione e notifica l'utente
        /// </summary>
        private async System.Threading.Tasks.Task CheckForApplicationUpdatesAsync()
        {
            try
            {
                var pluginManager = new PluginManager();
                var updateInfo = await pluginManager.CheckAppUpdateAsync();

                if (updateInfo != null)
                {
                    var message = $"È disponibile una nuova versione: {updateInfo.AvailableVersion}\n\n" +
                                  $"Versione corrente: {updateInfo.CurrentVersion}\n" +
                                  $"Data rilascio: {updateInfo.ReleaseDate}\n\n" +
                                  $"Vuoi scaricare l'aggiornamento ora?";

                    var result = MessageBox.Show(
                        message,
                        "Aggiornamento Disponibile",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        // Apri il browser per scaricare l'aggiornamento
                        System.Diagnostics.Process.Start(updateInfo.DownloadUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log silently - non mostrare errori all'utente per il check update
                System.Diagnostics.Debug.WriteLine($"Error checking for updates: {ex.Message}");
            }
        }
    }
}
