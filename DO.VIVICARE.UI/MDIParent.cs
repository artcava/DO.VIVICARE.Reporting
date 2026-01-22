using System;
using System.Deployment.Application;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;

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
            else
                Text = $"Reporting [{GetApplicationVersion()}]";

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
                                  $"Vuoi scaricare e installare l'aggiornamento ora?\n\n" +
                                  $"Nota: L'applicazione verrà riavviata automaticamente.";

                    var result = MessageBox.Show(
                        message,
                        "Aggiornamento Disponibile",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        // Implementa vero auto-updater
                        await DownloadAndInstallUpdateAsync(updateInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log silently - non mostrare errori all'utente per il check update
                System.Diagnostics.Debug.WriteLine($"Error checking for updates: {ex.Message}");
            }
        }

        /// <summary>
        /// Scarica e installa l'aggiornamento automaticamente
        /// </summary>
        private async System.Threading.Tasks.Task DownloadAndInstallUpdateAsync(
            PluginManager.UpdateInfo updateInfo)
        {
            try
            {
                // Step 1: Download in cartella temporanea
                var tempPath = Path.Combine(
                    Path.GetTempPath(),
                    $"DO.VIVICARE-Update-{updateInfo.AvailableVersion}.zip"
                );

                // Pulisci eventuale file precedente
                if (File.Exists(tempPath))
                    File.Delete(tempPath);

                statusStrip.Text = "Downloading update...";
                Application.DoEvents();

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(5);

                    using (var response = await client.GetAsync(updateInfo.DownloadUrl,
                        HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        var totalBytes = response.Content.Headers.ContentLength ?? 0L;
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var file = new FileStream(tempPath, FileMode.Create))
                        {
                            var buffer = new byte[8192];
                            var totalRead = 0L;
                            int bytesRead;

                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                            {
                                await file.WriteAsync(buffer, 0, bytesRead);
                                totalRead += bytesRead;

                                if (totalBytes > 0)
                                {
                                    var percent = (int)((totalRead * 100L) / totalBytes);
                                    statusStrip.Text = $"Downloading update... {percent}%";
                                }

                                Application.DoEvents();
                            }
                        }
                    }
                }

                statusStrip.Text = "Verifying integrity...";
                Application.DoEvents();

                // Step 2: Verifica checksum (se disponibile)
                if (!string.IsNullOrEmpty(updateInfo.Checksum))
                {
                    if (!VerifyFileChecksum(tempPath, updateInfo.Checksum))
                    {
                        File.Delete(tempPath);
                        MessageBox.Show(
                            "❌ Integrity check failed. Downloaded file may be corrupted.\n\n" +
                            "Please try again later or visit:\n" +
                            "https://github.com/artcava/DO.VIVICARE.Reporting/releases",
                            "Download Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        statusStrip.Text = "Update failed";
                        return;
                    }
                }

                statusStrip.Text = "Installing update...";
                Application.DoEvents();

                // Step 3: Estrai in cartella di installazione
                var installPath = Application.StartupPath;

                try
                {
                    // Estrai il ZIP
                    System.IO.Compression.ZipFile.ExtractToDirectory(
                        tempPath,
                        installPath,
                        overwriteFiles: true
                    );

                    MessageBox.Show(
                        "✅ Update installed successfully!\n\n" +
                        "The application will restart now to apply changes.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Step 4: Riavvia l'app
                    Application.Restart();
                }
                catch (Exception extractEx)
                {
                    File.Delete(tempPath);
                    MessageBox.Show(
                        $"❌ Error extracting files: {extractEx.Message}\n\n" +
                        $"Installation failed. Please manually download from:\n" +
                        $"https://github.com/artcava/DO.VIVICARE.Reporting/releases",
                        "Installation Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    statusStrip.Text = "Installation failed";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error during update: {ex.Message}\n\n" +
                    $"You can manually download from:\n" +
                    $"https://github.com/artcava/DO.VIVICARE.Reporting/releases",
                    "Update Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                statusStrip.Text = "Update failed";
            }
        }

        /// <summary>
        /// Verifica SHA256 checksum del file
        /// </summary>
        private bool VerifyFileChecksum(string filePath, string expectedChecksum)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                using (var sha256 = SHA256.Create())
                using (var fileStream = File.OpenRead(filePath))
                {
                    var hashBytes = sha256.ComputeHash(fileStream);
                    var computedChecksum = BitConverter.ToString(hashBytes)
                        .Replace("-", "")
                        .ToLowerInvariant();

                    var cleanExpected = expectedChecksum
                        .Replace("sha256:", "", StringComparison.OrdinalIgnoreCase)
                        .ToLowerInvariant();

                    return computedChecksum.Equals(cleanExpected, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verifying checksum: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ottieni versione dell'app dal file AssemblyInfo
        /// </summary>
        private string GetApplicationVersion()
        {
            try
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly()
                    .GetName()
                    .Version;
                return version?.ToString(3) ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
