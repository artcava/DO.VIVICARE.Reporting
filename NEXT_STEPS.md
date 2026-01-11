# DO.VIVICARE v1.2.0 - Implementation Roadmap

**Status**: Architecture finalized ✅  
**Current Date**: 11 Gennaio 2026  
**Next Phase**: Development & Integration

---

## 📋 **What's Complete**

- ✅ `INSTALLATION_GUIDE.md` - Complete user & developer documentation
- ✅ `manifest.json` - Full plugin registry with all 16 plugins
- ✅ `.github/workflows/release.yml` - Dual-mode CI/CD (app + plugin)
- ✅ `PluginManager.cs` (documentation) - C# implementation template
- ✅ Architecture decision - Single installer + independent plugins
- ✅ Versioning strategy - App unified, plugins independent

---

## 🚀 **Next Steps (In Order)**

### **Phase 1: Prepare Release v1.2.0 (THIS WEEK)**

#### Step 1.1: Update AssemblyInfo.cs Files

**For UI and Reporter (version must be 1.2.0):**

```csharp
// DO.VIVICARE.UI/Properties/AssemblyInfo.cs
[assembly: AssemblyVersion("1.2.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]

// DO.VIVICARE.Reporter/Properties/AssemblyInfo.cs  
[assembly: AssemblyVersion("1.2.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]
```

**For Plugin libraries (each can have independent versions - use 1.0.0 as baseline):**

```csharp
// DO.VIVICARE.Document.ADIAltaIntensita/Properties/AssemblyInfo.cs
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// DO.VIVICARE.Report.Dietetica/Properties/AssemblyInfo.cs
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// ... repeat for all 16 plugin libraries
```

**Commands:**
```bash
cd DO.VIVICARE.Reporting

# Update all files at once (PowerShell)
Get-ChildItem -Recurse -Include "AssemblyInfo.cs" | ForEach-Object {
    Write-Host "Updating: $_"
}
```

#### Step 1.2: Commit & Tag for Release

```bash
# 1. Commit all changes
git add .
git commit -m "v1.2.0: Final release with plugin management system"

# 2. Push to master
git push origin master

# 3. Create tag (triggers GitHub Actions)
git tag -a v1.2.0 -m "DO.VIVICARE Reporting v1.2.0 - Single installer with plugin manager"

# 4. Push tag to trigger workflow
git push origin v1.2.0
```

**Expected Result:**
- GitHub Actions builds automatically
- Creates `DO.VIVICARE-Setup-1.2.0.zip` (installer)
- Generates `CHECKSUM.txt`
- Creates GitHub Release with both files
- ✅ Release available at: https://github.com/artcava/DO.VIVICARE.Reporting/releases/tag/v1.2.0

---

### **Phase 2: Implement PluginManager (Week 2)**

#### Step 2.1: Create PluginManager.cs in Project

**Location**: `DO.VIVICARE.UI/Services/PluginManager.cs`

**Copy the code from the template** (see below) into this file.

**Required NuGet Packages:**
```bash
NuGet\Install-Package Newtonsoft.Json -Version 13.0.3
```

#### Step 2.2: Add Plugin Manager Tab to frmSettings

**In Visual Studio Designer:**

1. Open `frmSettings.cs` [Design]
2. Add new `TabPage`:
   - Name: `tabPlugins`
   - Text: "Plugin Manager"
3. Add controls to tab:
   ```
   TabPage: tabPlugins
   ├─ Label: lblPlugins ("Available Plugins")
   ├─ DataGridView: dgvPlugins
   │  ├─ Columns: Name, Version, Available, Size, [Download]
   ├─ ProgressBar: pbDownload
   ├─ Label: lblStatus
   └─ Button: btnRefresh
   ```

**In frmSettings.cs [Code]:**

