using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class frmSettings : Form
    {
        private PluginManager _pluginManager;
        private PluginManifest _manifest;

        public frmSettings()
        {
            InitializeComponent();
            SetDataGrid();
            _pluginManager = new PluginManager();
        }

        private void SetDataGrid()
        {
            // Configure Document grid
            dgvElencoDocuments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvElencoDocuments.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            dgvElencoDocuments.Columns["NomeFileDocument"].FillWeight = 150;
            dgvElencoDocuments.Columns["VersionInstalledDocument"].FillWeight = 100;
            dgvElencoDocuments.Columns["HasUpdateDocument"].FillWeight = 100;
            dgvElencoDocuments.Columns["DownloadDocument"].FillWeight = 80;

            // Configure Report grid
            dgvElencoReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvElencoReports.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            dgvElencoReports.Columns["NomeFileReport"].FillWeight = 150;
            dgvElencoReports.Columns["VersionInstalledReport"].FillWeight = 100;
            dgvElencoReports.Columns["HasUpdateReport"].FillWeight = 100;
            dgvElencoReports.Columns["DownloadReport"].FillWeight = 80;
        }

        /// <summary>
        /// Carica la lista di plugin disponibili dal manifest.json di GitHub
        /// </summary>
        private async Task LoadPluginManifestAsync()
        {
            try
            {
                lblResult.Text = "Caricamento plugin disponibili dal manifest GitHub...";
                _manifest = await _pluginManager.GetManifestAsync();

                if (_manifest != null)
                {
                    PopulateDocumentPlugins(_manifest.Documents);
                    PopulateReportPlugins(_manifest.Reports);
                    lblResult.Text = $"Manifest caricato: {_manifest.Documents.Count} document plugin, {_manifest.Reports.Count} report plugin";
                }
                else
                {
                    lblResult.Text = "Errore nel caricamento del manifest da GitHub";
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = $"Errore: {ex.Message}";
                MessageBox.Show($"Errore nel caricamento del manifest:\n{ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Popola la griglia con i plugin documents disponibili dal manifest
        /// </summary>
        private void PopulateDocumentPlugins(List<PluginInfo> plugins)
        {
            if (plugins == null || plugins.Count == 0)
                return;

            var installed = _pluginManager.GetInstalledPlugins();
            dgvElencoDocuments.Rows.Clear();

            foreach (var plugin in plugins)
            {
                // Cerca se il plugin è già installato
                var installedPlugin = installed.FirstOrDefault(
                    p => p.Name.Contains(plugin.Id) || p.Name.Contains(plugin.Name.Replace(" ", "").ToLower()));

                var installedVersion = installedPlugin?.Version ?? "Non installato";
                var hasUpdate = installedVersion != "Non installato" && 
                    _pluginManager.HasUpdate(
                        new PluginInfo { Version = installedVersion },
                        plugin
                    );

                var status = installedVersion == "Non installato" 
                    ? "Download" 
                    : (hasUpdate ? "⬇️ Aggiorna" : "✓ Aggiornato");

                var updateStatus = installedVersion == "Non installato"
                    ? "Non disponibile"
                    : (hasUpdate ? "⬆️ Disponibile" : "✓ No");

                var row = new DataGridViewRow();
                var rowIndex = dgvElencoDocuments.Rows.Add(row);
                dgvElencoDocuments.Rows[rowIndex].Cells["NomeFileDocument"].Value = plugin.Name;
                dgvElencoDocuments.Rows[rowIndex].Cells["NomeFileDocumentCompleto"].Value = plugin.Id;
                dgvElencoDocuments.Rows[rowIndex].Cells["VersionInstalledDocument"].Value = installedVersion;
                dgvElencoDocuments.Rows[rowIndex].Cells["HasUpdateDocument"].Value = updateStatus;
                dgvElencoDocuments.Rows[rowIndex].Cells["DownloadDocument"].Value = status;
                dgvElencoDocuments.Rows[rowIndex].Tag = plugin; // Salva il plugin object nel tag
            }
        }

        /// <summary>
        /// Popola la griglia con i plugin reports disponibili dal manifest
        /// </summary>
        private void PopulateReportPlugins(List<PluginInfo> plugins)
        {
            if (plugins == null || plugins.Count == 0)
                return;

            var installed = _pluginManager.GetInstalledPlugins();
            dgvElencoReports.Rows.Clear();

            foreach (var plugin in plugins)
            {
                var installedPlugin = installed.FirstOrDefault(
                    p => p.Name.Contains(plugin.Id) || p.Name.Contains(plugin.Name.Replace(" ", "").ToLower()));

                var installedVersion = installedPlugin?.Version ?? "Non installato";
                var hasUpdate = installedVersion != "Non installato" && 
                    _pluginManager.HasUpdate(
                        new PluginInfo { Version = installedVersion },
                        plugin
                    );

                var status = installedVersion == "Non installato" 
                    ? "Download" 
                    : (hasUpdate ? "⬇️ Aggiorna" : "✓ Aggiornato");

                var updateStatus = installedVersion == "Non installato"
                    ? "Non disponibile"
                    : (hasUpdate ? "⬆️ Disponibile" : "✓ No");

                var row = new DataGridViewRow();
                var rowIndex = dgvElencoReports.Rows.Add(row);
                dgvElencoReports.Rows[rowIndex].Cells["NomeFileReport"].Value = plugin.Name;
                dgvElencoReports.Rows[rowIndex].Cells["NomeFileReportCompleto"].Value = plugin.Id;
                dgvElencoReports.Rows[rowIndex].Cells["VersionInstalledReport"].Value = installedVersion;
                dgvElencoReports.Rows[rowIndex].Cells["HasUpdateReport"].Value = updateStatus;
                dgvElencoReports.Rows[rowIndex].Cells["DownloadReport"].Value = status;
                dgvElencoReports.Rows[rowIndex].Tag = plugin;
            }
        }

        /// <summary>
        /// Scarica e installa un plugin con progress tracking
        /// </summary>
        private async Task DownloadAndInstallPluginAsync(PluginInfo plugin)
        {
            try
            {
                var progress = new Progress<DownloadProgress>(p =>
                {
                    BeginInvoke((MethodInvoker)delegate
                    {
                        lblResult.Text = $"Download {plugin.Name}... {p.PercentComplete}% ({FormatBytes(p.BytesDownloaded)} / {FormatBytes(p.TotalBytes)})";
                    });
                });

                var success = await _pluginManager.DownloadPluginAsync(plugin, progress, CancellationToken.None);

                BeginInvoke((MethodInvoker)delegate
                {
                    if (success)
                    {
                        lblResult.Text = $"{plugin.Name} v{plugin.Version} installato con successo! (Hot-reload: nessun riavvio richiesto)";
                        MessageBox.Show(
                            $"{plugin.Name} v{plugin.Version} installato con successo!\n\nNessun riavvio dell'applicazione richiesto (hot-reload enabled).",
                            "Successo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        // Ricarica la lista
                        _ = LoadPluginManifestAsync();
                    }
                    else
                    {
                        lblResult.Text = $"Errore nell'installazione di {plugin.Name}";
                        MessageBox.Show(
                            $"Impossibile installare {plugin.Name}\n\nVerificare la connessione di rete e riprovare.",
                            "Errore",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                });
            }
            catch (Exception ex)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    lblResult.Text = $"Errore: {ex.Message}";
                    MessageBox.Show($"Errore: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }

        /// <summary>
        /// Formatta byte in formato leggibile (B, KB, MB)
        /// </summary>
        private string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024):F1} MB";
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void frmSettings_Shown(object sender, EventArgs e)
        {
            // Carica il manifest da GitHub all'apertura del form
            _ = LoadPluginManifestAsync();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Quando cambia tab, ripopola se il manifest è stato caricato
            if (tabControl.SelectedTab == tabControl.TabPages["tabPageDocuments"])
            {
                if (_manifest?.Documents != null && _manifest.Documents.Count > 0)
                {
                    PopulateDocumentPlugins(_manifest.Documents);
                }
            }
            else if (tabControl.SelectedTab == tabControl.TabPages["tabPageReports"])
            {
                if (_manifest?.Reports != null && _manifest.Reports.Count > 0)
                {
                    PopulateReportPlugins(_manifest.Reports);
                }
            }
        }

        private void dgvElencoDocuments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            lblResult.Text = string.Empty;
            if (e.ColumnIndex < 0) return;
            DataGridViewRow row = dgvElencoDocuments.Rows[dgvElencoDocuments.CurrentCell.RowIndex];

            switch (dgvElencoDocuments.Columns[e.ColumnIndex].Name)
            {
                case "DownloadDocument":
                    // Usa sempre il PluginManager dal manifest GitHub
                    if (row.Tag is PluginInfo plugin)
                    {
                        _ = DownloadAndInstallPluginAsync(plugin);
                    }
                    break;
            }
        }

        private void dgvElencoReports_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            lblResult.Text = string.Empty;
            if (e.ColumnIndex < 0) return;
            DataGridViewRow row = dgvElencoReports.Rows[dgvElencoReports.CurrentCell.RowIndex];

            switch (dgvElencoReports.Columns[e.ColumnIndex].Name)
            {
                case "DownloadReport":
                    // Usa sempre il PluginManager dal manifest GitHub
                    if (row.Tag is PluginInfo plugin)
                    {
                        _ = DownloadAndInstallPluginAsync(plugin);
                    }
                    break;
            }
        }
    }
}
