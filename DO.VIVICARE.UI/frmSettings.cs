using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class frmSettings : Form
    {
        WebClient client;
        private const string _path = "http://artcava.azurewebsites.net/reporting/";
        private const string _fileDocuments = "listDocuments.txt";
        private const string _fileReports = "listReports.txt";
        private const string _voce = "Scarica";

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
            dgvElencoDocuments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvElencoDocuments.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            dgvElencoDocuments.Columns["NomeFileDocument"].FillWeight = 150;

            dgvElencoReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvElencoReports.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            dgvElencoReports.Columns["NomeFileReport"].FillWeight = 150;
        }

        /// <summary>
        /// Carica la lista di plugin disponibili dal manifest.json
        /// </summary>
        private async Task LoadPluginManifestAsync()
        {
            try
            {
                lblResult.Text = "Caricamento plugin disponibili dal manifest...";
                _manifest = await _pluginManager.GetManifestAsync();

                if (_manifest != null)
                {
                    PopulateDocumentPlugins(_manifest.Documents);
                    PopulateReportPlugins(_manifest.Reports);
                    lblResult.Text = $"Manifest caricato: {_manifest.Documents.Count} document plugin, {_manifest.Reports.Count} report plugin";
                }
                else
                {
                    lblResult.Text = "Errore nel caricamento del manifest";
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = $"Errore: {ex.Message}";
            }
        }

        /// <summary>
        /// Popola la griglia con i plugin disponibili dal manifest
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
                    : (hasUpdate ? "⬇ Aggiorna" : "✓ Aggiornato");

                var row = new DataGridViewRow();
                var rowIndex = dgvElencoDocuments.Rows.Add(row);
                dgvElencoDocuments.Rows[rowIndex].Cells["NomeFileDocument"].Value = plugin.Name;
                dgvElencoDocuments.Rows[rowIndex].Cells["NomeFileDocumentCompleto"].Value = plugin.Id;
                dgvElencoDocuments.Rows[rowIndex].Cells["DownloadDocument"].Value = status;
                dgvElencoDocuments.Rows[rowIndex].Tag = plugin; // Salva il plugin object nel tag
            }
        }

        /// <summary>
        /// Popola la griglia report
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
                    : (hasUpdate ? "⬇ Aggiorna" : "✓ Aggiornato");

                var row = new DataGridViewRow();
                var rowIndex = dgvElencoReports.Rows.Add(row);
                dgvElencoReports.Rows[rowIndex].Cells["NomeFileReport"].Value = plugin.Name;
                dgvElencoReports.Rows[rowIndex].Cells["NomeFileReportCompleto"].Value = plugin.Id;
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
                        // lblResult per il progress (senza progressBar1 che non esiste nel Designer)
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
        /// Formatta byte in formato leggibile
        /// </summary>
        private string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024):F1} MB";
        }

        private void GetListDocumentsUpdated()
        {
            using (client = new WebClient())
            {
                try
                {
                    Stream stream = client.OpenRead(_path + _fileDocuments);
                    StreamReader reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();

                    string[] sep = new string[] { "\r\n" };
                    string[] lines = content.Split(sep, StringSplitOptions.None);

                    dgvElencoDocuments.Rows.Clear();
                    foreach (var dll in lines)
                    {
                        string[] nome = dll.Split(';');
                        var row = new DataGridViewRow();
                        var rowIndex = dgvElencoDocuments.Rows.Add(row);
                        dgvElencoDocuments.Rows[rowIndex].Cells["NomeFileDocument"].Value = nome[0];
                        dgvElencoDocuments.Rows[rowIndex].Cells["NomeFileDocumentCompleto"].Value = nome[1];
                        dgvElencoDocuments.Rows[rowIndex].Cells["DownloadDocument"].Value = _voce;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void GetListReportsUpdated()
        {
            using (client = new WebClient())
            {
                try
                {
                    Stream stream = client.OpenRead(_path + _fileReports);
                    StreamReader reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();

                    string[] sep = new string[] { "\r\n" };
                    string[] lines = content.Split(sep, StringSplitOptions.None);

                    dgvElencoReports.Rows.Clear();
                    foreach (var dll in lines)
                    {
                        string[] nome = dll.Split(';');
                        var row = new DataGridViewRow();
                        var rowIndex = dgvElencoReports.Rows.Add(row);
                        dgvElencoReports.Rows[rowIndex].Cells["NomeFileReport"].Value = nome[0];
                        dgvElencoReports.Rows[rowIndex].Cells["NomeFileReportCompleto"].Value = nome[1];
                        dgvElencoReports.Rows[rowIndex].Cells["DownloadReport"].Value = _voce;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    lblResult.Text = "Aggiornamento completato!";
                });
            }
            else
                MessageBox.Show(e.Error.Message);

            ((WebClient)sender).Dispose();
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //BeginInvoke((MethodInvoker)delegate {
            //    lblResult.Text = "Aggiornamento in corso... " + e.ProgressPercentage.ToString() + "%";
            //});
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void frmSettings_Shown(object sender, EventArgs e)
        {
            // Carica prima il manifest (nuovo sistema PluginManager)
            _ = LoadPluginManifestAsync();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabControl.TabPages["tabPageDocuments"])
            {
                // Carica dai plugin manager se disponibili
                if (_manifest?.Documents != null && _manifest.Documents.Count > 0)
                {
                    PopulateDocumentPlugins(_manifest.Documents);
                }
                else
                {
                    GetListDocumentsUpdated();
                }
            }
            else if (tabControl.SelectedTab == tabControl.TabPages["tabPageReports"])
            {
                if (_manifest?.Reports != null && _manifest.Reports.Count > 0)
                {
                    PopulateReportPlugins(_manifest.Reports);
                }
                else
                {
                    GetListReportsUpdated();
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
                    // Se esiste il Tag, significa che viene dal manifest
                    if (row.Tag is PluginInfo plugin)
                    {
                        _ = DownloadAndInstallPluginAsync(plugin);
                    }
                    else
                    {
                        // Fallback al sistema legacy
                        Thread thread = new Thread(() =>
                        {
                            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);

                            Uri URL = new Uri(_path + "Doc" + "/" + row.Cells["NomeFileDocumentCompleto"].Value.ToString());
                            client.DownloadFileAsync(URL, Path.Combine(Manager.DocumentLibraries, row.Cells["NomeFileDocumentCompleto"].Value.ToString()));
                        });
                        thread.Start();
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
                    if (row.Tag is PluginInfo plugin)
                    {
                        _ = DownloadAndInstallPluginAsync(plugin);
                    }
                    else
                    {
                        Thread thread = new Thread(() =>
                        {
                            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);

                            Uri URL = new Uri(_path + "Rep" + "/" + row.Cells["NomeFileReportCompleto"].Value.ToString());
                            client.DownloadFileAsync(URL, Path.Combine(Manager.ReportLibraries, row.Cells["NomeFileReportCompleto"].Value.ToString()));
                        });
                        thread.Start();
                    }
                    break;
            }
        }
    }
}