```csharp
private PluginManager _pluginManager;
private PluginManifest _manifest;

public frmSettings()
{
    InitializeComponent();
    _pluginManager = new PluginManager();
}

private async void frmSettings_Load(object sender, EventArgs e)
{
    // Load plugins when form opens
    await RefreshPluginsAsync();
}

private async Task RefreshPluginsAsync()
{
    try
    {
        lblStatus.Text = "Loading plugins...";
        this.Cursor = Cursors.WaitCursor;
        
        _manifest = await _pluginManager.GetManifestAsync();
        
        if (_manifest != null)
        {
            PopulatePluginsGrid(_manifest.Documents, _manifest.Reports);
        }
        
        lblStatus.Text = "Ready";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error loading plugins: {ex.Message}", "Error");
        lblStatus.Text = "Error loading plugins";
    }
    finally
    {
        this.Cursor = Cursors.Default;
    }
}

private void PopulatePluginsGrid(List<PluginInfo> documents, List<PluginInfo> reports)
{
    var allPlugins = new List<dynamic>();
    var installed = _pluginManager.GetInstalledPlugins();
    
    // Add document plugins
    foreach (var doc in documents)
    {
        var installedVersion = GetInstalledVersion(doc.Id, installed);
        var hasUpdate = _pluginManager.HasUpdate(
            new PluginInfo { Version = installedVersion ?? "0.0.0" },
            doc
        );
        
        allPlugins.Add(new
        {
            Category = "Document",
            Name = doc.Name,
            Current = installedVersion ?? "Not Installed",
            Available = doc.Version,
            Size = FormatSize(doc.FileSize),
            Status = installedVersion == null ? "⬇" : (hasUpdate ? "🔄" : "✓"),
            PluginId = doc.Id,
            DownloadUrl = doc.DownloadUrl,
            Checksum = doc.Checksum
        });
    }
    
    // Add report plugins
    foreach (var rep in reports)
    {
        var installedVersion = GetInstalledVersion(rep.Id, installed);
        var hasUpdate = _pluginManager.HasUpdate(
            new PluginInfo { Version = installedVersion ?? "0.0.0" },
            rep
        );
        
        allPlugins.Add(new
        {
            Category = "Report",
            Name = rep.Name,
            Current = installedVersion ?? "Not Installed",
            Available = rep.Version,
            Size = FormatSize(rep.FileSize),
            Status = installedVersion == null ? "⬇" : (hasUpdate ? "🔄" : "✓"),
            PluginId = rep.Id,
            DownloadUrl = rep.DownloadUrl,
            Checksum = rep.Checksum
        });
    }
    
    dgvPlugins.DataSource = allPlugins;
}

private string GetInstalledVersion(string pluginId, List<InstalledPlugin> installed)
{
    return installed
        .FirstOrDefault(p => p.Name.Contains(pluginId))
        ?.Version;
}

private string FormatSize(long bytes)
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

private async void dgvPlugins_CellContentClick(object sender, DataGridViewCellEventArgs e)
{
    if (e.ColumnIndex == dgvPlugins.Columns["Download"].Index)
    {
        var pluginId = dgvPlugins.Rows[e.RowIndex].Cells["PluginId"].Value?.ToString();
        var plugin = _manifest?.Documents.FirstOrDefault(p => p.Id == pluginId)
                  ?? _manifest?.Reports.FirstOrDefault(p => p.Id == pluginId);
        
        if (plugin != null)
        {
            await DownloadAndInstallAsync(plugin);
        }
    }
}

private async Task DownloadAndInstallAsync(PluginInfo plugin)
{
    try
    {
        var progress = new Progress<DownloadProgress>(p =>
        {
            pbDownload.Value = Math.Min(100, p.PercentComplete);
            lblStatus.Text = $"Downloading {plugin.Name}... {p.PercentComplete}%";
        });
        
        var success = await _pluginManager.DownloadPluginAsync(plugin, progress);
        
        if (success)
        {
            MessageBox.Show(
                $"{plugin.Name} v{plugin.Version} installed successfully!\n\nNo restart required.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            
            // Refresh plugin list
            pbDownload.Value = 0;
            await RefreshPluginsAsync();
        }
        else
        {
            MessageBox.Show(
                $"Failed to install {plugin.Name}. Check your internet connection.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error");
    }
}

private async void btnRefresh_Click(object sender, EventArgs e)
{
    await RefreshPluginsAsync();
}
```

#### Step 2.3: Build & Test

```bash
# Build the solution
msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release /p:Platform="Any CPU"

# Run the UI
cd DO.VIVICARE.UI\bin\Release
DO.VIVICARE.UI.exe
```

