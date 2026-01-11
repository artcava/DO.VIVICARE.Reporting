# C# Plugin Manager Implementation - Complete Guide

## Descrizione

Questo documento contiene l'implementazione completa del sistema di gestione plugin per DO.VIVICARE, con le seguenti funzionalità:

1. **Download di plugin indipendenti** da GitHub Releases
2. **Verifica di integrità** tramite SHA256
3. **Gestione versioni** separate per app e plugin
4. **Hot-reload** senza riavvio dell'app
5. **UI integrata** con Windows Forms
6. **Gestione delle dipendenze** tra plugin

---

## Step 1: Core Plugin Manager (✅ Completato)

```csharp
// PluginManager.cs
public class PluginManager
{
    private const string MANIFEST_URL = 
        "https://raw.githubusercontent.com/artcava/DO.VIVICARE.Reporting/master/manifest.json";
    
    private const string GITHUB_RELEASES = 
        "https://api.github.com/repos/artcava/DO.VIVICARE.Reporting/releases";
    
    private readonly string _pluginDirectory;
    private List<PluginInfo> _plugins;

    public PluginManager(string pluginDirectory = null)
    {
        _pluginDirectory = pluginDirectory ?? 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), 
                        "DO.VIVICARE", "Plugins");
        
        if (!Directory.Exists(_pluginDirectory))
            Directory.CreateDirectory(_pluginDirectory);
    }

    /// <summary>
    /// Scarica il manifest con lista di tutti i plugin disponibili
    /// </summary>
    public async Task<PluginManifest> GetManifestAsync()
    {
        try
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE");
                
                var response = await client.GetAsync(MANIFEST_URL);
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                var manifest = JsonConvert.DeserializeObject<PluginManifest>(json);
                
                return manifest;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading manifest: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Confronta versione installata con quella disponibile
    /// </summary>
    public bool HasUpdate(PluginInfo installed, PluginInfo available)
    {
        if (installed == null || available == null)
            return false;
        
        if (!Version.TryParse(installed.Version, out var installedVersion) ||
            !Version.TryParse(available.Version, out var availableVersion))
            return false;
        
        return availableVersion > installedVersion;
    }

    /// <summary>
    /// Scarica un plugin con progress tracking
    /// </summary>
    public async Task<bool> DownloadPluginAsync(
        PluginInfo plugin,
        IProgress<DownloadProgress> progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE");
                
                // Download con progress
                var response = await client.GetAsync(
                    plugin.DownloadUrl, 
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);
                
                response.EnsureSuccessStatusCode();
                
                var totalBytes = response.Content.Headers.ContentLength ?? 0L;
                var filePath = Path.Combine(_pluginDirectory, plugin.FileName);
                
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
                {
                    var totalRead = 0L;
                    var buffer = new byte[8192];
                    int bytesRead;
                    
                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                        totalRead += bytesRead;
                        
                        progress?.Report(new DownloadProgress
                        {
                            PluginId = plugin.Id,
                            BytesDownloaded = totalRead,
                            TotalBytes = totalBytes,
                            PercentComplete = (int)(totalRead * 100 / Math.Max(totalBytes, 1))
                        });
                    }
                }
                
                // Verifica checksum
                if (!await VerifyChecksumAsync(filePath, plugin.Checksum))
                {
                    File.Delete(filePath);
                    throw new Exception($"Checksum verification failed for {plugin.Name}");
                }
                
                return true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error downloading plugin: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Verifica integrità file tramite SHA256
    /// </summary>
    private async Task<bool> VerifyChecksumAsync(string filePath, string expectedChecksum)
    {
        try
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = sha256.ComputeHash(stream);
                var computedChecksum = "sha256:" + BitConverter.ToString(hash).Replace("-", "").ToLower();
                
                return computedChecksum == expectedChecksum.ToLower();
            }
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Carica un DLL plugin in memoria (hot-reload)
    /// </summary>
    public Assembly LoadPluginAssembly(string pluginId)
    {
        try
        {
            var pluginFile = Directory.GetFiles(_pluginDirectory)
                .FirstOrDefault(f => f.Contains(pluginId));
            
            if (pluginFile == null)
                throw new FileNotFoundException($"Plugin {pluginId} not found");
            
            var assemblyName = AssemblyName.GetAssemblyName(pluginFile);
            return Assembly.Load(assemblyName);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading plugin: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Ottiene lista di plugin installati
    /// </summary>
    public List<InstalledPlugin> GetInstalledPlugins()
    {
        var installed = new List<InstalledPlugin>();
        
        try
        {
            var files = Directory.GetFiles(_pluginDirectory, "*.dll");
            
            foreach (var file in files)
            {
                try
                {
                    var assembly = AssemblyName.GetAssemblyName(file);
                    installed.Add(new InstalledPlugin
                    {
                        Name = assembly.Name,
                        Version = assembly.Version?.ToString() ?? "Unknown",
                        FilePath = file,
                        FileSize = new FileInfo(file).Length
                    });
                }
                catch { /* Skip invalid assemblies */ }
            }
        }
        catch { /* Directory may not exist */ }
        
        return installed;
    }

    /// <summary>
    /// Verifica e risolve dipendenze plugin
    /// </summary>
    public List<PluginInfo> ResolveDependencies(PluginInfo plugin, PluginManifest manifest)
    {
        var dependencies = new List<PluginInfo> { plugin };
        var allPlugins = new List<PluginInfo>();
        allPlugins.AddRange(manifest.Documents);
        allPlugins.AddRange(manifest.Reports);
        
        foreach (var depId in plugin.Dependencies ?? new List<string>())
        {
            var dep = allPlugins.FirstOrDefault(p => p.Id == depId);
            if (dep != null && !dependencies.Any(d => d.Id == depId))
            {
                dependencies.AddRange(ResolveDependencies(dep, manifest));
            }
        }
        
        return dependencies;
    }

    /// <summary>
    /// Uninstalla un plugin
    /// </summary>
    public bool UninstallPlugin(string pluginId)
    {
        try
        {
            var pluginFile = Directory.GetFiles(_pluginDirectory)
                .FirstOrDefault(f => f.Contains(pluginId));
            
            if (pluginFile != null)
            {
                File.Delete(pluginFile);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error uninstalling plugin: {ex.Message}");
            return false;
        }
    }
}
```

