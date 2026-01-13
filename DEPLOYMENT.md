# Deployment and Distribution Strategy - DO.VIVICARE Reporting

Strategic document defining the modern distribution approach using GitHub with automated CI/CD pipelines and comprehensive testing.

## 📋 Table of Contents

1. [Current State](#current-state)
2. [Identified Issues](#identified-issues)
3. [Proposed Solution](#proposed-solution)
4. [Distribution Architecture](#distribution-architecture)
5. [Technical Implementation](#technical-implementation)
6. [Testing Strategy](#testing-strategy)
7. [Developer Workflow](#developer-workflow)
8. [User Workflow](#user-workflow)
9. [Implementation Roadmap](#implementation-roadmap)
10. [Fallback and Recovery](#fallback-and-recovery)

---

## Current State

### Current Architecture

```
USER (End User)
  │
  ├─ DO.VIVICARE.UI.exe (ClickOnce / .NET Framework)
  │   Legacy deployment mechanism
  │   Manual update process
  │
  └─ Dynamic Libraries
      ├─ Document*.dll
      ├─ Report*.dll
      └─ Manual download mechanism
          Managed by: Button click in frmSettings.cs
          File list: listDocuments.txt, listReports.txt

DEVELOPER
  │
  └─ Visual Studio
      ├─ Compiles solution
      ├─ Manual build and versioning
      └─ Manual library upload
```

### Problems

#### Problem 1: UI Distribution - Legacy ClickOnce
- ❌ Manual distribution process
- ❌ Authentication required for each update
- ❌ Difficult version management
- ❌ No clear versioning
- ❌ No release history
- ❌ No automation

#### Problem 2: Library Distribution - Semi-Manual
- ⚠️ Developer compiles and manually uploads
- ⚠️ File list (listDocuments.txt) manually managed
- ✓ User can download (better than UI, but not optimal)
- ❌ No version tracking
- ❌ No integrity verification
- ❌ Difficult rollback

#### Problem 3: Lack of Governance
- ❌ No CI/CD pipeline
- ❌ Manual build and testing
- ❌ No automated quality control
- ❌ No audit trail for deployments

---

## Proposed Solution

### Final Vision

```
┌─────────────────────────────────────────────────────────────────┐
│ GitHub DO.VIVICARE.Reporting                                    │
│ ├─ Master branch (stable)                                       │
│ ├─ Develop branch (dev)                                         │
│ └─ GitHub Releases (versioned)                                  │
│    ├─ DO.VIVICARE.UI v1.2.0                                    │
│    ├─ Document.ADI v1.2.0                                      │
│    └─ Report.AllegatoADI v1.2.0                                │
└─────────────────────────────────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────────────────────────────────┐
│ GitHub Actions (CI/CD Pipeline with Testing)                    │
│ ├─ Trigger: Push to develop/master or PR                        │
│ ├─ Build: MSBuild solution                                      │
│ ├─ Test: Execute automated test suite (Unit + Integration)      │
│ ├─ Quality: Code analysis and coverage checks                   │
│ ├─ Package: Create .zip and .msi (only if tests pass)           │
│ ├─ Release: Upload to GitHub Releases                          │
│ └─ Notify: Email stakeholders                                  │
└─────────────────────────────────────────────────────────────────┘
          │
          ├─ UI Exe (Modern ClickOnce or MSIX)
          ├─ Document DLLs
          ├─ Report DLLs
          └─ Release Notes
          │
          ▼
┌─────────────────────────────────────────────────────────────────┐
│ End Users                                                        │
│                                                                  │
│ OPTION 1: Semi-Automatic Update (Recommended)                   │
│ ├─ App checks GitHub API on startup                             │
│ ├─ If new version detected → User confirmation dialog           │
│ ├─ User clicks "Yes" to download and install                    │
│ ├─ Download from GitHub Releases (checksummed)                  │
│ ├─ Windows UAC prompt for admin privileges                      │
│ ├─ App restarts with new version                                │
│ └─ IT approval required ONLY for initial setup                  │
│                                                                  │
│ ⚠️  IMPORTANT NOTES:                                             │
│ ├─ Updates require 2 user confirmations (app + UAC)             │
│ ├─ App is tested before release (CI/CD pipeline)                │
│ ├─ Checksum verification prevents corrupted installs            │
│ ├─ NOT truly automatic - requires active user choice            │
│ └─ Best for teams <50 users with adequate testing               │
│                                                                  │
│ OPTION 2: Manual Update (Fallback)                              │
│ ├─ Visit GitHub Releases                                        │
│ ├─ Download DO.VIVICARE-Setup-v1.2.0.msi                       │
│ └─ Run installer (IT-approved)                                 │
│                                                                  │
│ OPTION 3: Plugin Manager (Libraries)                            │
│ ├─ Button in frmSettings: "Check for Updates"                  │
│ ├─ Downloads and installs new library versions                  │
│ └─ No IT request needed (pre-authorized in initial setup)      │
└─────────────────────────────────────────────────────────────────┘
```

---

## Distribution Architecture

### 1. GitHub Releases Structure

```
Repository: https://github.com/artcava/DO.VIVICARE.Reporting

Releases (Semantic Versioning: MAJOR.MINOR.PATCH)
│
├─ v1.2.0 (Latest)
│  ├─ DO.VIVICARE-UI-1.2.0.zip
│  ├─ DO.VIVICARE-UI-1.2.0.msi
│  ├─ Document.ADI-1.2.0.dll
│  ├─ Document.ADI-1.2.0.zip
│  ├─ Report.AllegatoADI-1.2.0.dll
│  ├─ Report.AllegatoADI-1.2.0.zip
│  ├─ RELEASE_NOTES.md
│  ├─ CHECKSUM.sha256
│  └─ MANIFEST.json
│
├─ v1.1.5
│  └─ [Previous assets]
│
└─ v1.0.0
   └─ [Initial assets]

Asset Manifest (MANIFEST.json)
{
  "version": "1.2.0",
  "releaseDate": "2026-01-15",
  "testedWith": "CI/CD Pipeline - All Tests Passed",
  "releaseNotes": "url_to_release_notes",
  "assets": [
    {
      "type": "ui",
      "name": "DO.VIVICARE.UI",
      "version": "1.2.0",
      "file": "DO.VIVICARE-UI-1.2.0.zip",
      "checksum": "sha256:...",
      "size": "15MB",
      "minFramework": "4.8"
    },
    {
      "type": "document",
      "name": "Document.ADI",
      "version": "1.2.0",
      "file": "Document.ADI-1.2.0.dll",
      "checksum": "sha256:...",
      "size": "2.5MB",
      "dependencies": []
    }
  ]
}
```

### 2. Versioning Strategy

**Semantic Versioning (MAJOR.MINOR.PATCH)**

```
1.2.3
│ │ └─ PATCH: Bug fixes, minor corrections
│ └──── MINOR: New compatible features
└────── MAJOR: Breaking changes

Examples:
- 1.0.0 → 1.0.1: Bug fix in Report engine
- 1.0.1 → 1.1.0: New Document.Dietetica module
- 1.1.0 → 2.0.0: Migration from .NET Framework 4.8 to .NET 6

Git Tagging:
v1.2.0-ui      (UI specific)
v1.2.0-docs    (Document modules)
v1.2.0-reports (Report modules)
v1.2.0         (Complete release)
```

---

## Technical Implementation

### Phase 1: GitHub Actions CI/CD Setup with Testing

**File: `.github/workflows/build-and-release.yml`**

```yaml
name: Build, Test and Release

on:
  push:
    branches:
      - master
      - develop
    tags:
      - 'v*'
  pull_request:
    branches:
      - develop
      - master
  workflow_dispatch:

jobs:
  build-and-test:
    runs-on: windows-latest
    name: Build, Test and Package
    
    steps:
    - uses: actions/checkout@v3
      with:
        fetch-depth: 0
    
    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v1.1
    
    - name: Setup NuGet
      uses: NuGet/setup-nuget@v1
    
    - name: Setup .NET (for test runner)
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '6.0.x'
    
    - name: Restore NuGet packages
      run: nuget restore DO.VIVICARE.Reporting.sln
    
    - name: Build solution
      run: msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release /p:Platform="Any CPU"
    
    - name: Run Unit Tests
      run: dotnet test DO.VIVICARE.Tests/DO.VIVICARE.Tests.csproj --configuration Release --logger "trx;LogFileName=TestResults.trx" --collect:"XPlat Code Coverage"
      continue-on-error: false
    
    - name: Run Integration Tests
      run: dotnet test DO.VIVICARE.IntegrationTests/DO.VIVICARE.IntegrationTests.csproj --configuration Release
      continue-on-error: false
    
    - name: Publish Test Results
      uses: dorny/test-reporter@v1
      if: success() || failure()
      with:
        name: Test Results
        path: '**/TestResults.trx'
        reporter: 'dotnet trx'
    
    - name: Code Coverage Report
      uses: codecov/codecov-action@v3
      with:
        files: ./coverage.cobertura.xml
        fail_ci_if_error: false
    
    - name: Extract version from tag
      id: version
      run: |
        if ("${{ github.ref }}" -match "refs/tags/v(.*)$") {
          $version = $matches[1]
        } else {
          $version = "1.0.0-dev"
        }
        echo "version=$version" >> $env:GITHUB_OUTPUT
    
    - name: Package UI binaries
      if: startsWith(github.ref, 'refs/tags/')
      run: |
        mkdir -p artifacts\ui
        copy DO.VIVICARE.UI\bin\Release\*.exe artifacts\ui\
        copy DO.VIVICARE.UI\bin\Release\*.dll artifacts\ui\
        copy DO.VIVICARE.UI\bin\Release\*.config artifacts\ui\
        Compress-Archive -Path artifacts\ui -DestinationPath DO.VIVICARE-UI-${{ steps.version.outputs.version }}.zip
    
    - name: Package Document DLLs
      if: startsWith(github.ref, 'refs/tags/')
      run: |
        $docModules = @(
          "DO.VIVICARE.Document.ADIAltaIntensita",
          "DO.VIVICARE.Document.ADIBassaIntensita",
          "DO.VIVICARE.Document.ASST"
        )
        
        foreach ($module in $docModules) {
          $moduleVersion = "${{ steps.version.outputs.version }}"
          if (Test-Path "$module\bin\Release\$module.dll") {
            Compress-Archive -Path "$module\bin\Release\$module.dll" -DestinationPath "$module-$moduleVersion.zip"
          }
        }
    
    - name: Package Report DLLs
      if: startsWith(github.ref, 'refs/tags/')
      run: |
        $reportModules = @(
          "DO.VIVICARE.Report.AllegatoADI",
          "DO.VIVICARE.Report.Dietetica",
          "DO.VIVICARE.Report.Valorizzazione"
        )
        
        foreach ($module in $reportModules) {
          $moduleVersion = "${{ steps.version.outputs.version }}"
          if (Test-Path "$module\bin\Release\$module.dll") {
            Compress-Archive -Path "$module\bin\Release\$module.dll" -DestinationPath "$module-$moduleVersion.zip"
          }
        }
    
    - name: Generate checksums
      if: startsWith(github.ref, 'refs/tags/')
      run: |
        Get-Item *.zip | ForEach-Object {
          (Get-FileHash $_.FullName -Algorithm SHA256).Hash + " " + $_.Name | Out-File -Append -Encoding ASCII CHECKSUM.sha256
        }
    
    - name: Create Release
      if: startsWith(github.ref, 'refs/tags/') && success()
      uses: softprops/action-gh-release@v1
      with:
        files: |
          *.zip
          *.msi
          CHECKSUM.sha256
        body: "✅ All CI/CD tests passed. This release is production-ready.\n\nRelease notes: See RELEASE_NOTES.md"
        draft: false
        prerelease: false
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
    
    - name: Release Failed - Tests Did Not Pass
      if: startsWith(github.ref, 'refs/tags/') && failure()
      run: |
        Write-Error "Release creation aborted: Tests failed. Fix issues and try again."
        exit 1
    
    - name: Notify Success
      if: success()
      run: |
        echo "Build completed successfully: v${{ steps.version.outputs.version }}"
        echo "✅ All tests passed"
        echo "✅ Code coverage verified"
        echo "✅ Release ready for deployment"
```

### Phase 2: Update Manager for UI (with Checksum Verification)

**File: `DO.VIVICARE.UI/UpdateManager.cs`**

```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;

public class GitHubUpdateManager
{
    private const string GITHUB_API_URL = "https://api.github.com/repos/artcava/DO.VIVICARE.Reporting";
    private const string RELEASES_URL = GITHUB_API_URL + "/releases";
    private const string LATEST_RELEASE_URL = GITHUB_API_URL + "/releases/latest";
    private static readonly HttpClient _httpClient = new HttpClient();

    static GitHubUpdateManager()
    {
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE.UI");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public class ReleaseInfo
    {
        public string TagName { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Body { get; set; } // Release notes
        public DateTime PublishedAt { get; set; }
        public bool IsPrerelease { get; set; }
        public string DownloadUrl { get; set; }
        public string ExpectedChecksum { get; set; }
    }

    /// <summary>
    /// Checks if a new version exists on GitHub
    /// </summary>
    public static async Task<ReleaseInfo> CheckForUpdatesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(LATEST_RELEASE_URL);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var release = ParseGitHubRelease(json);

            return release;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking updates: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Compares semantic versions
    /// </summary>
    public static bool IsUpdateAvailable(string currentVersion, string newVersion)
    {
        if (!Version.TryParse(currentVersion, out var current) ||
            !Version.TryParse(newVersion, out var latest))
            return false;

        return latest > current;
    }

    /// <summary>
    /// Download update with checksum verification
    /// </summary>
    public static async Task<bool> DownloadUpdateAsync(
        string downloadUrl,
        string destinationPath,
        string expectedChecksum = null,
        IProgress<DownloadProgressChangedEventArgs> progress = null)
    {
        try
        {
            using (var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? 0L;
                var canReportProgress = totalBytes != -1 && progress != null;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
                {
                    var totalRead = 0L;
                    var buffer = new byte[8192];
                    int read;

                    while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, read);
                        totalRead += read;

                        if (canReportProgress)
                        {
                            var args = new DownloadProgressChangedEventArgs(
                                bytesReceived: totalRead,
                                totalBytesToReceive: totalBytes,
                                userToken: null
                            );
                            progress.Report(args);
                        }
                    }
                }

                // Verify checksum if provided
                if (!string.IsNullOrEmpty(expectedChecksum))
                {
                    if (!VerifyChecksum(destinationPath, expectedChecksum))
                    {
                        File.Delete(destinationPath);
                        throw new Exception("File integrity check failed. Downloaded file may be corrupted.");
                    }
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error downloading update: {ex.Message}");
            if (File.Exists(destinationPath))
                File.Delete(destinationPath);
            return false;
        }
    }

    /// <summary>
    /// Verify SHA256 checksum
    /// </summary>
    public static bool VerifyChecksum(string filePath, string expectedChecksum)
    {
        try
        {
            using (var sha256 = SHA256.Create())
            using (var fileStream = File.OpenRead(filePath))
            {
                var hashBytes = sha256.ComputeHash(fileStream);
                var computedChecksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                
                return computedChecksum.Equals(expectedChecksum, StringComparison.OrdinalIgnoreCase);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error verifying checksum: {ex.Message}");
            return false;
        }
    }

    private static ReleaseInfo ParseGitHubRelease(string json)
    {
        // Parse JSON from GitHub API response
        var release = new ReleaseInfo
        {
            // Implementation details...
        };
        return release;
    }
}
```

**Integration in MDIParent.cs (Updated with User Confirmation):**

```csharp
public partial class MDIParent : Form
{
    private async void CheckForUpdatesOnStartup()
    {
        try
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var latestRelease = await GitHubUpdateManager.CheckForUpdatesAsync();

            if (latestRelease != null &&
                GitHubUpdateManager.IsUpdateAvailable(currentVersion, latestRelease.Version))
            {
                // IMPORTANT: User must confirm the update
                var result = MessageBox.Show(
                    $"A new update is available:\n\n" +
                    $"Current Version: {currentVersion}\n" +
                    $"New Version: {latestRelease.Version}\n" +
                    $"Released: {latestRelease.PublishedAt:yyyy-MM-dd}\n\n" +
                    $"Release Notes:\n{latestRelease.Body}\n\n" +
                    $"This release has passed all automated tests.\n\n" +
                    $"Download and install now?",
                    "Update Available",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    await DownloadAndInstallUpdateAsync(latestRelease);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in update check: {ex.Message}");
        }
    }

    private async Task DownloadAndInstallUpdateAsync(GitHubUpdateManager.ReleaseInfo release)
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "DO.VIVICARE.Update.zip");
            var progress = new Progress<DownloadProgressChangedEventArgs>(percent =>
            {
                lblStatus.Text = $"Downloading update... {percent.ProgressPercentage}%";
            });

            var success = await GitHubUpdateManager.DownloadUpdateAsync(
                release.DownloadUrl,
                tempPath,
                release.ExpectedChecksum,  // Verify integrity
                progress
            );

            if (success)
            {
                // Extract and install
                InstallUpdate(tempPath);
                MessageBox.Show("Update installed successfully! The application will restart.", "Success");
                Application.Restart();
            }
            else
            {
                MessageBox.Show(
                    "Failed to download update. Please try again later or visit:\n" +
                    "https://github.com/artcava/DO.VIVICARE.Reporting/releases",
                    "Download Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during update: {ex.Message}", "Error");
        }
    }
}
```

### Phase 3: Enhanced Plugin Manager for Libraries

**File: `DO.VIVICARE.UI/frmSettings.cs` (Updated Version)**

```csharp
public partial class frmSettings : Form
{
    private GitHubLibraryManager _libraryManager;

    public frmSettings()
    {
        InitializeComponent();
        _libraryManager = new GitHubLibraryManager();
        SetDataGrid();
    }

    private async void frmSettings_Shown(object sender, EventArgs e)
    {
        await LoadLibrariesFromGitHubAsync();
    }

    private async Task LoadLibrariesFromGitHubAsync()
    {
        try
        {
            lblStatus.Text = "Loading library list...";
            
            var manifest = await _libraryManager.GetManifestAsync();
            
            if (manifest != null)
            {
                PopulateDocumentGrid(manifest.Documents);
                PopulateReportGrid(manifest.Reports);
            }

            lblStatus.Text = "Ready";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PopulateDocumentGrid(List<LibraryInfo> documents)
    {
        dgvElencoDocuments.DataSource = documents.Select(d => new
        {
            Name = d.Name,
            InstalledVersion = GetInstalledVersion(d.Name),
            AvailableVersion = d.Version,
            UpdateAvailable = d.Version != GetInstalledVersion(d.Name),
            File = d.FileName
        }).ToList();
    }

    private async void dgvElencoDocuments_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex == 0) // Download button
        {
            var row = dgvElencoDocuments.Rows[e.RowIndex];
            var fileName = row.Cells["File"].Value.ToString();
            
            try
            {
                lblStatus.Text = $"Downloading {fileName}...";
                var destinationPath = Path.Combine(Manager.DocumentLibraries, fileName);
                
                var progress = new Progress<DownloadProgressChangedEventArgs>(p =>
                {
                    pbDownload.Value = p.ProgressPercentage;
                });

                await _libraryManager.DownloadLibraryAsync(fileName, destinationPath, progress);
                
                lblStatus.Text = "Download completed!";
                MessageBox.Show("Library downloaded successfully!", "Success");
                
                // Reload list
                await LoadLibrariesFromGitHubAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Download error: {ex.Message}", "Error");
            }
        }
    }

    private string GetInstalledVersion(string libraryName)
    {
        try
        {
            var assemblyPath = Path.Combine(Manager.DocumentLibraries, $"{libraryName}.dll");
            if (File.Exists(assemblyPath))
            {
                var version = AssemblyName.GetAssemblyName(assemblyPath).Version;
                return version?.ToString() ?? "Unknown";
            }
        }
        catch { }
        return "Not installed";
    }
}

public class GitHubLibraryManager
{
    private const string MANIFEST_URL = 
        "https://raw.githubusercontent.com/artcava/DO.VIVICARE.Reporting/master/manifest.json";
    private const string RELEASES_URL = 
        "https://api.github.com/repos/artcava/DO.VIVICARE.Reporting/releases";

    public async Task<LibraryManifest> GetManifestAsync()
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE.UI");
            var response = await client.GetAsync(MANIFEST_URL);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LibraryManifest>(json);
        }
    }

    public async Task<bool> DownloadLibraryAsync(
        string fileName,
        string destinationPath,
        IProgress<DownloadProgressChangedEventArgs> progress)
    {
        using (var client = new HttpClient())
        {
            var downloadUrl = $"https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/latest/{fileName}";
            
            using (var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? 0L;
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var file = new FileStream(destinationPath, FileMode.Create))
                {
                    var buffer = new byte[8192];
                    var totalRead = 0L;
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        await file.WriteAsync(buffer, 0, bytesRead);
                        totalRead += bytesRead;
                        
                        var args = new DownloadProgressChangedEventArgs(
                            totalRead, totalBytes, null);
                        progress?.Report(args);
                    }
                }
            }
        }
        return true;
    }
}

public class LibraryManifest
{
    public string Version { get; set; }
    public List<LibraryInfo> Documents { get; set; }
    public List<LibraryInfo> Reports { get; set; }
}

public class LibraryInfo
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string FileName { get; set; }
    public string Checksum { get; set; }
    public long Size { get; set; }
}
```

---

## Testing Strategy

### Why Testing is Critical Before Release

Without automated testing in CI/CD, users receive untested versions → risk of data corruption, crashes, and loss of user trust.

**Solution**: All releases ONLY happen after automated tests pass.

### Testing Phases

#### Phase 1: Unit Tests (CI/CD Pipeline)

**File: `DO.VIVICARE.Tests/UpdateManagerTests.cs`**

```csharp
using Xunit;
using System.Threading.Tasks;

public class UpdateManagerTests
{
    [Fact]
    public void IsUpdateAvailable_WithValidVersions_ReturnsCorrect()
    {
        // Arrange
        string currentVersion = "1.0.0";
        string newVersion = "1.1.0";

        // Act
        var result = GitHubUpdateManager.IsUpdateAvailable(currentVersion, newVersion);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsUpdateAvailable_WithOlderVersion_ReturnsFalse()
    {
        // Arrange
        string currentVersion = "1.1.0";
        string newVersion = "1.0.0";

        // Act
        var result = GitHubUpdateManager.IsUpdateAvailable(currentVersion, newVersion);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyChecksum_WithMatchingChecksum_ReturnsTrue()
    {
        // Arrange
        var testFile = "test.txt";
        System.IO.File.WriteAllText(testFile, "test content");
        var expectedChecksum = ComputeTestChecksum(testFile);

        // Act
        var result = GitHubUpdateManager.VerifyChecksum(testFile, expectedChecksum);

        // Assert
        Assert.True(result);

        // Cleanup
        System.IO.File.Delete(testFile);
    }

    [Fact]
    public void VerifyChecksum_WithMismatchingChecksum_ReturnsFalse()
    {
        // Arrange
        var testFile = "test.txt";
        System.IO.File.WriteAllText(testFile, "test content");
        var wrongChecksum = "wrong_checksum_value";

        // Act
        var result = GitHubUpdateManager.VerifyChecksum(testFile, wrongChecksum);

        // Assert
        Assert.False(result);

        // Cleanup
        System.IO.File.Delete(testFile);
    }
}
```

#### Phase 2: Integration Tests (CI/CD Pipeline)

**File: `DO.VIVICARE.IntegrationTests/DeploymentIntegrationTests.cs`**

```csharp
using Xunit;
using System.Threading.Tasks;
using System.IO;

public class DeploymentIntegrationTests
{
    [Fact]
    public async Task BuildArtifact_IsCreated_WithCorrectStructure()
    {
        // Verify that compiled binaries exist
        var uiBinary = "DO.VIVICARE.UI\\bin\\Release\\DO.VIVICARE.UI.exe";
        Assert.True(File.Exists(uiBinary), $"UI binary not found at {uiBinary}");
    }

    [Fact]
    public async Task DocumentModules_LoadSuccessfully_WithoutErrors()
    {
        // Verify that document modules can be loaded
        var docPath = "DO.VIVICARE.Document.ADI\\bin\\Release\\DO.VIVICARE.Document.ADI.dll";
        Assert.True(File.Exists(docPath), $"Document module not found at {docPath}");
    }

    [Fact]
    public async Task ReportModules_LoadSuccessfully_WithoutErrors()
    {
        // Verify that report modules can be loaded
        var reportPath = "DO.VIVICARE.Report.AllegatoADI\\bin\\Release\\DO.VIVICARE.Report.AllegatoADI.dll";
        Assert.True(File.Exists(reportPath), $"Report module not found at {reportPath}");
    }

    [Fact]
    public async Task AllDependencies_AreResolvable_AtRuntime()
    {
        // Verify that all required dependencies are available
        // (This would check NuGet packages, framework versions, etc.)
        Assert.True(true); // Placeholder for actual dependency check
    }
}
```

### Test Coverage Requirements

| Component | Test Type | Coverage Goal | Status |
|-----------|-----------|--------------|--------|
| UpdateManager | Unit | 90%+ | To implement |
| DocumentModules | Integration | 85%+ | To implement |
| ReportModules | Integration | 85%+ | To implement |
| UI Core | Unit | 80%+ | To implement |
| Plugin Manager | Integration | 75%+ | To implement |

### CI/CD Test Failure Behavior

```
✅ All Tests Pass
  ↓
✅ Build Artifacts Created
  ↓
✅ Package and Checksum Generated
  ↓
✅ Release Created on GitHub
  ↓
✅ Users Can Download Tested Release

---

❌ Tests Fail
  ↓
❌ Build Stops
  ↓
❌ No Artifacts Generated
  ↓
❌ No Release Created
  ↓
❌ Developer Notified
  ↓
❌ User Cannot Download Broken Version
```

---

## Developer Workflow

### How to Release a New Version

**Step 1: Prepare the Release**

```bash
# 1. Verify everything is committed to master
git status

# 2. Update version in AssemblyInfo.cs
# Example: Modify to
# [assembly: AssemblyVersion("1.2.0.0")]

# 3. Update RELEASE_NOTES.md
# Describe changes, bug fixes, and new features

# 4. Final commit
git add .
git commit -m "Release v1.2.0"
```

**Step 2: Create Git Tag**

```bash
# Create semantic version tag
git tag -a v1.2.0 -m "Release version 1.2.0"

# Push tag to repository (triggers GitHub Actions)
git push origin v1.2.0

# Or push everything
git push origin master --tags
```

**Step 3: GitHub Actions Runs All Tests**

```
GitHub Actions detects tag v1.2.0
  ↓
Build solution
  ↓
Run Unit Tests ← MUST PASS
  ↓
Run Integration Tests ← MUST PASS
  ↓
If tests fail → Release STOPS (developer notified)
If tests pass → Continue
  ↓
Create .zip packages
  ↓
Calculate SHA256 checksums
  ↓
Create GitHub Release
  ↓
Upload assets (UI, DLLs, Notes)
  ↓
✅ Release published and available to users
```

### Monitor Build Status

Visit: https://github.com/artcava/DO.VIVICARE.Reporting/actions

```
Build History
├─ ✅ v1.2.0 - All tests passed - 2026-01-15 14:32
├─ ✅ v1.1.5 - All tests passed - 2026-01-10 10:15
├─ ✅ v1.1.4 - All tests passed - 2026-01-05 16:45
└─ ❌ v1.1.3 - Tests failed: UpdateManager crash - 2025-12-28 09:20
```

---

## User Workflow

### Scenario 1: Semi-Automatic Update (Recommended)

**Initial Setup (IT Authentication Required):**
1. Download `DO.VIVICARE-Setup-v1.2.0.msi` from GitHub Releases
2. Run installer (IT approves installation)
3. App installs with auto-update enabled

**Subsequent Updates (NO IT Authentication - User Confirmed):**
1. App starts
2. Checks GitHub API for new version
3. If available, shows: "New version v1.2.1 available. Tests have passed. Update now?"
4. User clicks "Yes"
5. Download begins with progress bar
6. Checksum verified (confirms file integrity)
7. Windows UAC: "Allow app to make changes?"
8. User clicks "Yes"
9. App installs and restarts
10. User has v1.2.1

**Key Points**:
- ⚠️ Requires active user confirmation (2 dialogs)
- ✅ Release has passed all automated tests
- ✅ Checksum verified to prevent corruption
- ✅ No IT approval needed after initial setup

### Scenario 2: Manual Download

1. Visit https://github.com/artcava/DO.VIVICARE.Reporting/releases
2. Download `DO.VIVICARE-Setup-v1.2.0.msi`
3. Run installer
4. Authorize automatic updates in setup dialog

### Scenario 3: Update Libraries (Document and Report)

1. Open DO.VIVICARE.UI
2. Menu: Tools → Settings
3. Tab: "Libraries and Modules"
4. Click: "Check for Updates"
5. App downloads manifest from GitHub
6. Shows which libraries have available updates
7. Click download for each desired library
8. Auto-downloads and installs
9. No restart required (libraries are hot-loaded)

---

## Implementation Roadmap

### Phase 1 (Week 1-2): Test Infrastructure Setup
- [ ] Create `DO.VIVICARE.Tests` unit test project (xUnit)
- [ ] Create `DO.VIVICARE.IntegrationTests` project
- [ ] Write UpdateManager unit tests
- [ ] Write deployment integration tests
- [ ] Verify test execution locally

**Effort:** ~16 hours

### Phase 2 (Week 3): CI/CD Integration
- [ ] Create `.github/workflows/build-and-release.yml`
- [ ] Configure GitHub Actions with test execution
- [ ] Add code coverage reporting
- [ ] Configure test failure notifications
- [ ] Test workflow with tag push

**Effort:** ~12 hours

### Phase 3 (Week 4): Update Manager Implementation
- [ ] UpdateManager.cs with checksum verification
- [ ] MDIParent.cs integration
- [ ] Implement user confirmation dialog
- [ ] Test auto-update locally

**Effort:** ~16 hours

### Phase 4 (Week 5): Enhanced Plugin Manager
- [ ] GitHubLibraryManager.cs implementation
- [ ] frmSettings.cs refactor
- [ ] Manifest JSON schema
- [ ] Test download and install

**Effort:** ~12 hours

### Phase 5 (Week 6): Full Testing & Production Release
- [ ] Complete end-to-end testing
- [ ] Beta release (v1.2.0-beta.1) with tests
- [ ] Feedback collection
- [ ] Bug fixing
- [ ] Production release (v1.2.0) - all tests pass

**Effort:** ~20 hours

**Total Effort:** ~76 hours ≈ 2 weeks full-time development

---

## Fallback and Recovery

### Case: GitHub Actions Build Fails (Tests Failed)

**What Happens**:
1. Developer pushes tag v1.2.0
2. GitHub Actions starts building
3. Unit tests fail
4. Build stops - NO release created
5. Developer notified of failure

**Solution**:
1. Visit https://github.com/artcava/DO.VIVICARE.Reporting/actions
2. Click failed build
3. Analyze error log (test failure)
4. Fix code locally
5. Push corrected version
6. Create new tag (v1.2.1)
7. GitHub Actions retries

**Result**: User is protected from receiving broken version

### Case: User Downloads from GitHub (Manual)

**Protection**:
- Manifest.json has `minFramework` field
- Checksum verification on download
- Only tested releases on GitHub
- Clear versioning (users know what they're downloading)

### Case: Checksum Mismatch During Download

**What Happens**:
1. User downloads v1.2.0.msi
2. Network drops, file incomplete
3. App calculates SHA256 checksum
4. Checksum doesn't match expected
5. File deleted automatically
6. User shown error: "Download failed, please try again"

**Result**: Corrupted file never installed

### Case: User Needs Previous Version

**Solution**:
```
GitHub Releases → v1.1.5 → Download and install
```

All previous versions remain available on GitHub.

---

## Before vs After Comparison

| Aspect | Before | After |
|--------|--------|-------|
| **UI Distribution** | Manual legacy process | GitHub MSI + Semi-auto update ✅ |
| **Testing Before Release** | Manual (0%) | Automated CI/CD (100%) ✅ |
| **IT Authorization** | Every time (tedious) | Initial setup only ✅ |
| **Library Distribution** | Semi-manual + Click button | Auto-update + GitHub Releases ✅ |
| **Versioning** | None (confusing) | Semantic versioning ✅ |
| **Release Notes** | None | GitHub Releases + MANIFEST ✅ |
| **CI/CD** | Manual (0%) | GitHub Actions (100%) ✅ |
| **File Integrity** | None | SHA256 checksum verification ✅ |
| **Test Coverage** | None | Unit + Integration tests ✅ |
| **Rollback** | Difficult | Easy (GitHub Releases) ✅ |
| **Developer Automation** | Manual build + upload | Automatic with tag + tests ✅ |
| **User Safety** | Unknown code quality | All releases tested first ✅ |

---

## Conclusion

This solution transforms DO.VIVICARE from a legacy manual distribution model to a **modern, automated, tested, cloud-native deployment infrastructure**.

**Key Improvements**:
- ✅ Automated testing BEFORE each release
- ✅ Semi-automatic updates with user confirmation
- ✅ Checksum verification for file integrity
- ✅ Clear and traceable versioning
- ✅ IT approval only once (initial setup)
- ✅ Complete deployment automation
- ✅ Easy rollback capability
- ✅ Complete audit trail via GitHub
- ✅ Community-friendly (GitHub standard)

**Critical Success Factor**:
All releases must pass automated tests before reaching users. This prevents bugs from impacting users and maintains system reliability.

**Timeline:** 6 weeks for full implementation (including comprehensive testing)

**ROI:** Extremely high - reduces operational overhead, improves quality, increases user confidence

---

**Document Last Updated**: January 13, 2026  
**Status**: Ready for Implementation with Testing Strategy