**Expected Test Flow:**
1. Open app → Settings → Plugin Manager tab
2. See list of 16 plugins with statuses
3. Click Download on a plugin
4. Watch progress bar increase
5. Plugin installs successfully
6. List updates showing "✓ Up to date"

---

### **Phase 3: Release First Plugin (Week 2)**

**Example: Release ADI Alta Intensita plugin v1.0.0**

```bash
# 1. Make sure version in AssemblyInfo.cs is 1.0.0
# DO.VIVICARE.Document.ADIAltaIntensita/Properties/AssemblyInfo.cs
# [assembly: AssemblyVersion("1.0.0.0")]

# 2. Commit
git add DO.VIVICARE.Document.ADIAltaIntensita/
git commit -m "Release: Document.ADIAltaIntensita v1.0.0"

# 3. Tag with plugin format
git tag -a plugin/document.adialtaintensita/1.0.0 -m "ADI Alta Intensita v1.0.0"

# 4. Push to trigger GitHub Actions
git push origin plugin/document.adialtaintensita/1.0.0
```

**Expected Result:**
- GitHub Actions builds only this plugin
- Creates `DO.VIVICARE.Document.ADIAltaIntensita-1.0.0.dll`
- Uploads to GitHub Release under `plugin/document.adialtaintensita/1.0.0`
- DLL now available in Plugin Manager

---

### **Phase 4: Update Manifest.json (Week 2)**

**After first plugin release, update manifest with actual checksums:**

```json
{
  "documents": [
    {
      "id": "document.adialtaintensita",
      "name": "ADI Alta Intensita",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.adialtaintensita/1.0.0/DO.VIVICARE.Document.ADIAltaIntensita-1.0.0.dll",
      "checksum": "sha256:ABC123DEF456...",  // Get from GitHub release
      "fileSize": 1258291,
      "releaseDate": "2026-01-11",
      "dependencies": []
    }
  ]
}
```

---

## 📝 **Checklist Before Going Live**

### Installation Phase
- [ ] v1.2.0 released successfully
- [ ] Installer (ZIP) downloads and extracts correctly
- [ ] App launches after extraction
- [ ] No errors in console

### Plugin Manager Phase
- [ ] PluginManager.cs compiles without errors
- [ ] Plugin Manager tab appears in Settings
- [ ] Can load manifest.json from GitHub
- [ ] Lists all 16 plugins correctly
- [ ] Shows correct versions

### Download & Install Phase
- [ ] Can download one plugin successfully
- [ ] Checksum verification works
- [ ] DLL appears in plugin directory
- [ ] No restart required
- [ ] Plugin works after installation

### Update Phase
- [ ] Can detect newer plugin version
- [ ] Update option appears
- [ ] Can update plugin successfully
- [ ] Old version replaced cleanly

---

## 💻 **Development Timeline**

```
Week 1 (Jan 11-17)
├─ [x] Finalize architecture
├─ [ ] Release v1.2.0 (by Jan 15)
└─ [ ] First plugin release (by Jan 17)

Week 2 (Jan 18-24)
├─ [ ] Implement PluginManager.cs
├─ [ ] Add Plugin Manager UI
├─ [ ] Test download/install flow
├─ [ ] Release remaining 15 plugins
└─ [ ] Update manifest.json with actual checksums

Week 3 (Jan 25-31)
├─ [ ] Full end-to-end testing
├─ [ ] User documentation
├─ [ ] Deployment guide
└─ [ ] Go live!
```

---

## 🎯 **Success Criteria**

✅ Users can:
1. Download single installer
2. Install without IT permission
3. Add plugins from within app
4. Update plugins without restart
5. Download 16 plugins independently
6. See versions properly tracked

✅ Developers can:
1. Release app version independently
2. Release plugin version independently
3. See all releases on GitHub
4. Track plugin versions separately
5. Manage manifest centrally

---

## 📞 **Questions?**

Refer to:
- `INSTALLATION_GUIDE.md` - User documentation
- `manifest.json` - Plugin registry
- `.github/workflows/release.yml` - CI/CD logic
- `PluginManager.cs` - C# template

---

**Status**: Ready to implement  
**Owner**: Marco Cavallo  
**Last Updated**: 11 Gennaio 2026