---

## Step 2: Model Classes (✅ Completato)

```csharp
/// <summary>
/// Model classes for plugin management
/// </summary>
public class PluginManifest
{
    public AppInfo App { get; set; }
    public List<PluginInfo> Documents { get; set; }
    public List<PluginInfo> Reports { get; set; }
}

public class AppInfo
{
    [JsonProperty("version")]
    public string Version { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("downloadUrl")]
    public string DownloadUrl { get; set; }
    
    [JsonProperty("checksum")]
    public string Checksum { get; set; }
    
    [JsonProperty("releaseDate")]
    public string ReleaseDate { get; set; }
}

public class PluginInfo
{
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("description")]
    public string Description { get; set; }
    
    [JsonProperty("version")]
    public string Version { get; set; }
    
    [JsonProperty("downloadUrl")]
    public string DownloadUrl { get; set; }
    
    [JsonProperty("checksum")]
    public string Checksum { get; set; }
    
    [JsonProperty("fileSize")]
    public long FileSize { get; set; }
    
    [JsonProperty("releaseDate")]
    public string ReleaseDate { get; set; }
    
    [JsonProperty("dependencies")]
    public List<string> Dependencies { get; set; } = new List<string>();
    
    [JsonIgnore]
    public string FileName => $"{Id.Replace(".", "-")}-{Version}.dll";
}

public class InstalledPlugin
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; }
}

public class DownloadProgress
{
    public string PluginId { get; set; }
    public long BytesDownloaded { get; set; }
    public long TotalBytes { get; set; }
    public int PercentComplete { get; set; }
}
```

---

## Step 3: UI Implementation (🚀 NUOVO)

### frmPluginManager.cs - Form principale

```csharp
public partial class frmPluginManager : Form
{
    private PluginManager _pluginManager;
    private PluginManifest _manifest;
    private CancellationTokenSource _cancellationTokenSource;

    public frmPluginManager()
    {
        InitializeComponent();
        _pluginManager = new PluginManager();
        _cancellationTokenSource = new CancellationTokenSource();
        SetupUI();
    }

    private void SetupUI()
    {
        // Configure DataGridViews
        ConfigureDataGridView(dgvDocuments);
        ConfigureDataGridView(dgvReports);
        
        // Setup buttons
        btnDownload.Click += BtnDownload_Click;
        btnUninstall.Click += BtnUninstall_Click;
        btnRefresh.Click += BtnRefresh_Click;
        btnCancel.Click += BtnCancel_Click;
        
        // Setup progress bar
        progressBar.Style = ProgressBarStyle.Continuous;
        progressBar.Visible = false;
    }

    private void ConfigureDataGridView(DataGridView dgv)
    {
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.AllowUserToResizeRows = false;
        dgv.ReadOnly = true;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect = false;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
    }

    private async void frmPluginManager_Load(object sender, EventArgs e)
    {
        await RefreshPluginsAsync();
    }

    private async Task RefreshPluginsAsync()
    {
        try
        {
            btnRefresh.Enabled = false;
            lblStatus.Text = "Loading plugins...";
            
            _manifest = await _pluginManager.GetManifestAsync();
            
            if (_manifest != null)
            {
                PopulateDocumentGrid(_manifest.Documents);
                PopulateReportGrid(_manifest.Reports);
                lblStatus.Text = $"Ready - {_manifest.Documents.Count} document plugins, {_manifest.Reports.Count} report plugins";
            }
            else
            {
                MessageBox.Show("Failed to load plugin manifest", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Error loading plugins";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Error";
        }
        finally
        {
            btnRefresh.Enabled = true;
        }
    }

    private void PopulateDocumentGrid(List<PluginInfo> plugins)
    {
        var installed = _pluginManager.GetInstalledPlugins();
        var data = new List<dynamic>();
        
        foreach (var plugin in plugins)
        {
            var installedPlugin = installed.FirstOrDefault(p => p.Name.Contains(plugin.Id));
            var installedVersion = installedPlugin?.Version ?? "Not Installed";
            
            var hasUpdate = !string.IsNullOrEmpty(installedVersion) && installedVersion != "Not Installed"
                ? _pluginManager.HasUpdate(
                    new PluginInfo { Version = installedVersion },
                    plugin
                )
                : false;
            
            var status = string.IsNullOrEmpty(installedVersion) || installedVersion == "Not Installed"
                ? "Not Installed"
                : hasUpdate ? "⬇ Update Available" : "✓ Up to date";
            
            data.Add(new
            {
                Name = plugin.Name,
                Installed = installedVersion,
                Available = plugin.Version,
                Size = FormatFileSize(plugin.FileSize),
                Status = status,
                Description = plugin.Description,
                PluginId = plugin.Id,
                DownloadUrl = plugin.DownloadUrl
            });
        }
        
        dgvDocuments.DataSource = new BindingSource(data, null);
    }

    private void PopulateReportGrid(List<PluginInfo> plugins)
    {
        var installed = _pluginManager.GetInstalledPlugins();
        var data = new List<dynamic>();
        
        foreach (var plugin in plugins)
        {
            var installedPlugin = installed.FirstOrDefault(p => p.Name.Contains(plugin.Id));
            var installedVersion = installedPlugin?.Version ?? "Not Installed";
            
            var hasUpdate = !string.IsNullOrEmpty(installedVersion) && installedVersion != "Not Installed"
                ? _pluginManager.HasUpdate(
                    new PluginInfo { Version = installedVersion },
                    plugin
                )
                : false;
            
            var status = string.IsNullOrEmpty(installedVersion) || installedVersion == "Not Installed"
                ? "Not Installed"
                : hasUpdate ? "⬇ Update Available" : "✓ Up to date";
            
            data.Add(new
            {
                Name = plugin.Name,
                Installed = installedVersion,
                Available = plugin.Version,
                Size = FormatFileSize(plugin.FileSize),
                Status = status,
                Description = plugin.Description,
                PluginId = plugin.Id,
                DownloadUrl = plugin.DownloadUrl
            });
        }
        
        dgvReports.DataSource = new BindingSource(data, null);
    }

    private async void BtnDownload_Click(object sender, EventArgs e)
    {
        DataGridView activeGrid = GetActiveGrid();
        if (activeGrid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a plugin to download", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedRow = activeGrid.SelectedRows[0];
        var pluginId = selectedRow.Cells["PluginId"].Value?.ToString();
        
        var plugin = _manifest?.Documents.FirstOrDefault(p => p.Id == pluginId)
                  ?? _manifest?.Reports.FirstOrDefault(p => p.Id == pluginId);
        
        if (plugin != null)
        {
            await DownloadAndInstallPluginAsync(plugin);
        }
    }

    private void BtnUninstall_Click(object sender, EventArgs e)
    {
        DataGridView activeGrid = GetActiveGrid();
        if (activeGrid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a plugin to uninstall", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedRow = activeGrid.SelectedRows[0];
        var pluginId = selectedRow.Cells["PluginId"].Value?.ToString();
        
        if (DialogResult.Yes == MessageBox.Show(
            $"Are you sure you want to uninstall this plugin? No restart required.",
            "Confirm",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question))
        {
            if (_pluginManager.UninstallPlugin(pluginId))
            {
                MessageBox.Show("Plugin uninstalled successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshPluginsAsync();
            }
            else
            {
                MessageBox.Show("Failed to uninstall plugin", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnRefresh_Click(object sender, EventArgs e)
    {
        await RefreshPluginsAsync();
    }

    private void BtnCancel_Click(object sender, EventArgs e)
    {
        if (progressBar.Visible)
        {
            _cancellationTokenSource.Cancel();
            progressBar.Visible = false;
            btnDownload.Enabled = true;
            btnUninstall.Enabled = true;
            lblStatus.Text = "Download cancelled";
        }
    }

    private DataGridView GetActiveGrid()
    {
        return tabControl.SelectedTab == tabDocuments ? dgvDocuments : dgvReports;
    }

    private async Task DownloadAndInstallPluginAsync(PluginInfo plugin)
    {
        try
        {
            progressBar.Visible = true;
            progressBar.Value = 0;
            btnDownload.Enabled = false;
            btnUninstall.Enabled = false;
            _cancellationTokenSource = new CancellationTokenSource();
            
            var progress = new Progress<DownloadProgress>(p =>
            {
                progressBar.Value = Math.Min(p.PercentComplete, 100);
                lblStatus.Text = $"Downloading {plugin.Name}... {p.PercentComplete}%";
            });
            
            var success = await _pluginManager.DownloadPluginAsync(
                plugin,
                progress,
                _cancellationTokenSource.Token
            );
            
            if (success)
            {
                MessageBox.Show(
                    $"{plugin.Name} v{plugin.Version} installed successfully!\n\nNo restart required.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                
                await RefreshPluginsAsync();
            }
            else
            {
                MessageBox.Show(
                    $"Failed to install {plugin.Name}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        catch (OperationCanceledException)
        {
            lblStatus.Text = "Download cancelled by user";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            progressBar.Visible = false;
            btnDownload.Enabled = true;
            btnUninstall.Enabled = true;
        }
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _cancellationTokenSource?.Cancel();
        base.OnFormClosing(e);
    }
}
```

### frmPluginManager.Designer.cs - UI Designer

```csharp
namespace DO.VIVICARE.UI
{
    partial class frmPluginManager
    {
        private System.ComponentModel.IContainer components = null;
        private TabControl tabControl;
        private TabPage tabDocuments;
        private TabPage tabReports;
        private DataGridView dgvDocuments;
        private DataGridView dgvReports;
        private Button btnDownload;
        private Button btnUninstall;
        private Button btnRefresh;
        private Button btnCancel;
        private ProgressBar progressBar;
        private Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabDocuments = new System.Windows.Forms.TabPage();
            this.tabReports = new System.Windows.Forms.TabPage();
            this.dgvDocuments = new System.Windows.Forms.DataGridView();
            this.dgvReports = new System.Windows.Forms.DataGridView();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnUninstall = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();

            this.tabControl.SuspendLayout();
            this.tabDocuments.SuspendLayout();
            this.tabReports.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDocuments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).BeginInit();
            this.SuspendLayout();

            // Tab Control
            this.tabControl.Controls.Add(this.tabDocuments);
            this.tabControl.Controls.Add(this.tabReports);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ItemSize = new System.Drawing.Size(100, 25);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(900, 500);
            this.tabControl.TabIndex = 0;

            // Tab Documents
            this.tabDocuments.Controls.Add(this.dgvDocuments);
            this.tabDocuments.Location = new System.Drawing.Point(4, 29);
            this.tabDocuments.Name = "tabDocuments";
            this.tabDocuments.Padding = new System.Windows.Forms.Padding(3);
            this.tabDocuments.Size = new System.Drawing.Size(892, 467);
            this.tabDocuments.TabIndex = 0;
            this.tabDocuments.Text = "Document Plugins";
            this.tabDocuments.UseVisualStyleBackColor = true;

            // Tab Reports
            this.tabReports.Controls.Add(this.dgvReports);
            this.tabReports.Location = new System.Drawing.Point(4, 29);
            this.tabReports.Name = "tabReports";
            this.tabReports.Padding = new System.Windows.Forms.Padding(3);
            this.tabReports.Size = new System.Drawing.Size(892, 467);
            this.tabReports.TabIndex = 1;
            this.tabReports.Text = "Report Plugins";
            this.tabReports.UseVisualStyleBackColor = true;

            // DataGridView Documents
            this.dgvDocuments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDocuments.Location = new System.Drawing.Point(3, 3);
            this.dgvDocuments.Name = "dgvDocuments";
            this.dgvDocuments.Size = new System.Drawing.Size(886, 461);
            this.dgvDocuments.TabIndex = 0;

            // DataGridView Reports
            this.dgvReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReports.Location = new System.Drawing.Point(3, 3);
            this.dgvReports.Name = "dgvReports";
            this.dgvReports.Size = new System.Drawing.Size(886, 461);
            this.dgvReports.TabIndex = 0;

            // Buttons
            this.btnDownload.Location = new System.Drawing.Point(12, 515);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(100, 30);
            this.btnDownload.TabIndex = 1;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;

            this.btnUninstall.Location = new System.Drawing.Point(118, 515);
            this.btnUninstall.Name = "btnUninstall";
            this.btnUninstall.Size = new System.Drawing.Size(100, 30);
            this.btnUninstall.TabIndex = 2;
            this.btnUninstall.Text = "Uninstall";
            this.btnUninstall.UseVisualStyleBackColor = true;

            this.btnRefresh.Location = new System.Drawing.Point(224, 515);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;

            this.btnCancel.Location = new System.Drawing.Point(330, 515);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;

            // Progress Bar
            this.progressBar.Location = new System.Drawing.Point(12, 560);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(876, 23);
            this.progressBar.TabIndex = 5;
            this.progressBar.Visible = false;

            // Status Label
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 590);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(35, 13);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Ready";

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 620);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnUninstall);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblStatus);
            this.Name = "frmPluginManager";
            this.Text = "Plugin Manager";
            this.Load += new System.EventHandler(this.frmPluginManager_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);

            this.tabControl.ResumeLayout(false);
            this.tabDocuments.ResumeLayout(false);
            this.tabReports.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDocuments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
```

---

## Integrazione in frmSettings.cs

```csharp
public partial class frmSettings : Form
{
    private frmPluginManager _pluginManager;

    // ...

    private void InitializePluginMenu()
    {
        // Aggiungi voce di menu per Plugin Manager
        var toolsMenu = menuStrip.Items.Cast<ToolStripItem>()
            .FirstOrDefault(x => x.Text == "Tools") as ToolStripDropDown;
        
        if (toolsMenu != null)
        {
            var pluginManagerItem = new ToolStripMenuItem("Plugin Manager");
            pluginManagerItem.Click += (s, e) => OpenPluginManager();
            toolsMenu.DropDownItems.Add(pluginManagerItem);
        }
    }

    private void OpenPluginManager()
    {
        if (_pluginManager == null || _pluginManager.IsDisposed)
        {
            _pluginManager = new frmPluginManager();
        }
        
        _pluginManager.Show();
        _pluginManager.BringToFront();
    }
}
```

---

## Deployment Checklist

- [ ] Aggiungi NuGet package: `Newtonsoft.Json`
- [ ] Copia `PluginManager.cs` nel progetto UI
- [ ] Copia `frmPluginManager.cs` e `.Designer.cs` nel progetto UI
- [ ] Aggiungi tutti i model classes al progetto
- [ ] Integra `InitializePluginMenu()` in frmSettings
- [ ] Testa download di un plugin
- [ ] Testa uninstall di un plugin
- [ ] Verifica hot-reload senza riavvio
- [ ] Testa progress bar e cancellation
- [ ] Verifica checksum validation

---

**Documento creato**: 11 Gennaio 2026 - 19:50 CET
**Status**: ✅ Step 3 Completato - Pronto per testing
